using System;
using System.Collections.Generic;
using System.Text;

namespace Dives
{
    public class SendPipe
    {
        private object locker = new object();

        Queue<ArraySegment<byte>> messageQueue = new Queue<ArraySegment<byte>>();

        Pool<byte[]> pool;

        public SendPipe(int MaxMessageSize)
        {
            pool = new Pool<byte[]>(() => new byte[MaxMessageSize]);
        }

        public void Enqueue(ArraySegment<byte> message)
        {
            lock (locker)
            {
                ArraySegment<byte> segment = default;

                byte[] bytes = pool.Take();

                Buffer.BlockCopy(message.Array, 0, bytes, 0, message.Count);

                segment = new ArraySegment<byte>(bytes, 0, message.Count);

                messageQueue.Enqueue(segment); 
            }
        }


        public bool DequeueAndSerializeAll(ref byte[] payload, out int packetSize)
        {
            lock (locker)
            {
               packetSize = 0;
               
                // return immediately if queue is empty.
                if(messageQueue.Count == 0)
                {
                    return false;
                }

                // 4 byte header + message
                foreach (ArraySegment<byte> message in messageQueue)
                {
                    packetSize += 4 + message.Count;
                }

                if(payload==null || payload.Length < packetSize)
                {
                    payload = new byte[packetSize];
                }

                int position = 0;
                while(messageQueue.Count > 0)
                {
                    ArraySegment<byte> message = messageQueue.Dequeue();

                    Utils.IntToBytesBigEndianNonAlloc(message.Count, ref payload, position);
                    position += 4;

                    Buffer.BlockCopy(message.Array, 0, payload, position, message.Count);
                    position += message.Count;

                    pool.Return(message.Array);
                }

                return true;
            }
        }

        public int GetMessageQueueCount()
        {
            lock (locker)
            {
                return messageQueue.Count;
            }
        }

        

        public int GetPoolCount()
        {
            lock (locker)
            {
                return pool.Count();
            }
        }
    }
}
