using System.Linq;
using System.Web;
using Core.Entities;
using NHibernate;
using NHibernate.Linq;

namespace Web
{
	public static class Db
	{
		public static ISession Session
		{
			get { return (ISession)HttpContext.Current.Items["dbSession"]; }
		}
		public static IOrderedQueryable<Product> Products
		{
			get { return Session.Linq<Product>(); }
		}
		public static IOrderedQueryable<Category> Category
		{
			get { return Session.Linq<Category>(); }
		}

	}
}