using System;
using System.Collections.Generic;

namespace SerializableCollections
{
	public class SortedSet<T> : HashSet<T>, SortedCollection<T>
	{
		private System.Collections.Generic.SortedSet<T> sortedSet;

		public SortedSet() { }
		public SortedSet(IComparer<T> comparer)
		{
			sortedSet = new System.Collections.Generic.SortedSet<T>(comparer);
			Implementation = sortedSet;
		}

		public SortedSet(IEnumerable<T> collection)
		{
			sortedSet = new System.Collections.Generic.SortedSet<T>(collection);
			Implementation = sortedSet;
		}

		public SortedSet(IEnumerable<T> collection, IComparer<T> comparer)
		{
			sortedSet = new System.Collections.Generic.SortedSet<T>(collection, comparer);
			Implementation = sortedSet;
		}

		public SortedSet<T> GetViewBetween(T lowerValue, T upperValue)
		{
			return new SortedSet<T>(sortedSet.GetViewBetween(lowerValue, upperValue));
		}

		public IComparer<T> Comparer => sortedSet.Comparer;
		public T Min => sortedSet.Min;
		public T Max => sortedSet.Max;

		public void CopyTo(T[] array, int index, int count) => sortedSet.CopyTo(array, index, count);
		public void CopyTo(T[] array) => sortedSet.CopyTo(array);
		public IEnumerable<T> Reverse() => sortedSet.Reverse();
	}
}
