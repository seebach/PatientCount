﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using DataTables;

namespace PatientCount
{
    public class Global : System.Web.HttpApplication
    {
        

        protected void Application_Start(object sender, EventArgs e)
        {
            
            GlobalConfiguration.Configure(WebApiConfig.Register);

        }
        
    }
}