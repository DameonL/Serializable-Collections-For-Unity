using SerializableCollections.GUI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SerializableCollections
{
#if UNITY_EDITOR
	[UsePropertyDrawer(typeof(DictionaryDrawer))]
#endif
 public class Dictionary<KeyType, ValueType> : SerializableCollection, ISerializationCallbackReceiver, ICollection<KeyValuePair<KeyType, ValueType>>, IEnumerable<KeyValuePair<KeyType, ValueType>>, IEnumerable, IDictionary<KeyType, ValueType>, ICollection
	{
		[SerializeField]
		private System.Collections.Generic.List<KeyType> keys = new System.Collections.Generic.List<KeyType>();
		[SerializeField]
		private System.Collections.Generic.List<ValueType> values = new System.Collections.Generic.List<ValueType>();

		protected IDictionary<KeyType, ValueType> Implementation { get; set; }

		// This key and value is used by the property drawer to add new keys/values to the dicitonary.
		[SerializeField]
		private KeyType key = default;
		protected KeyType Key { get { return key; } set { key = value; } }
		[SerializeField]
		private ValueType value = default;
		protected ValueType Value { get { return value; } set { this.value = value; } }
#pragma warning disable 0414
		[SerializeField]
		private bool hasLostValue = false;
#pragma warning restore 0414

		public ValueType this[KeyType key]
		{
			get { return Implementation[key]; }
			set { Implementation[key] = value; }
		}

		public Dictionary() : base()
		{
			Implementation = new System.Collections.Generic.Dictionary<KeyType, ValueType>();
		}

		public Dictionary(IDictionary<KeyType, ValueType> implementation)
		{
			Implementation = implementation;
		}

		public Dictionary(IEqualityComparer<KeyType> comparer)
		{
			Implementation = new System.Collections.Generic.Dictionary<KeyType, ValueType>(comparer);
		}

		public Dictionary(int capacity)
		{
			Implementation = new System.Collections.Generic.Dictionary<KeyType, ValueType>(capacity);
		}

		public Dictionary(int capacity, IEqualityComparer<KeyType> comparer)
		{
			Implementation = new System.Collections.Generic.Dictionary<KeyType, ValueType>(capacity, comparer);
		}

		private void AddTemp()
		{
			Add(key, value);
			ClearTemp();
		}

		private void ClearTemp()
		{
#if UNITY_EDITOR
			if (!typeof(KeyType).IsValueType && typeof(KeyType).GetConstructor(Type.EmptyTypes) != null)
				key = Activator.CreateInstance<KeyType>();
			else
				key = default;

			if (!typeof(ValueType).IsValueType && typeof(ValueType).GetConstructor(Type.EmptyTypes) != null && !(typeof(ValueType).IsSubclassOf(typeof(UnityEngine.Object))))
				value = Activator.CreateInstance<ValueType>();
			else
				value = default;
#else
				key = default;
				value = default;
#endif
		}

		public void OnBeforeSerialize()
		{
			keys.Clear();
			values.Clear();

			foreach (var pair in Implementation)
			{
				keys.Add(pair.Key);
				values.Add(pair.Value);
			}
		}

		public void OnAfterDeserialize()
		{
			Implementation.Clear();
			for (var i = 0; i < keys.Count && i < values.Count; i++)
			{
				if (!Implementation.ContainsKey(keys[i]))
					Implementation.Add(keys[i], values[i]);
				else
				{
					key = keys[i];
					value = values[i];
					hasLostValue = true;
				}
			}
#if !UNITY_EDITOR
			keys.Clear();
			values.Clear();
			ClearTemp();
#endif
		}

		public ICollection<KeyType> Keys => Implementation.Keys;
		public ICollection<ValueType> Values => Implementation.Values;
		public int Count => Implementation.Count;
		public bool IsReadOnly => Implementation.IsReadOnly;

		public bool IsSynchronized => (Implementation is ICollection) ? ((ICollection)Implementation).IsSynchronized : throw new NotImplementedException();

		public object SyncRoot => (Implementation is ICollection) ? ((ICollection)Implementation).SyncRoot : throw new NotImplementedException();

		public virtual void Add(KeyType key, ValueType value) => Implementation.Add(key, value);
		public void Add(KeyValuePair<KeyType, ValueType> item) => Add(item.Key, item.Value);
		public virtual void Clear() => Implementation.Clear();
		public bool Contains(KeyValuePair<KeyType, ValueType> item) => Implementation.Contains(item);
		public bool ContainsKey(KeyType key) => Implementation.ContainsKey(key);
		public bool ContainsValue(ValueType value)
		{
			foreach (var dictionaryValue in Implementation.Values)
				if (value.Equals(dictionaryValue))
					return true;

			return false;
		}
		public void CopyTo(KeyValuePair<KeyType, ValueType>[] array, int arrayIndex) => Implementation.CopyTo(array, arrayIndex);
		public IEnumerator<KeyValuePair<KeyType, ValueType>> GetEnumerator() => Implementation.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		public virtual bool Remove(KeyType key) => Implementation.Remove(key);
		public virtual bool Remove(KeyValuePair<KeyType, ValueType> item) => Remove(item.Key);
		bool IDictionary<KeyType, ValueType>.Remove(KeyType key) => Remove(key);
		public bool TryGetValue(KeyType key, out ValueType value) => Implementation.TryGetValue(key, out value);

		public void CopyTo(Array array, int index)
		{
			if (Implementation is ICollection)
				((ICollection)Implementation).CopyTo(array, index);
			else
				throw new NotImplementedException();
		}
	}
}