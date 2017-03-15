using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PatientCount
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        public User currentUser = new User(HttpContext.Current.User.Identity.Name.ToString());
        public string administrationLink;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (currentUser.IsAdmin == 1)
            {
                administrationLink = "<a href = \"administration.aspx\" > Administration </a>";
            }
        }
    }
}