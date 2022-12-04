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
            Assert.AreEqual(pipe.MessageQueueCount, 0);

            ArraySegment<byte> segment = Encoding.ASCII.GetBytes("Hello");
            Entry entry = new Entry(0,segment, EventType.Data);

            pipe.Enqueue(entry);

            Assert.AreEqual(pipe.MessageQueueCount, 1);

            Assert.AreEqual(pipe.MessageCountForThisClient(entry.connectionId), 1);
            
        }
    }
}
