using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PatientCount.Models
{
    public class ProductModel
    {
        public class Products
        {
            public string Product { get; set; }
            public string ProductType { get; set; }
            //formatted as checkbox
            public int Active { get; set; }
            public int ProductGroupId { get; set; }
        }

        public class ProductGroups
        {
            public string ProductGroup { get; set; }
        }
    }

}
