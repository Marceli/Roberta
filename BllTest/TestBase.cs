
using System.Transactions;
using NUnit.Framework;

namespace BllTest
{
    [TestFixture]
    public abstract class TestBase
    {
   
        protected TransactionScope ts;
        /// <summary>
        /// Wykonywany przed wykonaniem test�w w klasie testowej.
        /// Tworzy now� transakcj� rozproszon�.
        /// </summary>
        [TestFixtureSetUp]
        public virtual void TestFixtureSetUp()
        {
            ts = new TransactionScope();
        }
        /// <summary>
        /// Wykonywany po wykonaniem test�w w klasie testowej.
        /// Wycofuje transacj� roprzoszon� ��cznie ze wszystkimi transakcjami zatwierdzonymi przez testy.
        /// </summary>
        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            ts.Dispose();
        }
    }
}
