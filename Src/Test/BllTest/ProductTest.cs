using NUnit.Framework;
using Bll;

namespace BllTest
{
    [TestFixture]
    public class ProductTest :TestBase
    {
        [Test]
        public void Basic()
        {
            Product p = new Product();
            Assert.IsNotNull(p, "nie utworzy³");
        }
        [Test]
        public void Save()
        {
            Category c = new Category();
            c.Save(null);
            c.Name = "olo";
            Product p = new Product();
            p.Category = c;
            p.Save(null);
            Assert.IsTrue(p.Id>0, "nie zapisa³");
        }
        
        [Test]
        public void SaveWithPicture()
        {           
            Product p = new Product();
            p.Title = "Tytu³";
            p.Price = 90m;
            p.Description = "jakisopis";
            p.CategoryId = Dm.RetrieveAll<Category>()[0].Id;
            Picture pic = new Picture();
            
            p.Pictures.Add(pic);
            p.Save(null);
            int zapisanyId = p.Id;
            Dm.ObjectSpace.ClearTracking();
            Product fromDb = Dm.GetObject<Product>(zapisanyId);           
            Assert.AreEqual(1,fromDb.Pictures.Count);
        }
    }
}
