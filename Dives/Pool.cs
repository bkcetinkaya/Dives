using System;
using System.Collections.Generic;

namespace Dives
{
    public class Pool<T>
    {
        Stack<T> objects = new Stack<T>();

        Func<T> objectGenerator;

        public Pool(Func<T> objectGenerator)
        {
            
            this.objectGenerator = objectGenerator;
        }

        public T Take()
        {
            if(objects.Count > 0)
            {
                return objects.Pop();
            }
            else
            {
               return objectGenerator();
            }
        }

        public void Return(T item)
        {
            objects.Push(item);
        }

        public int Count()
        {
            return objects.Count;
        }
    }
}
