
using System.Transactions;
using NUnit.Framework;
using Vulcan.Stypendia.Bll;

namespace TestProject
{
    /// <summary>
    /// Klasa bazowa dla wszystkich testów w tym projekcie. Wszystkie w klasy testowe w projekcie piowinny po niej dziedziczyæ.
    /// </summary>
    public abstract class BaseFixture
    {
        protected TransactionScope ts;
        /// <summary>
        /// Wykonywany przed wykonaniem testów w klasie testowej.
        /// Tworzy now¹ transakcjê rozproszon¹.
        /// </summary>
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
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
        /// <summary>
        /// Musi zostaæ przeci¹¿ona. Test powinien sprawdziæ zapis obiektu do bazy danych
        /// </summary>
        [Test]
        public abstract void Save();
        
        
        
        /// <summary>
        /// Musi zostaæ przeci¹¿ona. Test powinien sprawdziæ czy mo¿na utworzyæ obiekt biznesowy w pamiêci.
        /// </summary>
        [Test]
        public abstract void Create();
        
        /// <summary>
        /// Test sprawdza czy mo¿na utworzyæ po³¹czenie z baz¹ danych.
        /// </summary>
        [Test]
        public void TestConnection()
        {
            Assert.IsNotNull(Dm.ObjectSpace, "Nie utworzy³ transakcji mo¿e connection string jest z³y");
        }
    }
}
