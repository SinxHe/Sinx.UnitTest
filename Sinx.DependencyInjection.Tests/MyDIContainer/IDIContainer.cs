using System;

namespace Sinx.DependencyInjection.Tests.MyDIContainer
{
	internal interface IDIContainer
	{
		/// <summary>
		/// 添加服务
		/// </summary>
		void AddService(DIContainerElement element);

		/// <summary>
		/// 获取服务
		/// </summary>
		object GetService(Type type);
	}
}
