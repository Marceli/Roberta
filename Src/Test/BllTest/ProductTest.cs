using System.Linq;
using Core.Data;
using Core.Entities;
using NHibernate;
using NUnit.Framework;

namespace BllTest
{
    [TestFixture]
    public class ProductTest 
    {
		ISessionFactory factory;
		[TestFixtureSetUp]
		public void CreateFactory()
		{
			factory = new SessionProvider().Factory;
		}

		[TestFixtureTearDown]
		public void DisposeFactory()
		{
			factory.Dispose();
		}

        [Test]
        public void Basic()
        {
            var p = new Product();
            Assert.IsNotNull(p, "nie utworzy³");
        }
		[Test]
		public void Save()
		{

			var c = new Category {Name = "olo"};
			var p = new Product {Category = c};
			using (var session = factory.OpenSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					session.Save(c);
					session.Save(p);
					transaction.Commit();
				}
			}
			Product fromDb;
			using (var session = factory.OpenSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					fromDb = session.Get<Product>(p.Id);
					Assert.That(fromDb.Category.Id == c.Id);
				}
			}
		}


    	[Test]
        public void SaveWithPicture()
        {
    		Product p;
            
			using (var session = factory.OpenSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					p = new Product { Title = "Tytu³", Price = 90m, Description = "jakisopis" };
					session.Save(p); 
					var pic = new Picture();
					p.AddPicture(pic);
				
					transaction.Commit();
				}
			}
			using (var session = factory.OpenSession())
			{
				var fromDb=session.Get<Product>(p.Id);
			
				Assert.That(fromDb.Pictures.Count(),Is.EqualTo(1));
			}
        }
    }
}
