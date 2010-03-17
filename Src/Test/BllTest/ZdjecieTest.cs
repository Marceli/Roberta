
using System.Transactions;
using NUnit.Framework;
using Bll;

namespace BllTest
{
    [TestFixture]
    public class ZdjecieTest
    {
    
        TransactionScope ts;
        [SetUp]
        public void Start()
        {
            ts = new TransactionScope();
        }
        [TearDown]
        public void End()
        {
            ts.Dispose();
        }
//        [Test]
//    public void SaveTest()
//        {
//            Category c = new Category();
//            c.Save(null);
//            Product p = new Product();
//            p.MyCategoryId = c.Id;
//            p.Save(null);
//            Picture z = new Picture();
//            z.ProductId = p.Id;
//            z.Url = "olo";
//            z.Save(null);
//            
//            
//            Assert.IsTrue(z.Id>1);
//        }
    }
}
