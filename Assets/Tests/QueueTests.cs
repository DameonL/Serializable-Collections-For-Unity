using System;
using NUnit.Framework;

namespace SerializableCollections.Tests
{
	public class QueueTests
	{
		[Serializable]
		public class StringQueue : Queue<string> { }

		private StringQueue CreateTestQueue()
		{
			var queue = new StringQueue();
			for (var i = 0; i < 25; i++)
			{
				queue.Enqueue(new string((char)('a' + i), 1));
			}

			return queue;
		}

		[Test]
		public void Add()
		{
			var queue = new StringQueue();
			queue.Enqueue("a");
			Assert.Contains("a", queue);
		}

		[Test]
		public void Clear()
		{
			var queue = CreateTestQueue();
			queue.Clear();
			Assert.Zero(queue.Count);
		}

		[Test]
		public void Count()
		{
			var queue = CreateTestQueue();
			Assert.AreEqual(queue.Count, 25);
		}

		[Test]
		public void Contains()
		{
			var queue = CreateTestQueue();
			Assert.IsTrue(queue.Contains("a"));
		}

		[Test]
		public void Dequeue()
		{
			var queue = CreateTestQueue();
			string dequeue = queue.Dequeue();
			Assert.AreEqual("a", dequeue);
		}

		[Test]
		public void Enumeration_Works_Correctly()
		{
			var queue = CreateTestQueue();
			int count = 0;
			foreach (var item in queue)
				count++;

			Assert.IsTrue(count == 25);
		}
	}
}
