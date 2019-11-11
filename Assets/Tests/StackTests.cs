using NUnit.Framework;
using System;

namespace SerializableCollections.Tests
{
	public class StackTests
	{
		[Serializable]
		public class StringStack : Stack<string> { }

		private StringStack CreateTestStack()
		{
			var stack = new StringStack();
			for (var i = 0; i < 25; i++)
			{
				stack.Push(new string((char)('a' + i), 1));
			}

			return stack;
		}

		[Test]
		public void Add()
		{
			var stack = new StringStack();
			stack.Push("a");
			Assert.Contains("a", stack);
		}

		[Test]
		public void Clear()
		{
			var stack = new StringStack();
			stack.Push("a");
			stack.Clear();
			Assert.Zero(stack.Count);
		}

		[Test]
		public void Contains()
		{
			var stack = CreateTestStack();
			Assert.IsTrue(stack.Contains("a"));
		}

		[Test]
		public void Count()
		{
			var stack = new StringStack();
			stack.Push("a");
			stack.Push("b");
			stack.Push("c");
			stack.Push("d");
			stack.Push("e");
			Assert.AreEqual(stack.Count, 5);
		}

		[Test]
		public void IsLIFO()
		{
			var stack = new StringStack();
			stack.Push("a");
			stack.Push("b");
			stack.Push("c");
			string pop = stack.Pop();
			Assert.AreEqual("c", pop);
		}

		[Test]
		public void Enumeration_Works_Correctly()
		{
			var stack = CreateTestStack();
			int count = 0;
			foreach (var item in stack)
				count++;

			Assert.IsTrue(count == 25);
		}
	}
}
