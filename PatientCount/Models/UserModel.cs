using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PatientCount.Models
{
    public class UserModel
    {
        public int id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }
        //formatted as cehckbox
        public int IsUser { get; set; }
        //formatted as cehckbox
        public int IsAdmin { get; set; }
    }
}