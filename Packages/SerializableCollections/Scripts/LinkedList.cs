using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SerializableCollections
{
#if UNITY_EDITOR
	[GUI.UsePropertyDrawer(typeof(GUI.LinkedListDrawer))]
#endif
	public class LinkedList<T> : SerializableCollection, ISerializationCallbackReceiver, ICollection<T>, IEnumerable<T>, IEnumerable, IReadOnlyCollection<T>, ICollection
	{
		private System.Collections.Generic.LinkedList<T> linkedList = new System.Collections.Generic.LinkedList<T>();
		[SerializeField]
		private System.Collections.Generic.List<T> values = new System.Collections.Generic.List<T>();

		[SerializeField]
		private T value;

		public LinkedList()
		{
			linkedList = new System.Collections.Generic.LinkedList<T>();
		}

		public LinkedList(IEnumerable<T> collection)
		{
			linkedList = new System.Collections.Generic.LinkedList<T>(collection);
		}

		private void AddTemp()
		{
			linkedList.AddLast(value);
			ClearTemp();
		}

		private void ClearTemp()
		{
			value = default;
		}

		public void OnBeforeSerialize()
		{
			values.Clear();

			foreach (var value in linkedList)
			{
				values.Add(value);
			}
		}

		public void OnAfterDeserialize()
		{
			linkedList.Clear();
			for (var i = 0; i < values.Count && i < values.Count; i++)
			{
				if (!linkedList.Contains(values[i]))
					linkedList.AddLast(values[i]);
			}

		}

		public int Count => linkedList.Count;
		public bool IsReadOnly => ((ICollection<T>)linkedList).IsReadOnly;
		public bool IsSynchronized => ((ICollection)linkedList).IsSynchronized;
		public object SyncRoot => ((ICollection)linkedList).SyncRoot;

		public void Add(T value) => linkedList.AddLast(value);
		public LinkedListNode<T> AddAfter(LinkedListNode<T> node, T value) => linkedList.AddAfter(node, value);
		public LinkedListNode<T> AddBefore(LinkedListNode<T> node, T value) => linkedList.AddBefore(node, value);
		public LinkedListNode<T> AddFirst(T value) => linkedList.AddFirst(value);
		public LinkedListNode<T> AddLast(T value) => linkedList.AddLast(value);
		public void Clear() => linkedList.Clear();
		public bool Contains(T value) => linkedList.Contains(value);
		public void CopyTo(T[] array, int arrayIndex) => linkedList.CopyTo(array, arrayIndex);
		public void CopyTo(Array array, int index) => ((ICollection)linkedList).CopyTo(array, index);
		public LinkedListNode<T> Find(T value) => linkedList.Find(value);
		public LinkedListNode<T> FindLast(T value) => linkedList.FindLast(value);
		public IEnumerator<T> GetEnumerator() => linkedList.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => linkedList.GetEnumerator();
		public bool Remove(T value) => linkedList.Remove(value);
		public void Remove(LinkedListNode<T> node) => linkedList.Remove(node);
		public void RemoveFirst() => linkedList.RemoveFirst();
		public void RemoveLast() => linkedList.RemoveLast();
	}
}
