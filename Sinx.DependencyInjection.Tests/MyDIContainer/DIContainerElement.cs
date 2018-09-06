using System;

namespace Sinx.DependencyInjection.Tests.MyDIContainer
{
	/// <summary>
	/// 生命周期管理
	/// </summary>
	internal class DIContainerElement
	{
		private readonly IDIContainerElementLifecyle _lifecycle;
		private object _instance;
		private readonly Func<object> _createInstance;
		public Type Type { get; }

		public DIContainerElement(
			Type type,
			object instance)
			: this(type, IDIContainerElementLifecyle.Singleton, instance, null)
		{
			if (instance == null)
			{
				throw new ArgumentNullException(nameof(instance));
			}
		}
		internal enum IDIContainerElementLifecyle
		{
			Singleton,
			Transient
		}
		public DIContainerElement(
			Type type,
			IDIContainerElementLifecyle lifecycle,
			Func<object> create)
			: this(type, lifecycle, null, create)
		{
			if (create == null)
			{
				throw new ArgumentNullException(nameof(create));
			}
		}

		private DIContainerElement(
			Type type,
			IDIContainerElementLifecyle lifecycle,
			object instance, Func<object> create)
		{
			Type = type;
			_lifecycle = lifecycle;
			_instance = instance;
			_createInstance = create;
		}

		public object GetInstance()
		{
			if (_lifecycle == IDIContainerElementLifecyle.Singleton)
			{
				return _instance ?? (_instance = _createInstance());
			}
			if (_lifecycle == IDIContainerElementLifecyle.Transient)
			{
				return _createInstance();
			}
			throw new Exception();
		}
	}
}
