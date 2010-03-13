
using System.Collections.Generic;

namespace Bll
{
    
        public class Category:EntityBase
        {
            private int id;
            private string name;
            private IList<Product> products; // Supports both ObjectSet and Lazy-Loaded ObjectList
            public override void Save(Wilson.ORMapper.Transaction transacion)
            {
                base.Save(transacion,true);
            }

            public int Id
            {
                get { return this.id; }
            }

            public string Name
            {
                get { return this.name; }
                set { this.name = value; }
            }

            public IList<Product> Products
            {
                get { return this.products; }
            }   

        }
    }

