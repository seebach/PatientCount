using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PatientCount.Models
{
    public class MappingsModel
    {
        public class mappings
        {
            // public int id { get; set; }
                      
            public int FormId { get; set; }

            public int CountryId { get; set; }

        }

        public class forms
        {
            public string Form { get; set; }
        }

        public class countries
        {
            public string Country { get; set; }
        }
    }
}