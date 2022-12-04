using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dives;
using System;
using System.Text;

namespace Tests
{
    [TestClass]
    public class ReceivePipeTest
    {
        ReceivePipe pipe;

        [TestMethod]
        public void TestEnqueue()
        {
            pipe = new ReceivePipe(64);
            Assert.AreEqual(pipe.MessageQueueCount(), 0);

            ArraySegment<byte> segment = Encoding.ASCII.GetBytes("Hello");
            Entry entry = new Entry(0,segment, EventType.Data);

            pipe.Enqueue(entry);

            Assert.AreEqual(pipe.MessageQueueCount(), 1);

            Assert.AreEqual(pipe.MessageCountForThisClient(entry.connectionId), 1);
            
        }

        [TestMethod]
        public void EnqueueThenPeek()
        {
            pipe = new ReceivePipe(64);
            ArraySegment<byte> segment = Encoding.UTF8.GetBytes("Hello");
            Entry entry = new Entry(0, segment, EventType.Data);

            pipe.Enqueue(entry);
            Assert.AreEqual(1, pipe.MessageQueueCount());


            Assert.IsTrue(pipe.Peek(out Entry e));
            Assert.AreEqual("Hello",Encoding.UTF8.GetString(e.data));

            // Peek should not remove the Entry from the queue
            Assert.AreEqual(1, pipe.MessageQueueCount());
            
        }

        [TestMethod]
        public void EnqueueThenDequeue()
        {
            pipe = new ReceivePipe(64);
            ArraySegment<byte> segment = Encoding.UTF8.GetBytes("Hello");
            Entry entry = new Entry(0, segment, EventType.Data);

            pipe.Enqueue(entry);

            Assert.AreEqual(1, pipe.MessageCountForThisClient(entry.connectionId));

            pipe.Dequeue();

            // Peek should remove the Entry from the queue
            Assert.AreEqual(0, pipe.MessageQueueCount());

        }
    }
}
