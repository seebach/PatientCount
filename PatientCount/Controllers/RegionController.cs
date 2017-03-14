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
    /// <summary>
    /// This is controller is used by the majority of Editor examples as it
    /// provides a nice rounded set of information for the client-side Editor
    /// Javascript library to show its capabilities.
    ///
    /// In the code here, note that the `StaffModel` is used as the model for
    /// the Editor, which automatically defines the database fields to be read.
    /// Additional instructions can be given for each field by creating a `Field`
    /// instance for it - many of the fields have validation methods applied here
    /// and the date field has a formatter to make it readable to users looking
    /// at the table!
    /// </summary>
    public class RegionController : ApiController
    {
        [Route("api/region")]
        [HttpGet]
        [HttpPost]
        // [Authorize] // incomment to force authentication
        public IHttpActionResult Region()
        {
            var request = HttpContext.Current.Request;
            string dbConnection = Properties.Settings.Default.dbConnection;
            
            //Initialize DB Connection
            using (var db = new Database("sqlserver", dbConnection))
            {
                // Name of the region datatable in sql
                var response = new Editor(db, "Regions")
                    .Model<RegionModel>()
                    .TryCatch(false)
                    .Field(new Field("Region")
                        .Validator(Validation.NotEmpty(new ValidationOpts { Message = "A region is required" }))
                    )                    
                    .Process(request)
                    .Data();

                return Json(response);
            }
        }

    }
}