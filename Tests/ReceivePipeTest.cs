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
            Entry entry = new Entry(1,segment, EventType.Data);

            //Before enqueing anything, message count and messageCountPer client must be 0
            Assert.AreEqual(pipe.MessageQueueCount(), 0);
            Assert.AreEqual(pipe.MessageCountForThisClient(entry.connectionId), 0);


            // enqueue for id:1 client
            pipe.Enqueue(entry);

            Assert.AreEqual(pipe.MessageQueueCount(), 1);
            Assert.AreEqual(pipe.MessageCountForThisClient(entry.connectionId), 1);

            Entry entry2 = new Entry(24, segment, EventType.Data);
            // enqueue for id:24 client
            pipe.Enqueue(entry2);

            Assert.AreEqual(pipe.MessageCountForThisClient(entry2.connectionId), 1);
            Assert.AreEqual(pipe.MessageQueueCount(), 2);

         

        }

        [TestMethod]
        public void TestDequeueWhenQueueIsEmpty()
        {
            pipe = new ReceivePipe(64);

            Assert.IsFalse(pipe.Dequeue());
        }


        [TestMethod]
        public void TestEnqueue100Message()
        {
            pipe = new ReceivePipe(64);
            Assert.AreEqual(pipe.MessageQueueCount(), 0);

            ArraySegment<byte> segment = Encoding.ASCII.GetBytes("Hello");
            Entry entry = new Entry(1, segment, EventType.Data);


            // enqueue 100 messages
            for (int i = 0; i < 100; i++)
            {
                pipe.Enqueue(entry);
            }

            //After enqueing 100 messages, message count and messageCount for this client must be 100
            Assert.AreEqual(100, pipe.MessageQueueCount());
            Assert.AreEqual(100, pipe.MessageCountForThisClient(entry.connectionId));
            

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

            // pool should have 0 objects in it at first
            Assert.AreEqual(0, pipe.PoolCount());
            pipe.Dequeue();

            // Peek should remove the Entry from the queue
            Assert.AreEqual(0, pipe.MessageQueueCount());

            //After dequeing, pool.count must be +1
            Assert.AreEqual(1, pipe.PoolCount());

        }
    }
}
