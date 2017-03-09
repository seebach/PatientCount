using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;
using DataTables;
using PatientCount.Models;

namespace PatientCount.Controllers
{    
    public class ProductGroupController : ApiController
    {
        [Route("api/productgroup")]
        [HttpGet]
        [HttpPost]
        public IHttpActionResult Region()
        {
            var request = HttpContext.Current.Request;
            string dbConnection = Properties.Settings.Default.dbConnection;

            //Initialize DB Connection
            using (var db = new Database("sqlserver", dbConnection))
            {
                // Name of the region datatable in sql
                var response = new Editor(db, "ProductGroups")
                    .Model<ProductGroupModel>()
                    .TryCatch(false)
                    .Field(new Field("ProductGroup")
                        .Validator(Validation.NotEmpty(new ValidationOpts { Message = "A product group is required" }))
                    )
                    .Process(request)
                    .Data();

                return Json(response);
            }
        }

    }
}
