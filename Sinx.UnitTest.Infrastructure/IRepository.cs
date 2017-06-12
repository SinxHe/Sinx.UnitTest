using System.Collections.Generic;

namespace Sinx.UnitTest.Infrastructure
{
    public interface IRepository<in TKey, out TAggregateRoot>
		where TAggregateRoot : class, IAggregateRoot<TKey>
    {
	    TAggregateRoot GetAsync(TKey id);
	    IEnumerable<TAggregateRoot> GetAsync(int index, int size);
    }

	public interface IRepository<out TaggregateRoot> : IRepository<object, TaggregateRoot>
		where TaggregateRoot : class, IAggregateRoot<object>
	{
	}
}
