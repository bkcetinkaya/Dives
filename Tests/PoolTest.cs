using Dives;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class PoolTest
    {
        Pool<byte[]> pool;

        [TestMethod]
        public void TestCreation()
        {
            pool = new Pool<byte[]>( ()=> new byte[64]);

            
            Assert.IsTrue(pool.Count() == 0);
          
        }

        [TestMethod]
        public void TestTake()
        {
            pool = new Pool<byte[]>(() => new byte[64]);

            byte[] bytes = pool.Take();
            Assert.AreEqual(64, bytes.Length);

        }

        [TestMethod]
        public void TakeThenReturn()
        {
            pool = new Pool<byte[]>(() => new byte[64]);

            byte[] bytes = pool.Take();
            Assert.AreEqual(pool.Count(), 0);

            //After returning the bytes to the pool, now count must be 1
            pool.Return(bytes);
            Assert.AreEqual(pool.Count(), 1);

        }

    }
}
