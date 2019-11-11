using System;
using NUnit.Framework;

namespace SerializableCollections.Tests
{
	public class SortedDictionaryTests
	{
		[Serializable]
		private class IntIntSortDic : SortedDictionary<int, int> { }

		private IntIntSortDic CreateTestDictionary()
		{
			var dictionary = new IntIntSortDic();
			for (var i = 0; i < 25; i++)
			{
				dictionary.Add(i, i + 26);
			}
			return dictionary;
		}

		[Test]
		public void Add()
		{
			var dictionary = new IntIntSortDic();
			dictionary.Add(new System.Collections.Generic.KeyValuePair<int, int>(1, 100));
			dictionary.Add(2, 200);
			Assert.AreEqual(dictionary[1], 100);
			Assert.AreEqual(dictionary[2], 200);
		}

		[Test]
		public void Clear()
		{
			var dictionary = CreateTestDictionary();
			dictionary.Clear();
			Assert.Zero(dictionary.Count);
		}

		[Test]
		public void ContainsKey()
		{
			var dictionary = CreateTestDictionary();
			Assert.IsTrue(dictionary.ContainsKey(1));
		}

		[Test]
		public void ContainsValue()
		{
			var dictionary = CreateTestDictionary();
			Assert.IsTrue(dictionary.ContainsValue(27));
		}

		[Test]
		public void Count()
		{
			var dictionary = CreateTestDictionary();
			Assert.AreEqual(dictionary.Count, 25);
		}

		[Test]
		public void Contains()
		{
			var dictionary = new IntIntSortDic();
			System.Collections.Generic.KeyValuePair<int, int> value = new System.Collections.Generic.KeyValuePair<int, int>(1, 27);
			dictionary.Add(value);
			Assert.IsTrue(dictionary.Contains(value));
		}

		[Test]
		public void Remove()
		{
			var dictionary = CreateTestDictionary();
			dictionary.Remove(1);
			Assert.IsFalse(dictionary.ContainsKey(1));
		}

		[Test]
		public void Enumeration_Works_Correctly()
		{
			var dictionary = CreateTestDictionary();
			int count = 0;
			foreach (var kvp in dictionary)
			{
				count++;
			}
			Assert.True(count == 25);
		}
	}
}
