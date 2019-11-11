using System;
using NUnit.Framework;

namespace SerializableCollections.Tests
{
	public class HashSetTests
	{
		[Serializable]
		public class IntSet : HashSet<int> { }

		private IntSet CreateTestHashSet()
		{
			var hashSet = new IntSet();
			for (var i = 0; i < 25; i++)
			{
				hashSet.Add(i);
			}
			return hashSet;
		}

		[Test]
		public void Add()
		{
			IntSet hashSet = CreateTestHashSet();
			Assert.Contains(1, hashSet);
		}

		[Test]
		public void Clear()
		{
			IntSet hashSet = CreateTestHashSet();
			hashSet.Clear();
			Assert.Zero(hashSet.Count);
		}

		[Test]
		public void Count()
		{
			IntSet hashSet = CreateTestHashSet();
			Assert.AreEqual(hashSet.Count, 25);
		}

		[Test]
		public void Contains()
		{
			IntSet hashSet = CreateTestHashSet();
			Assert.IsTrue(hashSet.Contains(1));
		}

		[Test]
		public void Remove()
		{
			IntSet hashSet = CreateTestHashSet();
			hashSet.Remove(1);
			Assert.IsFalse(hashSet.Contains(1));
		}

		[Test]
		public void Enumeration_Works_Correctly()
		{
			IntSet hashSet = CreateTestHashSet();
			int count = 0;
			foreach (var item in hashSet)
				count++;

			Assert.True(count == 25);
		}
	}
}
