
using System.Transactions;
using NUnit.Framework;
using Vulcan.Stypendia.Bll;

namespace TestProject
{
    /// <summary>
    /// Klasa bazowa dla wszystkich test�w w tym projekcie. Wszystkie w klasy testowe w projekcie piowinny po niej dziedziczy�.
    /// </summary>
    public abstract class BaseFixture
    {
        protected TransactionScope ts;
        /// <summary>
        /// Wykonywany przed wykonaniem test�w w klasie testowej.
        /// Tworzy now� transakcj� rozproszon�.
        /// </summary>
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
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
        /// <summary>
        /// Musi zosta� przeci��ona. Test powinien sprawdzi� zapis obiektu do bazy danych
        /// </summary>
        [Test]
        public abstract void Save();
        
        
        
        /// <summary>
        /// Musi zosta� przeci��ona. Test powinien sprawdzi� czy mo�na utworzy� obiekt biznesowy w pami�ci.
        /// </summary>
        [Test]
        public abstract void Create();
        
        /// <summary>
        /// Test sprawdza czy mo�na utworzy� po��czenie z baz� danych.
        /// </summary>
        [Test]
        public void TestConnection()
        {
            Assert.IsNotNull(Dm.ObjectSpace, "Nie utworzy� transakcji mo�e connection string jest z�y");
        }
    }
}
