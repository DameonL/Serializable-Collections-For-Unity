using NUnit.Framework;
using System;

namespace SerializableCollections.Tests
{
	public class SortedSetTests
	{
		[Serializable]
		private class StringSortedSet : SortedSet<string> { }

		private StringSortedSet CreateTestSet()
		{
			var set = new StringSortedSet();
			for (var i = 0; i < 25; i++)
			{
				set.Add(('a' + i).ToString());
			}
			return set;
		}

		[Test]
		public void Add()
		{
			var set = new StringSortedSet();
			set.Add("asdf");
			Assert.Contains("asdf", set);
		}

		[Test]
		public void Clear()
		{
			var set = CreateTestSet();
			set.Clear();
			Assert.Zero(set.Count);
		}

		[Test]
		public void Contains()
		{
			var set = new StringSortedSet {	"qwerty" };
			Assert.IsTrue(set.Contains("qwerty"));
		}

		[Test]
		public void Count()
		{
			var set = new StringSortedSet { "a", "b", "c", "d", "e" };
			Assert.AreEqual(set.Count, 5);
		}

		[Test]
		public void Remove()
		{
			var set = new StringSortedSet { "qwerty" };
			set.Remove("qwerty");
			Assert.IsFalse(set.Contains("qwerty"));
		}

		[Test]
		public void Enumeration_Works_Correctly()
		{
			var set = CreateTestSet();
			int count = 0;
			foreach (var kvp in set)
			{
				count++;
			}
			Assert.True(count == 25);
		}
	}
}
