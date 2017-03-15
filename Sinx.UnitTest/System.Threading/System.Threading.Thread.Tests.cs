﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using ThreadState = System.Threading.ThreadState;

namespace Sinx.UnitTest.System.Threading
{
	/// <summary>
	/// 
	/// </summary>
	/// <remarks>
	/// 1. 进程和线程的区别: 
	///		进程好比商店, 线程好比店员, 进程负责提供代码, 图片, 音频等资源(在内存中开辟一块空间), 执行代码的是线程, 一般一个商店只有一个老板娘(主线程), 老板娘死了, 店也黄了;
	/// </remarks>
	public class ThreadTest
	{
		[Theory]
		[InlineData(0, nameof(ParameterizedThreadStart))]
		[InlineData(500, "Func<object>")]
		public void NewThread(int waittime, string except)
		{
			ParameterizedThreadStart func = obj => ((string[])obj)[0] = "Func<object>"; // Func<object>
			string[] param = { nameof(ParameterizedThreadStart) };

			Thread thread0 = new Thread(func);
			// 传入 null
			//thread0.Start(); NullReferenceException

			Thread thread1 = new Thread(func);
			// 传入 非空string
			thread1.Start(param);
			Thread.Sleep(waittime);     // 不进行等待, 那么这里的值应该是 nameof(ParameterizedThreadStart)
			Assert.Equal(param[0], except);
		}

		[Fact]
		public void CurrentThread_GetInfo()
		{
			int threadId = Thread.CurrentThread.ManagedThreadId;    // 线程Id
			Assert.True(threadId > 0);

			// 是否为 后台线程
			bool isBackground = Thread.CurrentThread.IsBackground;
			// Assert.False(isBackground);		// 这个在调试和运行的时候是后台线程, 在使用"实时调试"的时候是前台线程
			// 前台线程: 前台线程执行完程序才能结束
			// 后台线程: 程序可以直接结束不管后台线程的执行状态
			// Thread创建的默认是前台线程, Beginxxx和Task创建的默认是后台线程

			bool isAlive = Thread.CurrentThread.IsAlive;            // 是否 "活着"
			Assert.True(isAlive);

			ThreadState state = Thread.CurrentThread.ThreadState;   // 运行状态
																	// Assert.Equal(state, ThreadState.Running);	// 实时调试 - 前台线程 -> ThreadState.Running	|	调试/运行 - 后台线程 -> ThreadState.Background

			ThreadPriority priority = Thread.CurrentThread.Priority;// 优先级
			Assert.Equal(priority, ThreadPriority.Normal);          // Highest > AboveNormal > Normal > BelowNormal > Lowest

			bool isThreadProolThread = Thread.CurrentThread.IsThreadPoolThread; // 是否是线程池线程
			Assert.False(isThreadProolThread);

			// 当前线程的各种上下文信息
			ExecutionContext executionContext = Thread.CurrentThread.ExecutionContext;  // todo 
																						// 当前线程使用的语言文化
			CultureInfo culture = Thread.CurrentThread.CurrentCulture;
			Assert.Equal(culture.ToString(), "zh-CN");
		}

		[Fact]
		public void CurrentContext_GetContextInfo()
		{
			var context = Thread.CurrentContext;

			int contextId = context.ContextID;  // 上下文Id
			Assert.True(contextId >= 0);

			//context.SetProperty();	// 不允许向默认上下文添加属性, 此方法为支持基础结构的方法, 程序员别随便用
			IContextProperty[] properties = context.ContextProperties;  // 从上下文收集命名信息

			// message: 此上下文已经被冻结
			Assert.Throws<InvalidOperationException>(() => context.Freeze());   // 冻结上下文, 使无法从上下文添加/移除上下文属性
		}

		[Fact]
		public void CurrentPrincipal_GetPrincipalInfo()
		{
			// 当前线程的负责人(对基于角色的安全性而言)
			IIdentity identity = Thread.CurrentPrincipal.Identity;      // 获取用户标识
			Assert.Equal(identity.Name, string.Empty);                  // 默认角色名称 ""
			Assert.False(identity.IsAuthenticated);                     // 默认是没有进行身份验证
			Assert.Equal(identity.AuthenticationType, string.Empty);    // 验证类型 ""
			GenericIdentity genericIdentity = identity as GenericIdentity;  // TODO 以后学习

			bool isInRole = Thread.CurrentPrincipal.IsInRole(string.Empty);
			Assert.True(isInRole);
		}

		/// <summary> </summary>
		/// <remarks>
		/// 线程上下文: TODO
		///		
		/// </remarks>
		[Fact]
		public void SynchronizationContext_Create()
		{
			// 获取当前线程的同步上下文
			SynchronizationContext context = SynchronizationContext.Current;
			context.OperationStarted();                     // 空方法, 在派生类中重写以响应操作 开始 执行的回调
			context.OperationCompleted();                   // 空方法, 在派生类中重写以响应操作 完成 执行的回调
			var b = context.IsWaitNotificationRequired();   // 确定是否等待通知
															// SendOrPostCallback - Action<object> - 表示在消息即将被调度到同步上下文时要调用的方法。
															//context.Post((SendOrPostCallback)null, (object)null);	// 在派生类中重写, 将异步消息分派到同步上下文
															//context.Send((SendOrPostCallback)null, (object)null);
															//context.Wait((IntPtr[])null, waitAll: true, millisecondsTimeout: 1000);	// 等待数组中的任意元素或所有元素接收信号
		}

		[Theory]
		[InlineData(100, 1)]
		[InlineData(0, 2)]
		public void ConExecute_UseLock(int waittime, int expect)
		{
			var ar = new[] { 0 };
			Thread thd1 = new Thread(() =>
			{
				Thread.Sleep(waittime);
				ar[0] = 1;
			});
			Thread thd2 = new Thread(() => ar[0] = 2);
			thd1.Start();
			thd2.Start();
			Thread.Sleep(waittime + 500);
			Assert.Equal(expect, ar[0]);

			var exeAr = new object();
			thd1 = new Thread(() =>
			{
				lock (exeAr)
				{
					Thread.Sleep(waittime);
					ar[0] = 1;
				}
			});
			thd2 = new Thread(() =>
			{
				Thread.Sleep(5);
				lock (exeAr)
				{
					ar[0] = 2;
				}
			});
			thd1.Start();
			thd2.Start();
			Thread.Sleep(waittime + 500);
			lock (exeAr)
			{
				Assert.Equal(2, ar[0]);
			}
		}

		[Fact]
		public void ConExecute_UseMonitor()
		{
			var ar = new[] { 0 };
			var exeAr = new object();
			Thread thd1 = new Thread(() =>
			{
				Monitor.Enter(exeAr);
				try
				{
					Thread.Sleep(500);
					ar[0] = 1;
				}
				finally		// 其实在 lock 中帮忙做了这个
				{
					Monitor.Exit(exeAr);
				}
			});
			Thread thd2 = new Thread(() =>
			{
				Monitor.Enter(exeAr);
				try
				{
					ar[0] = 2;
				}
				finally     // 其实在 lock 中帮忙做了这个
				{
					Monitor.Exit(exeAr);
				}
			});
			thd1.Start();
			Thread.Sleep(100);
			thd2.Start();
			Thread.Sleep(100);	// 恢复线程2的时候要保证当前线程不会锁住exeAr
			Monitor.Enter(exeAr);
			try
			{
				Assert.Equal(2, ar[0]);
			}
			finally     // 其实在 lock 中帮忙做了这个
			{
				Monitor.Exit(exeAr);
			}
		}
	}
}