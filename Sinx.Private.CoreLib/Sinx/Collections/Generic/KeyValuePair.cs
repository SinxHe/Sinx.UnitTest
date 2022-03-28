namespace Sinx.Collections.Generic
{
	public readonly struct KeyValuePair<TKey, TValue>
	{
		public TKey Key { get; }
		public TValue Value { get; }

		public KeyValuePair(TKey key, TValue value)
		{
			Key = key;
			Value = value;
		}
	}
}