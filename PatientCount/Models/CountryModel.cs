using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PatientCount.Models
{
    public class CountryModel
    {
        public class Countries
        {
           // public int id { get; set; }

            public string Country { get; set; }

            public int RegionId { get; set; }

            public int Active { get; set; }

            public string ReportingInterval { get; set; }
        }

        public class Regions
        {
            public string Region { get; set; }
        }
    }
}