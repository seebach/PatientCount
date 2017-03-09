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
    public class CountryController : ApiController
    {
        [Route("api/country")]
        [HttpGet]
        [HttpPost]
        public IHttpActionResult Join()
        {
            var request = HttpContext.Current.Request;
            string dbConnection = Properties.Settings.Default.dbConnection;
            //string dbConnection = "Server = WSSDSPCM01; Integrated security = SSPI; database = PCM";

            using (var db = new Database("sqlserver", dbConnection))
            {
                var response = new Editor(db, "Countries")
                    .Model<CountryModel>()
                    // Create a options field for displaying possible regions. Lookup on country regionid field on Region table id field and format with teh Region field
                    .Field(new Field("Countries.Country")
                        .Validator(Validation.NotEmpty(new ValidationOpts { Message = "A country is required" }))
                    )
                    .Field(new Field("Countries.ReportingInterval")
                        .Validator(Validation.NotEmpty(new ValidationOpts { Message = "A reporting interval is required" }))
                    )
                    .Field(new Field("Countries.RegionId")
                        .Options("Regions", "id", "Region")
                        .Validator(Validation.DbValues(new ValidationOpts { Empty = false }))
                    )
                     .MJoin(new MJoin("Products")
                        .Link("Countries.id", "ExcludeProductCountry.CountryId")
                        .Link("Products.id", "ExcludeProductCountry.ProductId")
                        .Model<CountryProduct>()
                        .Order("Products.Product")
                        .Field(new Field("id")
                            .Options("Products", "id", "Product")
                        )
                    )
                    // left joint on regions table using the Regions.id as key and the model key Countries.RegionID
                    .LeftJoin("Regions", "Regions.id", "=", "Countries.RegionId")
                    .Process(request)
                    .Data();

                return Json(response);
            }
        }
    }
}