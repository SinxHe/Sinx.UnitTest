namespace Sinx.UnitTest.Infrastructure
{
    public interface IEntity<TKey>
    {
	    TKey Id { get; set; }
    }

	public interface IEntity : IEntity<object>
	{
	}
}
