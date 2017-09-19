using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTables;
using PatientCount;


namespace PatientCount
{
    public partial class  Administration : System.Web.UI.Page
    {
        public User currentUser = new User(HttpContext.Current.User.Identity.Name.ToString());

        protected void Page_Load(object sender, EventArgs e)
        {
        
     /*       if (currentUser.IsAdmin != 1)
            {
                // if user does not have access send him to the index page
                Response.Redirect("index.aspx?message=forbidden+you%27re+not+allowed+access+to+this+administration+page");
            }
            */
        }

    }
}