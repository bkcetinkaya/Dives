using System;
using System.Collections.Generic;
using System.Text;

namespace Dives
{
    public class ReceivePipe
    {
        private object locker = new object();

        Queue<Entry> messageQueue = new Queue<Entry>();

        Pool<byte[]> pool;

        Dictionary<int, int> messagePerClientDict = new Dictionary<int, int>();


        public ReceivePipe(int MaxMessageSize)
        {
            pool = new Pool<byte[]>(() => new byte[MaxMessageSize]);
        }


        public int MessageQueueCount()
        {
            lock (locker)
            {
                return messageQueue.Count;
            }
        }

        public int MessageCountForThisClient(int id)
        {
            lock (locker)
            {
                return messagePerClientDict.TryGetValue(id, out int count) ? count : 0;
            }
        }

        public void Enqueue(Entry entry)
        {
            lock (locker)
            {
                ArraySegment<byte> segment = default;

                if (entry.data != null)
                {
                    byte[] bytes = pool.Take();

                    Buffer.BlockCopy(entry.data.Array, 0, bytes, 0, entry.data.Count);

                    // if the actual message size is smaller than the MaxMessageSize, there will be whitespace at the end of the message. 
                    // To get rid of it, we simply create an array segment which is as large as the character count of actual message.
                    segment = new ArraySegment<byte>(bytes,0, entry.data.Count);

                }

                Entry e = new Entry(entry.connectionId, segment, EventType.Data);
                messageQueue.Enqueue(e);

                int oldCount = MessageCountForThisClient(entry.connectionId);
                messagePerClientDict[entry.connectionId] = oldCount + 1;
            }

            
        }

        public bool Peek(out Entry entry)
        {
            entry = default;

            lock (locker)
            {
                if(messageQueue.Count > 0)
                {
                    entry = messageQueue.Dequeue();
                    return true;
                }
                return false;
            }

        }
    }
}
