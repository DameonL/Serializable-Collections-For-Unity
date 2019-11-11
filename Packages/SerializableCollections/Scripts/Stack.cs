using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SerializableCollections
{
#if UNITY_EDITOR
	[GUI.UsePropertyDrawer(typeof(GUI.LinkedListDrawer))]
#endif
	public class Stack<T> : SerializableCollection, ISerializationCallbackReceiver, IEnumerable<T>, IEnumerable, IReadOnlyCollection<T>, ICollection
	{
		private System.Collections.Generic.Stack<T> stack = new System.Collections.Generic.Stack<T>();
		[SerializeField]
		private System.Collections.Generic.List<T> values = new System.Collections.Generic.List<T>();
		[SerializeField]
		private T value;

		public bool IsSynchronized => ((ICollection)stack).IsSynchronized;
		public object SyncRoot => ((ICollection)stack).SyncRoot;
		public int Count => stack.Count;

		private void AddTemp()
		{
			stack.Push(value);
			ClearTemp();
		}

		private void ClearTemp()
		{
			value = default;
		}

		public void OnBeforeSerialize()
		{
			values.Clear();
			foreach (var item in stack)
			{
				values.Add(item);
			}
		}

		public void OnAfterDeserialize()
		{
			stack.Clear();
			for (var i = values.Count - 1; i > -1; i--)
			{
				if (!stack.Contains(values[i]))
					stack.Push(values[i]);
			}
		}

		public virtual void Clear() => stack.Clear();
		public bool Contains(T item) => stack.Contains(item);
		public void CopyTo(T[] array, int arrayIndex) => stack.CopyTo(array, arrayIndex);
		public void CopyTo(Array array, int index) => ((ICollection)stack).CopyTo(array, index);
		public IEnumerator<T> GetEnumerator() => stack.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		public T Peek() => stack.Peek();
		public virtual T Pop() => stack.Pop();
		public virtual void Push(T item) => stack.Push(item);
		public T[] ToArray => stack.ToArray();
		public void TrimExcess() => stack.TrimExcess();
	}
}