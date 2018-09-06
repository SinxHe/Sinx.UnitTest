using System;
using System.Collections.Generic;
using System.Linq;

namespace Sinx.DependencyInjection.Tests.MyDIContainer
{
	internal class MyDIContainer : IDIContainer
	{
		private readonly IList<DIContainerElement> _elements = 
			new List<DIContainerElement>();
		public void AddService(DIContainerElement element)
		{
			_elements.Add(element);
		}

		public object GetService(Type type)
		{
			return _elements.LastOrDefault(e => e.Type == type)?.GetInstance();
		}
	}
}
