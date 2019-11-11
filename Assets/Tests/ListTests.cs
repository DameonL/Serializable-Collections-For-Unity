using System;
using NUnit.Framework;

namespace SerializableCollections.Tests
{
	public class ListTests
	{
		[Serializable]
		public class IntList : List<int> { }

		private IntList CreateTestList()
		{
			var list = new IntList();
			for (var i = 0; i < 25; i++)
			{
				list.Add(i);
			}

			return list;
		}

		[Test]
		public void Add()
		{
			var list = CreateTestList();
			Assert.Contains(1, list);
		}

		[Test]
		public void Clear()
		{
			var list = CreateTestList();
			list.Clear();
			Assert.Zero(list.Count);
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
			var list = CreateTestList();
			Assert.IsTrue(list.Contains(1));
		}

		[Test]
		public void Remove()
		{
			var list = CreateTestList();
			list.Remove(1);
			Assert.IsFalse(list.Contains(1));
		}

		[Test]
		public void Enumeration_Works_Correctly()
		{
			var list = CreateTestList();
			int count = 0;
			foreach (var item in list)
				count++;

			Assert.True(count == 25);
		}
	}
}
