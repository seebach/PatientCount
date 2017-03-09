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
    public class ProductController : ApiController
    {
        [Route("api/product")]
        [HttpGet]
        [HttpPost]
        public IHttpActionResult Join()
        {
            var request = HttpContext.Current.Request;
            string dbConnection = Properties.Settings.Default.dbConnection;

            using (var db = new Database("sqlserver", dbConnection))
            {
                var response = new Editor(db, "Products")
                    .Model<ProductModel>()
                    // Might need custom validation
                    .Field(new Field("Products.Product").Validator(Validation.NotEmpty(new ValidationOpts { Message = "A product is required" })))
                    .Field(new Field("Products.ProductType").Validator(Validation.NotEmpty(new ValidationOpts { Message = "A product type is required" })))
                    .Field(new Field("Products.ProductGroupId")
                        .Options("ProductGroups", "id", "ProductGroup")
                        .Validator(Validation.DbValues(new ValidationOpts { Empty = false }))
                    )
                    // left joint on regions table using the Regions.id as key and the model key Countries.RegionID
                    .LeftJoin("ProductGroups", "ProductGroups.id", "=", "Products.ProductGroupId")
                    .Process(request)
                    .Data();

                return Json(response);
            }
        }
    }
}