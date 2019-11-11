using NUnit.Framework;
using System;

namespace SerializableCollections.Tests
{
	public class SortedListTests
	{
		[Serializable]
		private class IntIntSortList : SortedList<int, int> { }

		private IntIntSortList CreateTestList()
		{
			var list = new IntIntSortList();
			for (var i = 0; i < 25; i++)
			{
				list.Add(i, i + 26);
			}
			return list;
		}

		[Test]
		public void Add()
		{
			var list = new IntIntSortList();
			list.Add(new System.Collections.Generic.KeyValuePair<int, int>(1, 100));
			list.Add(2, 200);

			Assert.AreEqual(list[0], 100);
			Assert.AreEqual(list[1], 200);
		}

		[Test]
		public void Clear()
		{
			var list = CreateTestList();
			list.Clear();
			Assert.Zero(list.Count);
		}

		[Test]
		public void ContainsKey()
		{
			var list = CreateTestList();
			Assert.IsTrue(list.ContainsKey(1));
		}

		[Test]
		public void ContainsValue()
		{
			var list = CreateTestList();
			Assert.IsTrue(list.ContainsValue(27));
		}

		[Test]
		public void Count()
		{
			var list = CreateTestList();
			Assert.AreEqual(list.Count, 25);
		}

		[Test]
		public void Contains()
		{
			var list = new IntIntSortList();
			System.Collections.Generic.KeyValuePair<int, int> value = new System.Collections.Generic.KeyValuePair<int, int>(1, 27);
			list.Add(value);
			Assert.IsTrue(list.Contains(value));
		}

		[Test]
		public void Remove()
		{
			var list = CreateTestList();
			list.Remove(1);
			Assert.IsFalse(list.ContainsKey(1));
		}

		[Test]
		public void Enumeration_Works_Correctly()
		{
			var list = CreateTestList();
			int count = 0;
			foreach (var kvp in list)
			{
				count++;
			}
			Assert.True(count == 25);
		}
	}
}
