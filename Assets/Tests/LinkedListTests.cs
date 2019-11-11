using System;
using NUnit.Framework;

namespace SerializableCollections.Tests
{
	public class LinkedListTests
	{
		[Serializable]
		public class StringLinkedList : LinkedList<string> { }

		private StringLinkedList CreateTestLinkedList()
		{
			var linkedList = new StringLinkedList();
			for (var i = 0; i < 25; i++)
			{
				linkedList.Add(new string((char)('a' + i), 1));
			}
			return linkedList;
		}

		[Test]
		public void Add()
		{
			var linkedList = CreateTestLinkedList();
			Assert.Contains("a", linkedList);
		}

		[Test]
		public void Clear()
		{
			var linkedList = CreateTestLinkedList();
			linkedList.Clear();
			Assert.Zero(linkedList.Count);
		}

		[Test]
		public void Count()
		{
			var linkedList = CreateTestLinkedList();
			Assert.AreEqual(linkedList.Count, 25);
		}

		[Test]
		public void Contains()
		{
			var linkedList = CreateTestLinkedList();
			Assert.IsTrue(linkedList.Contains("a"));
		}

		[Test]
		public void Remove()
		{
			var linkedList = CreateTestLinkedList();
			linkedList.Remove("a");
			Assert.IsFalse(linkedList.Contains("a"));
		}

		[Test]
		public void Enumeration_Works_Correctly()
		{
			var linkedList = CreateTestLinkedList();
			int count = 0;
			foreach (var item in linkedList)
				count++;

			Assert.True(count == 25);
		}
	}
}
