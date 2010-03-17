using NUnit.Framework;
using Bll;

namespace BllTest
{
    [TestFixture]
    public class CategoryTest : TestBase
    {
        [Test]
        public void Basic()
        {
            Category c = new Category();
            Assert.IsNotNull(c,"nie utworzy³ category");
        }
        [Test]
        public void Save()
        {
            Category c = new Category();
            c.Name = "olo";
            c.Save(null);
            int cid = c.Id;
            Dm.ObjectSpace.ClearTracking();
            Category fromDb = Dm.GetObject<Category>(cid);
            Dm.ObjectSpace.ClearTracking();
            Assert.IsTrue(fromDb.Id>0, "nie zapisa³ category");
        }
        [Test]
        public void SaveWithProduct()
        {
           // ts.Dispose();
            Category c = new Category();
            c.Name = "olo";
           
           
            Product p = new Product();
            p.Title = "olo";
            
            c.Products.Add(p);
            Product p2 = new Product();
            p2.Title = "olo2";

            c.Products.Add(p2);
            //p.CategoryId = c.Id;
            
            c.Save(null);
            Assert.IsTrue(c.Id > 0, "nie zapisa³ category");
            Assert.IsTrue(p.Id > 0, "nie zapisa³ produktu");
           
            int cid = c.Id;
            Dm.ObjectSpace.ClearTracking();
            Category fromDb = Dm.GetObject<Category>(cid);
            Assert.IsTrue(fromDb.Products[0].CategoryId == cid, "NIE ZAPISA£ PRODUKTÓW");
            Assert.IsTrue(fromDb.Products.Count==2,"NIE ZAPISA£ PRODUKTÓW");
        }
    }
}
