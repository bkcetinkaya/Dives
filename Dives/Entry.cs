using System;
using System.Collections.Generic;
using System.Text;

namespace Dives
{
    public struct Entry
    {
        public int connectionId;
        public ArraySegment<byte> data;
        public EventType eventType;

       public Entry(int connectionID, ArraySegment<byte> data, EventType eventType)
        {
           this.connectionId = connectionID;
           this.data = data;
           this.eventType = eventType;
        }
    }
}
