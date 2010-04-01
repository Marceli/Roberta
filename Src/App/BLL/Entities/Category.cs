using System.Collections.Generic;


namespace Core.Entities
{
	public class Category 
	{
		private int id;
		private string name;
		private IList<Product> products=new List<Product>(); // Supports both ObjectSet and Lazy-Loaded ObjectList

		public virtual int Id
		{
			get { return id; }
			set{ id=value;}
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

		public virtual IList<Product> Products
		{
			get { return products; }
		}

		
	}
}