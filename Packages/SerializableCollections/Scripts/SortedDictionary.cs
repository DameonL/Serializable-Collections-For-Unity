using System.Collections.Generic;

namespace SerializableCollections
{
	public class SortedDictionary<KeyType, ValueType> : Dictionary<KeyType, ValueType>, IReadOnlyCollection<KeyValuePair<KeyType, ValueType>>, IReadOnlyDictionary<KeyType, ValueType>, SortedCollection<KeyType>
	{
		private System.Collections.Generic.SortedDictionary<KeyType, ValueType> sortedDictionary;

		public SortedDictionary()
		{
			sortedDictionary = new System.Collections.Generic.SortedDictionary<KeyType, ValueType>();
			Implementation = sortedDictionary;
		}

		public SortedDictionary(IComparer<KeyType> comparer)
		{
			sortedDictionary = new System.Collections.Generic.SortedDictionary<KeyType, ValueType>(comparer);
			Implementation = sortedDictionary;
		}

		public SortedDictionary(IDictionary<KeyType, ValueType> dictionary)
		{
			sortedDictionary = new System.Collections.Generic.SortedDictionary<KeyType, ValueType>(dictionary);
			Implementation = sortedDictionary;
		}

		public SortedDictionary(IDictionary<KeyType, ValueType> dictionary, IComparer<KeyType> comparer)
		{
			sortedDictionary = new System.Collections.Generic.SortedDictionary<KeyType, ValueType>(dictionary, comparer);
			Implementation = sortedDictionary;
		}

		public IComparer<KeyType> Comparer => sortedDictionary.Comparer;
		IEnumerable<KeyType> IReadOnlyDictionary<KeyType, ValueType>.Keys => ((IReadOnlyDictionary<KeyType, ValueType>)sortedDictionary).Keys;
		IEnumerable<ValueType> IReadOnlyDictionary<KeyType, ValueType>.Values => ((IReadOnlyDictionary<KeyType, ValueType>)sortedDictionary).Values;
	}
}
