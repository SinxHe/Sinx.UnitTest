namespace Sinx.Collections.Generic
{
	// https://stackoverflow.com/questions/12622539/why-doesnt-generic-icollection-implement-ireadonlycollection-in-net-4-5/12622784#12622784
	public interface ICollection<T> : IReadonlyCollection<T>
	{
		bool IsReadonly { get; }

		void Add(T item);

		void Clear();

		bool Contains(T item);

		void CopyTo(T[] array, int arrayIndex);

		bool Remove(T item);
	}
}