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
    public class UserController : ApiController
    {
        [Route("api/user")]
        [HttpGet]
        [HttpPost]
        public IHttpActionResult Join()
        {
            var request = HttpContext.Current.Request;
            string dbConnection = Properties.Settings.Default.dbConnection;

            using (var db = new Database("sqlserver", dbConnection))
            {
                var response = new Editor(db, "PCMUsers")
                    .Model<UserModel>()
                    // Might need custom validation
                    .Field(new Field("UserName").Validator(Validation.NotEmpty(new ValidationOpts { Message = "A user name is required" })))
                    .Field(new Field("Email").Validator(Validation.Email(new ValidationOpts { Message = "A valid email is required" })))
                     //.Field(new Field("IsUser"))
                     //.Field(new Field("IsAdmin"))
                     .MJoin(new MJoin("Countries")
                        .Link("PCMUsers.id", "UserCountry.UserId")
                        .Link("Countries.id", "UserCountry.CountryId")
                        .Model<UserCountryModel>()
                        .Order("Countries.Country")
                        .Field(new Field("id")
                            .Options("Countries", "id", "Country", q => q.Where("Active", "1", "="))
                        )
                    )
                    .Process(request)
                    .Data();

                return Json(response);
            }
        }
    }
}