using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SerializableCollections
{
#if UNITY_EDITOR
	[GUI.UsePropertyDrawer(typeof(GUI.HashSetDrawer))]
#endif
	public class HashSet<T> : SerializableCollection, ISet<T>, ISerializationCallbackReceiver, ICollection, ICollection<T>, IEnumerable<T>, IEnumerable, IReadOnlyCollection<T>
	{
		protected ISet<T> Implementation { get; set; }
		[SerializeField]
		private System.Collections.Generic.List<T> values = new System.Collections.Generic.List<T>();
		[SerializeField]
		private T value = default;
#pragma warning disable 0414
		[SerializeField]
		private bool hasLostValue = false;
#pragma warning restore 0414

		public HashSet()
		{
			Implementation = new System.Collections.Generic.HashSet<T>();
		}

		public HashSet(ISet<T> implementation)
		{
			Implementation = implementation;
		}

		public HashSet(IEqualityComparer<T> comparer)
		{
			Implementation = new System.Collections.Generic.HashSet<T>(comparer);
		}

		public HashSet(IEnumerable<T> collection)
		{
			ClearTemp();
		}

		private void AddTemp()
		{
			Add(value);
			ClearTemp();
		}

		private void ClearTemp()
		{
			if (!typeof(T).IsValueType && typeof(T).GetConstructor(Type.EmptyTypes) != null)
				value = Activator.CreateInstance<T>();
			else
				value = default;
		}

		public void OnBeforeSerialize()
		{
			values.Clear();

			foreach (var item in Implementation)
			{
				values.Add(item);
			}
		}

		public void OnAfterDeserialize()
		{
			Implementation.Clear();
			for (var i = 0; i < values.Count && i < values.Count; i++)
			{
				if (!Implementation.Contains(values[i]))
					Implementation.Add(values[i]);
				else
				{
					value = values[i];
					hasLostValue = true;
				}
			}
#if !UNITY_EDITOR
			values.Clear();
			ClearTemp();
#endif
		}

		public int Count => Implementation.Count;
		public bool IsReadOnly => Implementation.IsReadOnly;

		public bool IsSynchronized => ((ICollection)values).IsSynchronized;

		public object SyncRoot => ((ICollection)values).SyncRoot;

		void ICollection<T>.Add(T item) => Add(item);
		public virtual bool Add(T item) => Implementation.Add(item);
		public virtual void Clear() => Implementation.Clear();
		public bool Contains(T item) => Implementation.Contains(item);
		public void CopyTo(T[] array, int arrayIndex) => Implementation.CopyTo(array, arrayIndex);
		public void ExceptWith(IEnumerable<T> other) => Implementation.ExceptWith(other);
		public IEnumerator<T> GetEnumerator() => Implementation.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		public void IntersectWith(IEnumerable<T> other) => Implementation.IntersectWith(other);
		public bool IsProperSubsetOf(IEnumerable<T> other) => Implementation.IsProperSubsetOf(other);
		public bool IsProperSupersetOf(IEnumerable<T> other) => Implementation.IsProperSupersetOf(other);
		public bool IsSubsetOf(IEnumerable<T> other) => Implementation.IsSubsetOf(other);
		public bool IsSupersetOf(IEnumerable<T> other) => Implementation.IsSupersetOf(other);
		public bool Overlaps(IEnumerable<T> other) => Implementation.Overlaps(other);
		public virtual bool Remove(T item) => Implementation.Remove(item);
		public virtual int RemoveWhere(Predicate<T> match)
		{
			System.Collections.Generic.HashSet<T> removals = new System.Collections.Generic.HashSet<T>();
			foreach (var item in Implementation)
				if (match.Invoke(item))
					removals.Add(item);

			int removeCount = removals.Count;
			foreach (var item in removals)
				Implementation.Remove(item);

			return removeCount;
		}
		public bool SetEquals(IEnumerable<T> other) => Implementation.SetEquals(other);
		public void SymmetricExceptWith(IEnumerable<T> other) => Implementation.SymmetricExceptWith(other);
		public void UnionWith(IEnumerable<T> other) => Implementation.UnionWith(other);

		public void CopyTo(Array array, int index)
		{
			((ICollection)values).CopyTo(array, index);
		}
	}
}