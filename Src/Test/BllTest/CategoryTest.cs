using System.Linq;
using BllTest;
using Core.Data;
using Core.Entities;
using NHibernate;
using NUnit.Framework;
using NHibernate.Linq;

namespace CoreTest
{
	[TestFixture]
	public class CategoryTest 
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
		public void ConfigurationTest()
		{
			using (var session = new SessionProvider().Create()) { }
		}


		[Test]
		public void CanReadAndSaveCategory()
		{
			int personId;
			var category = new Category { Name = "Category" };
			using (var session = factory.OpenSession())
			{
				using (var transaction = session.BeginTransaction())
				{


					session.Save(category);
					transaction.Commit();
				}
			}
			using (var session = factory.OpenSession())
			{
				using (session.BeginTransaction())
				{
					var categoryFromDb = (from c in ((IOrderedQueryable<Category>)session.Linq<Category>()) where c.Id == category.Id select c).First();
					Assert.That(categoryFromDb.Name=="Category");
				}
			}
		}

		[Test]
		public void CanCreateCategory()
		{
			var c = new Category();
			Assert.IsNotNull(c);
		}
		public void SaveWithProduct()
		{
			// ts.Dispose();
			var c = new Category { Name = "olo" };


			var p = new Product { Title = "olo" };

			c.Products.Add(p);
			var p2 = new Product { Title = "olo2" };
			Category fromDb;
			c.Products.Add(p2);
			//p.CategoryId = c.Id;
			using (var session = factory.OpenSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					session.Save(c);
					transaction.Commit();
				}
			}
			using (var session = factory.OpenSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					fromDb = session.Get<Category>(c.Id);
				}
				Assert.That(fromDb.Products.Count == 2);
			}
		}
	}
}