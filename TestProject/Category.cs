using System;
using System.Collections.Generic;
using System.Text;

namespace TestProject
{
    using System;
    using System.Collections;
    using Wilson.ORMapper;

    namespace CompanyName.BusinessObjects
    {
        public class Category
        {
            private int id;
            private string name;
            private IList productCategories; // Supports both ObjectSet and Lazy-Loaded ObjectList

            public int Id
            {
                get { return this.id; }
            }

            public string Name
            {
                get { return this.name; }
                set { this.name = value; }
            }

            public IList ProductCategories
            {
                get { return this.productCategories; }
            }

        }
    }
}
