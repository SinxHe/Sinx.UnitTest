using System;
using System.Reflection;
using System.Reflection.Emit;
using Xunit;

namespace Sinx.Reflection.Tests
{
	public class EmitDynamicMethodTests
	{
		private long test = 42;

		/// <summary>
		/// ��������ִ�з�����ί������
		/// </summary>
		private delegate TReturn OneParameter<out TReturn, in TParameter>(TParameter p);

		/// <summary>
		/// �����ִ�а󶨵�ģ��Ķ�̬����
		/// </summary>
		[Fact]
		public void EmitDynamicMethod_BindToModule()
		{
			// ������̬����
			Type[] methodArgs = { typeof(int) };
			var squareIt = new DynamicMethod(
				// ����ҪΪ��̬��������, ���Ҳ���ͨ�����Ƶ�������
				// �����̬�������Ծ�����ͬ������, ���ƽ��ڵ��ö�ջ����ʾ���ҿ����ڵ���
				"SquareIt",
				// ��������
				typeof(long),
				// ����Ϊ��̬����ָ���������͵�����
				methodArgs,
				// �÷��������EmitDynamicMethodTests���ģ����й���
				// ��ʵ����ָ���κμ��ص�ģ��, ��̬��������Ϊ������ģ�鼶���static����
				typeof(EmitDynamicMethodTests).Module);

			// ������������, ����ʹ��ILGenerator���󷢳�IL
			// Ҳ����ʹ��DynamicILInfo��������йܴ�������������
			ILGenerator il = squareIt.GetILGenerator();
			// ������0���õ���ֵ��ջ��
			il.Emit(OpCodes.Ldarg_0);
			// ����ֵ��ջջ��������ת����int64
			il.Emit(OpCodes.Conv_I8);
			// ������ֵջ����ֵ, Ȼ��ѹ����ֵջ
			il.Emit(OpCodes.Dup);
			// ��������ֵ, �����ѹ��ջ
			il.Emit(OpCodes.Mul);
			// �ӵ�ǰ��������, �����ֵ��ջ����ֵ, ��ֵ��������ߵ���ֵ��ջ
			il.Emit(OpCodes.Ret);
			// ͨ������CreateDelegate����������ʾ��̬������ί�е�ʵ��
			// ����ί�м���ʾ�÷����������Ѿ����, �����κθ��ķ����ĳ��Զ���������
			var invokeSquareIt =
				(OneParameter<long, int>)
				squareIt.CreateDelegate(typeof(OneParameter<long, int>));
			var p = 3;
			var actual = invokeSquareIt(p);
			Assert.Equal(9, actual);
		}

		/// <summary>
		/// �����ִ�а󶨵�����Ķ�̬����
		/// </summary>
		[Fact]
		public void EmitDynamicMethod_BindToClass()
		{
			// ������̬����
			// ���������ί��Ҫ�󶨵�����, ��һ������������ί��Ҫ�󶨵�������ƥ��
			var methodArgs = new[] { GetType(), typeof(int) };
			var multiplyHidden = new DynamicMethod(
				// ����
				string.Empty,
				// ��������
				typeof(int),
				// ����
				methodArgs,
				// �󶨵�������
				GetType());

			// ������̬����
			ILGenerator iLGenerator = multiplyHidden.GetILGenerator();
			// �����Ͳ����ŵ���ջ
			iLGenerator.Emit(OpCodes.Ldarg_0);
			var testInfo = GetType().GetField("test", BindingFlags.NonPublic | BindingFlags.Instance);
			// Ѱ�ҵ�ǰ��ֵ��ջ�����õĶ����һ���ֶ�ֵ
			iLGenerator.Emit(OpCodes.Ldfld, testInfo);
			// ���ڶ����������ص���ջ��
			iLGenerator.Emit(OpCodes.Ldarg_1);
			// ������ֵ���
			iLGenerator.Emit(OpCodes.Mul);
			// ���������
			iLGenerator.Emit(OpCodes.Ret);
			// ����ί��
			OneParameter<int, int> invoker =
				(OneParameter<int, int>)
				multiplyHidden.CreateDelegate(
					typeof(OneParameter<int, int>),
					Activator.CreateInstance(GetType()));
			var actual = invoker(3);
			Assert.Equal(3 * 42, actual);
		}
	}
}
