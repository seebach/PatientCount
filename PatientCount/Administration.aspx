<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Administration.aspx.cs" Inherits="PatientCount.Administration" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=edge"/>
    <title>NovoSeven&reg; Patient Estimation Model</title>

    <link href="css/bootstrap.min.css" rel="stylesheet" />
    <link href="css/PatientCount.css" rel="stylesheet" />

    <link rel="stylesheet" type="text/css" href="https://cdn.datatables.net/1.10.12/css/jquery.dataTables.min.css"/>
	<link rel="stylesheet" type="text/css" href="https://cdn.datatables.net/buttons/1.2.0/css/buttons.dataTables.min.css"/>
	<link rel="stylesheet" type="text/css" href="https://cdn.datatables.net/select/1.2.0/css/select.dataTables.min.css"/>

    <link rel="stylesheet" type="text/css" href="css/editor.dataTables.min.css"/>

     
     <script src="js/jquery-2.1.4.js"></script> 
    <script src="js/bootstrap.min.js"></script>

    <script type="text/javascript" src="https://cdn.datatables.net/1.10.12/js/jquery.dataTables.min.js"></script>	
	<script type="text/javascript" src="https://cdn.datatables.net/buttons/1.2.0/js/dataTables.buttons.min.js"></script>	
	<script type="text/javascript" src="https://cdn.datatables.net/select/1.2.0/js/dataTables.select.min.js"></script>	
	<script type="text/javascript" src="js/dataTables.editor.min.js"></script>
	

    <script>


var editor; // use a global for the submit and return data rendering in the examples
   
$(document).ready(function() {
    editor = new $.fn.dataTable.Editor({
        // This points to a .Net class       
        ajax: "/api/region",      
		table: "#example",
		fields: [ {
				label: "Region:",
				name: "Region"
			}
		]
	} );
   
	$('#example').DataTable( {
	    dom: "Bfrtip",
	    ajax: "/api/region",
	   // ajax: 'DataTest.txt',
		columns: [			
			{ data: "Region" }			
		],
		select: true,
		buttons: [
			{ extend: "create", editor: editor },
			{ extend: "edit",   editor: editor },
			{ extend: "remove", editor: editor }
		]
	} );
} );


$(document).ready(function () {
    editor = new $.fn.dataTable.Editor({
        // This points to a .Net class       
        ajax: "/api/productgroup",
        table: "#productgroup",
        fields: [{
            label: "Product Group:",
            name: "ProductGroup"
        }
        ]
    });

    $('#productgroup').DataTable({
        dom: "Bfrtip",
        ajax: "/api/productgroup",
        // ajax: 'DataTest.txt',
        columns: [
			{ data: "ProductGroup" }
        ],
        select: true,
        buttons: [
			{ extend: "create", editor: editor },
			{ extend: "edit", editor: editor },
			{ extend: "remove", editor: editor }
        ]
    });
});


//var editor; // use a global for the submit and return data rendering in the examples

$(document).ready(function () {
    editor = new $.fn.dataTable.Editor({
        ajax: "/api/country",
        table: "#countries",
        fields: [{
            label: "Country:",
            name: "Countries.Country"
        }, {
            label: "Region:",
            name: "Countries.RegionId",
            type: "select",
            placeholder: "Select a region"
        },
         {
             label: "Reporting Interval:",
             name: "Countries.ReportingInterval",
             type: "select",
             placeholder: "Reporting Interval",
             options: [
                   { label: "Monthly", value: "Monthly" },
                   { label: "Quarterly", value: "Quarterly" },
             ]

         },
       /*   {
              "label": "Products:",
              "name": "Products[].id",
              "type": "checkbox"
          },*/
         {
             label: "Is Active:",
             name: "Countries.Active",
             type: "checkbox",
             options: [
                 { label: "", value: 1 }
             ],
             separator: '|',
             unselectedValue: 0,
         }
        ]
    });

    $('#countries').DataTable({
        dom: "Bfrtip",
        ajax: {
            url: "/api/country",
            type: 'POST'
        },
        columns: [           
            { data: "Countries.Country" },
             { data: "Regions.Region" },            
             { data: "Countries.ReportingInterval" },
            // { data: "Products", render: "[, ].Product" },
              {
                  "class": "center",
                  "data": "Countries.Active",
                  "render": function (val, type, row) {
                      return val == 0 ? "" : "<span class='glyphicon glyphicon-ok' aria-hidden='true'></span>";
                  }
              }
        ],
        select: true,
        buttons: [
			{ extend: "create", editor: editor },
			{ extend: "edit", editor: editor },
			{ extend: "remove", editor: editor }
        ]
    });
});

//var editor; // use a global for the submit and return data rendering in the examples

$(document).ready(function () {
    editor = new $.fn.dataTable.Editor({
        ajax: "/api/user",
        table: "#Users",       
        fields: [
          
            {
            label: "User:",
            name: "UserName"
        }, {
            label: "Email:",
            name: "Email"
        },
         {
             "label": "Countries:",
             "name": "Countries[].id",
             "type": "checkbox"
         },
        {
            label: "Is User:",
            name: "IsUser",
            type: "checkbox",
            options: [               
                { label: "", value: 1 }
            ],
            separator: '|',
            unselectedValue: 0,            
        },       
        {
            label: "Is Admin:",
            name: "IsAdmin",
            type: "checkbox",
            options: [
                { label: "", value: 1 }
            ],
            separator: '|',
            unselectedValue: 0,
        }
        ]
    });

    $('#Users').DataTable({
        dom: "Bfrtip",
        ajax: {
            url: "/api/user",
            type: 'POST'
        },
        columns: [
			{ data: "UserName" },
			{ data: "Email" },
            { data: "Countries", render: "[, ].Country" },
			{
			    "class": "center",
			    "data": "IsUser",
			    "render": function (val, type, row) {
                            return val == 0 ? "" : "<span class='glyphicon glyphicon-ok' aria-hidden='true'></span>";
                        }
			},
			{
			    "class": "center",
			    "data": "IsAdmin",
			   "render": function (val, type, row) {
                            return val == 0 ? "" : "<span class='glyphicon glyphicon-ok' aria-hidden='true'></span>";
                        }
			}
        ],
        select: true,
        buttons: [
			{ extend: "create", editor: editor },
			{ extend: "edit", editor: editor },
			{ extend: "remove", editor: editor }
        ]
    });
});


        //var editor; // use a global for the submit and return data rendering in the examples

        $(document).ready(function () {
            editor = new $.fn.dataTable.Editor({
                ajax: "/api/product",
                table: "#products",
                fields: [{
                    label: "Product:",
                    name: "Products.Product"
                },
                 {
                     label: "Product Type:",
                     name: "Products.ProductType",
                     type: "select",
                     placeholder: "Product type",
                     options: [
                           { label: "On Demand", value: "On Demand" },
                           { label: "Prophylaxis", value: "Prophylaxis" },
                     ]
                    
                 },
                 {
                     label: "Product Group:",
                     name: "Products.ProductGroupId",
                     type: "select",
                     placeholder: "Select a product group"
                 },
                {
                    label: "Is Active:",
                    name: "Products.Active",
                    type: "checkbox",
                    options: [
                        { label: "", value: 1 }
                    ],
                    separator: '|',
                    unselectedValue: 0,
                }
                ]               
            });

            $('#products').DataTable({
                dom: "Bfrtip",
                ajax: {
                    url: "/api/product",
                    type: 'POST'
                },
                columns: [
                    { data: "Products.Product" },
                    {
                        data: "Products.ProductType"
                       /* "render": function (val, type, row) {
                            return val == 1 ? "On Demand" : "Prophylaxis";
                        }*/
                    },
                     { data: "ProductGroups.ProductGroup" },
                    {
                        "class": "center",
                        "data": "Products.Active",
                        "render": function (val, type, row) {
                            return val == 0 ? "" : "<span class='glyphicon glyphicon-ok' aria-hidden='true'></span>";
                        }
                    }
                ],
                select: true,
                buttons: [
                    { extend: "create", editor: editor },
                    { extend: "edit", editor: editor },
                    { extend: "remove", editor: editor }
                ]
            });
        });


        $(document).ready(function () {
            editor = new $.fn.dataTable.Editor({
                ajax: "/api/mappings",
                table: "#countryforms",
                fields: [ {
                    label: "Country:",
                    name: "mappings.CountryId",
                    type: "select",
                    placeholder: "Select a country"
                },              
                {
                    label: "Form:",
                    name: "mappings.FormId",
                    type: "select",
                    placeholder: "Select a form"
                },
                 {
                     "label": "Products:",
                     "name": "Products[].id",
                     "type": "checkbox"
                 }/*,
                 {
                     label: "Region:",
                     name: "Countries.RegionId",
                     type: "select",
                     placeholder: "Select a region"
                 }*/
                ]
            });

            $('#countryforms').DataTable({
                dom: "Bfrtip",
                ajax: {
                    url: "/api/mappings",
                    type: 'POST'
                },
                columns: [
                   { data: "countries.Country" },
                   { data: "forms.Form" },
                   { data: "Products", render: "[, ].ProductTypeName" }
                   // { data: "Regions.Region" }
                ],
                select: true,
                buttons: [
                    { extend: "create", editor: editor },
                    { extend: "edit", editor: editor },
                    { extend: "remove", editor: editor }
                ]
            });
        });

        
        

	</script>   
  

</head>
<body>

  
 
          
            <nav class="navbar navbar-default navbar-top" style="margin-bottom:2px">
                    <div class="container">
                        <div class="navbar-header">                         
                             <a class="navbar-brand" href="#">NovoSeven&reg; Estimation Model</a>
                         </div>                   
                        <div id="navbar" class="navbar-collapse collapse">
                            <ul class="nav navbar-nav">                             
                                <li ><a href="Index.html">Home</a></li>
                                <li ><a href="MonthlyInput.aspx">Input</a></li>
                                <li ><a href="Analytics/index.html">Reports</a></li>                                    
                            </ul>
                            <ul class="nav navbar-nav navbar-right">                               
                                <li class="active"><a href="#">Administration</a></li>                            
                            </ul>
                        </div>                  
                    </div>             
            </nav>

        <div class="container"> 
            
             
               
      
            
                  
            <section aria-labelledby="Manage Users">
                <div class="row">
			        <h1 class="page-header" id="usersSection">Manage Users</h1>
                    <p class="tableptext">Manage Users by updating entries in the form below. A user must have a valid username and email. Set the countries for which the users can input monthly reports.
                        The flag Is Admin gives the user access to the administration page and the flag Is User gives access to the monthly page. To disable a user remove both flags from the user.
                    </p>
                </div>
                <div class="row">
                    <table id="Users" class="stripe" cellspacing="0" width="100%">
				        <thead>
					        <tr>
                                <th>User</th>
						        <th>Email</th>
                                <th>Countries</th>
                                <th>User</th>
						        <th>Admin</th>                           							
					        </tr>
				        </thead>
                        <!--<tfoot>
					        <tr>
						        <th>Region</th>
					        </tr>
				        </tfoot>-->
				    </table>
                </div>
                </section>

            <section>
                <div class="row">
			        <h1 class="page-header" id="productsSection">Manage Products</h1>
                    <p class="tableptext">Manage prodcuts by updating entries in the form below. A pountry no longer used in input forms
                         should be disabled, which will remove the product from input forms and selections but keep historical values entered for the product</p>
                </div>
                <div class="row">
                    <table id="products" class="stripe" cellspacing="0" width="100%">
				        <thead>
					        <tr>
                                <th>Product</th>
                                <th>Product Type</th>
                                <th>Product Group</th>
						        <th>Active</th>                                                      							
					        </tr>
				        </thead>
                        <!--<tfoot>
					        <tr>
						        <th>Region</th>
					        </tr>
				        </tfoot>-->
				    </table>
                </div>
                </section>
           
           
            <section> 
                 <div class="row">
			         <h1 class="page-header" id="countryformSection">Manage Products per Country per Form</h1>
                     <p class="tableptext">
                         lore ipsum
                     </p>
                 </div>
                 <div class="row">
                    <table id="countryforms" class="stripe" cellspacing="0" width="100%">
				        <thead>
					        <tr>
                                <th>Country</th>
                                 <th>Form</th>	
                                 <th>Products</th>					                                  							
					        </tr>
				        </thead>
                        <!--<tfoot>
					        <tr>
						        <th>Region</th>
					        </tr>
				        </tfoot>-->
				    </table>
               </div>

                </section>

            <section>
                <div class="row">
			        <h1 class="page-header" id="productgroupSection">Manage Product Groups</h1>
                    <p class="tableptext"> Manage Product groups by updating entries in the form below. Product groups are used to group products and are used in aggregations in forms and reports. </p>
                </div>
                 <div class="row">
                    <table id="productgroup" class="stripe">
				        <thead>
					        <tr>
						        <th>Product Group</th>						
					        </tr>
				        </thead>
                       <!-- <tfoot>
					        <tr>
						        <th>Region</th>
					        </tr>
				        </tfoot>-->
				    </table>
                </div>
             </section>

            <section>
                <div class="row">
			        <h1 class="page-header" id="regionsSection">Manage Regions</h1>
                    <p class="tableptext"> Manage Regions by updating entries in the form below. Regions are used to group countries for easier administration and for reporting purposes. </p>
                </div>
                 <div class="row">
                    <table id="example" class="stripe">
				        <thead>
					        <tr>
						        <th>Region</th>						
					        </tr>
				        </thead>
                       <!-- <tfoot>
					        <tr>
						        <th>Region</th>
					        </tr>
				        </tfoot>-->
				    </table>
                </div>
             </section>

            <section> 
                 <div class="row">
			         <h1 class="page-header" id="countriesSection">Manage Countries</h1>
                     <p class="tableptext"> Manage Countries by updating entries in the form below. A country can be linked to only one region. A country no longer used in input forms
                         should be disabled, which will remove the country from input forms and selections but keep historical values entered for the country. 
                     </p>
                 </div>
                 <div class="row">
                    <table id="countries" class="stripe" cellspacing="0" width="100%">
				        <thead>
					        <tr>
                                <th>Country</th>
						        <th>Region</th>
                                <th>Interval</th>                              
                                <th>Active</th>                            							
					        </tr>
				        </thead>
                        <!--<tfoot>
					        <tr>
						        <th>Region</th>
					        </tr>
				        </tfoot>-->
				    </table>
               </div>

                </section>
             <section> 
                 <div class="row col-xs-12">
             <p><br/><br/>
<b>Disclaimer </b><br/>
*Patient Estimation Model is owned by NovoSeven® global brand team. Always ensure to follow local laws and regulations for inputs and use of the model – contact your counterpart in your region or global team (for global focus countries) if you have any questions. 
                 <br /> </p>
             </div>
             </section>
        </div>
    
             

</body>
</html>
