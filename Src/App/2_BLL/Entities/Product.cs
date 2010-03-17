using System.Collections.Generic;

namespace Core.Entities
{
	public class Product
	{
		private Category category; // Strongly Type as Category if not Lazy-Loading
		private string description;
		private int id;
		private string image;
		private IList<Picture> pictures = new List<Picture>();
		private decimal price;
		private string title;

		public virtual Category Category
		{
			get { return category; }
			set { category = value; }
		}

		public virtual int Id
		{
			get { return id; }
			set{ id = value;}
		}


		public virtual string Title
		{
			get { return title; }
			set { title = value; }
		}

		public virtual string Description
		{
			get { return description; }
			set { description = value; }
		}

		public virtual decimal Price
		{
			get { return price; }
			set { price = value; }
		}

		public virtual IEnumerable<Picture> Pictures
		{
			get { return pictures; }
			set { pictures=(IList<Picture>)value; }
			
		}
		public virtual void AddPicture(Picture picture)
		{
			picture.Product = this;
			this.pictures.Add(picture);
		
		}

		public virtual string Image
		{
			get { return image; }
			set { image = value; }
		}


//            public string Name
//            {
//                get { return this.name; }
//            }

//            public string Image
//            {
//                get { return this.image; }
//                set { this.image = value; }
//            }
		// Return the primary key property from the primary key object

//            public static ObjectSet<Product> GetByField(string fieldName, string sortExpresion
//     , bool ascOrder, string filterValue, int pageSize, int pageIndex)
//            {
//                fieldName = "Product." + fieldName;
//                QueryHelper helper = Dm.ObjectSpace.QueryHelper;
//
//                string whereExpresion = string.Empty;
//                if (!string.IsNullOrEmpty(filterValue))
//                {
//                    filterValue = filterValue[filterValue.Length - 1] != '%' ? filterValue + "%" : filterValue;
//                    whereExpresion = helper.GetExpression(fieldName, filterValue, ComparisonOperators.Like);
//                }
//                sortExpresion = sortExpresion ?? "";
//                if(sortExpresion.Length!=0)
//                {
//                    sortExpresion = string.Format("Product.{0}", sortExpresion);
//                    sortExpresion = helper.GetFieldName(sortExpresion);
//                    sortExpresion = string.Format("{0} {1}", sortExpresion, ascOrder ? "ASC" : "DESC");
//                }
//                ObjectQuery<Product> query = new ObjectQuery<Product>(whereExpresion, sortExpresion, pageSize, pageIndex);
//                return Dm.RetrieveQuery<Product>(query);
//            }
	}
}