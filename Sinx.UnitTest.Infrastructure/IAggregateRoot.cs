namespace Sinx.UnitTest.Infrastructure
{
	public interface IAggregateRoot<TKey> : IEntity<TKey>
	{
	}

	public interface IAggregateRoot : IAggregateRoot<object>
    {
    }
}
