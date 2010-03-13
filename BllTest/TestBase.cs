
using System.Transactions;
using NUnit.Framework;

namespace BllTest
{
    [TestFixture]
    public abstract class TestBase
    {
   
        protected TransactionScope ts;
        /// <summary>
        /// Wykonywany przed wykonaniem testów w klasie testowej.
        /// Tworzy now¹ transakcjê rozproszon¹.
        /// </summary>
        [TestFixtureSetUp]
        public virtual void TestFixtureSetUp()
        {
            ts = new TransactionScope();
        }
        /// <summary>
        /// Wykonywany po wykonaniem testów w klasie testowej.
        /// Wycofuje transacjê roprzoszon¹ ³¹cznie ze wszystkimi transakcjami zatwierdzonymi przez testy.
        /// </summary>
        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            ts.Dispose();
        }
    }
}
