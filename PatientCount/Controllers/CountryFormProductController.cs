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
    public class CountryFormProductController : ApiController
    {
        [Route("api/mappings")]
        [HttpGet]
        [HttpPost]
        // [Authorize] // incomment to force authentication
        public IHttpActionResult Join()
        {
            var request = HttpContext.Current.Request;
            string dbConnection = Properties.Settings.Default.dbConnection;
            //string dbConnection = "Server = WSSDSPCM01; Integrated security = SSPI; database = PCM";

            using (var db = new Database("sqlserver", dbConnection))
            {
                //var response = new Editor(db, "CountryFormProducts")
                    var response = new Editor(db, "mappings")
                    //var response = new Editor(db, "Countries")
                   // .Model<CountryFormModel>()
                    .Model<MappingsModel>()
                           //  .Model<CountryModel>()

                           // Create a options field for displaying possible regions. Lookup on country regionid field on Region table id field and format with the Region field
                           
                           .Field(new Field("mappings.CountryId")
                               .Options("Countries", "id", "Country")
                               .Validator(Validation.DbValues(new ValidationOpts { Empty = false }))
                               )
                                                        

                          .Field(new Field("mappings.FormId")
                               .Options("Forms", "id", "Form")
                               .Validator(Validation.DbValues(new ValidationOpts { Empty = false }))
                               )

                         


                    .MJoin(new MJoin("Products")                      
                        .Link("mappings.id", "productformcountry.FormCountryId") //ExcludeProductCountry.CountryId
                        .Link("Products.id", "productformcountry.ProductId")
                        .Model<CountryProductX>()
                        .Order("Products.Product")
                        .Field(new Field("id")
                            .Options("Products", "id", "ProductTypeName", q => q.Where("Active", "1", "="))
                        )
                         )

                  /*  .MJoin(new MJoin("Products")                      
                       .Link("Products.id", "CountryFormProducts.ProductId")
                       //.Model<CountryProduct>()
                       //.Order("Products.Product")
                       .Field(new Field("Products.id")
                           .Options("Products", "id", "Product")
                       )
                        )*/



                     .LeftJoin("Countries", "Countries.id", "=", "mappings.CountryId")
                     .LeftJoin("Forms", "Forms.id", "=", "mappings.FormId")                    
                    .Process(request)
                    .Data();                    



                return Json(response);
            }
        }
    }
}