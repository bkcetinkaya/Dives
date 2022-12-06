using Dives;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests
{
    [TestClass]
    public class SendPipeTest
    {
        SendPipe pipe;

        [TestMethod]
        public void TestEnqueue()
        {
            pipe = new SendPipe(64);

            ArraySegment<byte> segment = default;
            byte[] bytes = Encoding.UTF8.GetBytes("Hello");
            segment = new ArraySegment<byte>(bytes);

            pipe.Enqueue(segment);

          
            Assert.AreEqual(pipe.GetMessageQueueCount(), 1);
        }

        [TestMethod]
        public void TestEnqueueThenDequeueSerializeAll()
        {
            pipe = new SendPipe(64);

            ArraySegment<byte> firstMessage = new ArraySegment<byte>(new byte[] { 0xAA }); // <----- first message is 1 byte long
            ArraySegment<byte> secondMessage = new ArraySegment<byte>(new byte[] { 0xAA, 0xBB}); // < ----- second message is 2 bytes long

            pipe.Enqueue(firstMessage);
            pipe.Enqueue(secondMessage);

            byte[] payload = null;
            bool result = pipe.DequeueAndSerializeAll(ref payload, out int packetSize); 
            Assert.IsTrue(result);

            Assert.AreEqual(4+firstMessage.Count + 4 + secondMessage.Count, packetSize);
            

            Assert.AreEqual(payload[0], 0x00);
            Assert.AreEqual(payload[1], 0x00);
            Assert.AreEqual(payload[2], 0x00);
            Assert.AreEqual(payload[3], 0x01);
            Assert.AreEqual(payload[4], 0xAA);

            Assert.AreEqual(payload[5], 0x00);
            Assert.AreEqual(payload[6], 0x00);
            Assert.AreEqual(payload[7], 0x00);
            Assert.AreEqual(payload[8], 0x02);
            Assert.AreEqual(payload[9], 0xAA);
            Assert.AreEqual(payload[10], 0xBB);

            //After serializing everything, queue must be empty
            Assert.AreEqual(0, pipe.GetMessageQueueCount());

        }

        [TestMethod]
        public void TestPoolCountAfterDequeue()
        {
            pipe = new SendPipe(64);

            ArraySegment<byte> firstMessage = new ArraySegment<byte>(new byte[] { 0xAA });
            pipe.Enqueue(firstMessage);

            Assert.AreEqual(0, pipe.GetPoolCount());
            byte[] payload = null;
            pipe.DequeueAndSerializeAll(ref payload, out int packetSize);

            Assert.AreEqual(1, pipe.GetPoolCount());
        }
    }
}
