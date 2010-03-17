using System.Collections.Generic;


namespace Bll
{
	public class Category 
	{
		private int id;
		private string name;
		private IList<Product> products; // Supports both ObjectSet and Lazy-Loaded ObjectList

		public int Id
		{
			get { return id; }
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public IList<Product> Products
		{
			get { return products; }
		}

		
	}
}