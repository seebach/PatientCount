using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PatientCount.Models
{
    public class CountryProduct
    {
        public int id { get; set; }

        public string Product { get; set; }
    }

    public class CountryProductX
    {
        public int id { get; set; }

        public int Active { get; set; }

        public string Product { get; set; }

        public string ProductTypeName { get; set; }
    }
}