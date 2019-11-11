using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SerializableCollections
{
#if UNITY_EDITOR
	[GUI.UsePropertyDrawer(typeof(GUI.LinkedListDrawer))]
#endif
	public class Queue<T> : SerializableCollection, ISerializationCallbackReceiver, IEnumerable<T>, IEnumerable, IReadOnlyCollection<T>, ICollection
	{
		private System.Collections.Generic.Queue<T> queue;
		[SerializeField]
		private System.Collections.Generic.List<T> values = new System.Collections.Generic.List<T>();
		[SerializeField]
		private T value;

		private void AddTemp()
		{
			Enqueue(value);
			ClearTemp();
		}

		private void ClearTemp()
		{
			value = default;
		}

		public void OnBeforeSerialize()
		{
			values.Clear();

			foreach (var item in queue)
			{
				values.Add(item);
			}
		}

		public void OnAfterDeserialize()
		{
			queue.Clear();

			for (var i = 0; i < values.Count && i < values.Count; i++)
			{
				if (!queue.Contains(values[i]))
					queue.Enqueue(values[i]);
			}
		}

		public Queue() => queue = new System.Collections.Generic.Queue<T>();
		public Queue(IEnumerable<T> collection) => queue = new System.Collections.Generic.Queue<T>(collection);
		public Queue(int capacity) => queue = new System.Collections.Generic.Queue<T>(capacity);

		public void Clear() => queue.Clear();
		public bool Contains(T item) => queue.Contains(item);
		public void CopyTo(T[] array, int arrayIndex) => queue.CopyTo(array, arrayIndex);
		public void CopyTo(Array array, int index) => ((ICollection)queue).CopyTo(array, index);
		public int Count => queue.Count;
		public T Dequeue() => queue.Dequeue();
		public void Enqueue(T item) => queue.Enqueue(item);
		public IEnumerator<T> GetEnumerator() => queue.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		public bool IsSynchronized => ((ICollection)queue).IsSynchronized;
		public T Peek() => queue.Peek();
		public object SyncRoot => ((ICollection)queue).SyncRoot;
		public T[] ToArray => queue.ToArray();
		public void TrimExcess() => queue.TrimExcess();
	}
}
