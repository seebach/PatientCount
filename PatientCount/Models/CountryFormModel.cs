using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PatientCount.Models
{
    public class CountryFormModel
    {       
        public class CountryFormProducts
        {
          //  public int id { get; set; }

            public int FormId { get; set; }

            public int CountryId { get; set; }

            public int ProductId { get; set; }
        }


       public class Countries
        {           
            public string Country { get; set; }
           
        }

      /*  public class Forms
        {
            public string Form { get; set; }
        }*/

       /* public class Products
        {
            public string Product { get; set; }
        }*/
    }
}