using SerializableCollections.GUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SerializableCollections
{
	public class SortedList<KeyType, ValueType> : Dictionary<KeyType, ValueType>, SortedCollection<KeyType>
	{
		private System.Collections.Generic.SortedList<KeyType, ValueType> sortedList;

		public SortedList()
		{
			sortedList = new System.Collections.Generic.SortedList<KeyType, ValueType>();
			Implementation = sortedList;
		}

		public SortedList(int capacity)
		{
			sortedList = new System.Collections.Generic.SortedList<KeyType, ValueType>(capacity);
			Implementation = sortedList;
		}

		public SortedList(IDictionary<KeyType, ValueType> dictionary)
		{
			sortedList = new System.Collections.Generic.SortedList<KeyType, ValueType>(dictionary);
			Implementation = sortedList;
		}

		public SortedList(int capacity, IComparer<KeyType> comparer)
		{
			sortedList = new System.Collections.Generic.SortedList<KeyType, ValueType>(capacity, comparer);
			Implementation = sortedList;
		}

		public SortedList(IDictionary<KeyType, ValueType> dictionary, IComparer<KeyType> comparer)
		{
			sortedList = new System.Collections.Generic.SortedList<KeyType, ValueType>(dictionary, comparer);
			Implementation = sortedList;
		}

		public int Capacity => sortedList.Capacity;
		public IComparer<KeyType> Comparer => sortedList.Comparer;

		public ValueType this[int key] => sortedList.Values[key];
		public int IndexOfValue(ValueType value) => sortedList.IndexOfValue(value);
		public int IndexofKey(KeyType key) => sortedList.IndexOfKey(key);
		public void TrimExcess() => sortedList.TrimExcess();
	}
}
