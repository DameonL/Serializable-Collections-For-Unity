using System;
using NUnit.Framework;

namespace SerializableCollections.Tests
{
    public class DictionaryTests
    {	
		[Serializable]
		public class StringIntDictionary : Dictionary<string, int> { }

		private StringIntDictionary CreateTestDictionary()
		{
			var dictionary = new StringIntDictionary();
			for (var i = 0; i < 25; i++)
			{
				string key = new string((char)('a' + i), 1);
				dictionary.Add(key, i);
			}
			return dictionary;
		}

		[Test]
		public void Add()
		{
			var dictionary = new StringIntDictionary();
			dictionary.Add(new System.Collections.Generic.KeyValuePair<string, int>("a", 1));
			dictionary.Add("b", 2);
			Assert.AreEqual(dictionary["a"], 1);
			Assert.AreEqual(dictionary["b"], 2);
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
			Assert.IsTrue(dictionary.ContainsKey("a"));
		}

		[Test]
		public void ContainsValue()
		{
			var dictionary = CreateTestDictionary();
			Assert.IsTrue(dictionary.ContainsValue(1));
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
			var dictionary = CreateTestDictionary();
			System.Collections.Generic.KeyValuePair<string, int> value = new System.Collections.Generic.KeyValuePair<string, int>("asdf", 5);
			dictionary.Add(value);
			Assert.Contains(value, dictionary);
		}

		[Test]
		public void Remove()
		{
			var dictionary = CreateTestDictionary();
			dictionary.Remove("a");
			Assert.IsFalse(dictionary.ContainsKey("a"));
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
