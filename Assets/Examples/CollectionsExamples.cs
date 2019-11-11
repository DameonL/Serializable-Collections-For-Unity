using System;
using UnityEngine;

namespace SerializableCollections.Examples
{
	public class CollectionsExamples : MonoBehaviour
	{
		[Serializable]
		public class StringIntDictionary : Dictionary<string, int> { }
		[SerializeField]
		private StringIntDictionary dictionary = new StringIntDictionary();

		[Serializable]
		public class IntSet : HashSet<int> { }
		[SerializeField]
		private IntSet hashSet = new IntSet();

		[Serializable]
		public class IntList : List<int> { }
		[SerializeField]
		private IntList list = new IntList();

		[Serializable]
		public class StringLinkedList : LinkedList<string> { }
		[SerializeField]
		private StringLinkedList linkedList = new StringLinkedList();

		[Serializable]
		public class SortedStringListList : List<SortedStringList> {}
		[Serializable]
		public class SortedListSet : HashSet<SortedStringListList> { }
		[SerializeField]
		private SortedListSet nestedCollections = new SortedListSet();

		[Serializable]
		public class StringQueue : Queue<string> { }
		[SerializeField]
		private StringQueue queue = new StringQueue();

		[Serializable]
		public class IntMonoSortedDictionary : SortedDictionary<int, MonoBehaviour> { }
		[SerializeField]
		private IntMonoSortedDictionary sortedDictionary = new IntMonoSortedDictionary();

		[Serializable]
		public class SortedStringList : SortedList<int, string> { }
		[SerializeField]
		private SortedStringList sortedList = new SortedStringList();

		[Serializable]
		public class StringSortedSet : SortedSet<string> { }
		[SerializeField]
		private StringSortedSet sortedSet = new StringSortedSet();

		[Serializable]
		public class IntStack : Stack<int> { }
		[SerializeField]
		private IntStack stack = new IntStack();
	}
}
