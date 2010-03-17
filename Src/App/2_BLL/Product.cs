using System.Collections.Generic;
using Wilson.ORMapper;
namespace Bll
{
        public class Product:EntityBase
        {
            private int id;
            private string title;
            private string description;
            private string image;
            private decimal price;
            private int categoryId;
            private IList<Picture> pictures;
            private ObjectHolder<Category> category; // Strongly Type as Category if not Lazy-Loading
            private string categoryName;
            public Category Category
            {
                get
                {
                    return this.category.InnerObject;
                }
                set 
                { 
                    this.category.InnerObject = value;
                    if(value!=null)
                    {
                        this.categoryId = value.Id;
                    }
                }
            }
            public override void Save(Transaction transacion)
            {
                
                base.Save(transacion,true);
            }
            public int Id
            {
                get { return this.id; }
            }


            public string Title
            {
                get { return this.title; }
                set { this.title = value; }
            }

            public string Description
            {
                get { return this.description; }
                set { this.description = value; }
            }
            public decimal Price
            {
                get { return this.price; }
                set { this.price = value; }
            }
            public int CategoryId
            {
                get { return this.categoryId; }
                set { this.categoryId = value; }
            }

            public IList<Picture> Pictures
            {
                get{ return pictures;}
                set{ pictures = value;}
            }
            public string Image
            {
                get { return image; }
                set { image = value; }
            }

            public string CategoryName
            {
                get { return categoryName; }
                set { categoryName = value; }
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

            public static ObjectSet<Product> GetByField(string fieldName, string sortExpresion
     , bool ascOrder, string filterValue, int pageSize, int pageIndex)
            {
                fieldName = "Product." + fieldName;
                QueryHelper helper = Dm.ObjectSpace.QueryHelper;

                string whereExpresion = string.Empty;
                if (!string.IsNullOrEmpty(filterValue))
                {
                    filterValue = filterValue[filterValue.Length - 1] != '%' ? filterValue + "%" : filterValue;
                    whereExpresion = helper.GetExpression(fieldName, filterValue, ComparisonOperators.Like);
                }
                sortExpresion = sortExpresion ?? "";
                if(sortExpresion.Length!=0)
                {
                    sortExpresion = string.Format("Product.{0}", sortExpresion);
                    sortExpresion = helper.GetFieldName(sortExpresion);
                    sortExpresion = string.Format("{0} {1}", sortExpresion, ascOrder ? "ASC" : "DESC");
                }
                ObjectQuery<Product> query = new ObjectQuery<Product>(whereExpresion, sortExpresion, pageSize, pageIndex);
                return Dm.RetrieveQuery<Product>(query);
            }


            
        }
    }


