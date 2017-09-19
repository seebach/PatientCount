using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Collections;
using System.Globalization;


namespace PatientCount
{

    public partial class MonthlyInput : System.Web.UI.Page
    {
        // The number of columns before the matrix of the form begins (entering data)
        private const int hiddenColOffset = 2;
        private const int colOffset = 5;
        private const int rowOffset = 3;
        // Reference to lost, losttotrial etc
        private const int extraColProducts = 3;
        // Reference to PUP and previously tolerized
        public const int extraRowProducts = 2;

        int numberOfProduct = 0;
        string dbConnection = string.Empty;
        public string administrationLink;

        public User currentUser = new User(HttpContext.Current.User.Identity.Name.ToString());
        
        
        protected void Page_Load(object sender, EventArgs e)
        {

            if (currentUser.IsAdmin == 1) {
                administrationLink = "<a href = \"administration.aspx\" > Administration </a>";
            }
            if (currentUser.IsUser != 1)
            {
                // if user does not have access send him to the index page
                Response.Redirect("index.aspx?message=forbidden+you%27re+not+allowed+access+to+this+input+page");
            }
            // Create dataconnection
            dbConnection = Properties.Settings.Default.dbConnection;

            if (!Page.IsPostBack)
            {
                formArea.Visible = false;

                // Make sure to remove all cached instances if page is not a postback
                Cache.Remove("cols");
                Cache.Remove("Acquired Haemophilia");
                Cache.Remove("Surgery CH");
                Cache.Remove("Factor VII Deficiency"); 
                Cache.Remove("Glanzmann's thrombasthenia");
                Cache.Remove("CH Age split");
                Cache.Remove("Patients");

                // Load countries in countries dropdown
                LoadCountries(dbConnection);
                // Populate period table with data

                DateTime dateOffset = new DateTime(2009, 12, 1);
                DateTime dateNow = DateTime.Now;

               // for (int i = 0; i >= -((12*7)+3); i--)
               for (int i = 0; dateNow.AddMonths(i) >= dateOffset; i--)
               {
                   // string period = DateTime.Now.AddMonths(i).ToString("MMM-yyyy");
                   // string periodData = DateTime.Now.AddMonths(i).ToString("yyyyMM");

                    string period = dateNow.AddMonths(i).ToString("MMM-yyyy", CultureInfo.InvariantCulture);
                    string periodData = dateNow.AddMonths(i).ToString("yyyyMM");

                    periodsDdl.Items.Add(new ListItem(period, periodData));
                }                          

            }

            // Only run this if savebutton is clicked
            if (Page.IsPostBack)
            {
                restore_columns();
                restore_columns("Acquired Haemophilia", AHGrid);
                restore_columns("Surgery CH", SurgeryGrid);
                restore_columns("Factor VII Deficiency", FactorGrid);
                restore_columns("Glanzmann's thrombasthenia", thrombosisGrid);
                restore_columns("CH Age split", AgeSplitGrid);
                restore_columns("Patients", PatientsGrid);

                if (ViewState["productCount"] != null)
                {
                    numberOfProduct = (int)ViewState["productCount"];
                }
              
            }
        }

        protected void MontlyInputGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Might need a special clause for merging and setting as read only

            if (e.Row.RowType == DataControlRowType.DataRow )
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    for (int i = 0; i < e.Row.Cells.Count; i++)
                    {
                        TableCell cell = e.Row.Cells[i];
                        string cellValue = string.Empty;
                        if (cell.HasControls())
                        {
                            TextBox tb = (TextBox)cell.Controls[0];
                            cellValue = tb.Text;
                        }
                        else
                        {
                            cellValue = cell.Text;
                        }


                        if (cell.Visible)
                        {
                            int colSpanValue = 1;

                            for (int j = i + 1; j < e.Row.Cells.Count; j++)
                            {
                                TableCell otherCell = e.Row.Cells[j];

                                string otherValue = string.Empty;
                                if (otherCell.HasControls())
                                {
                                    TextBox tb = (TextBox)otherCell.Controls[0];
                                    otherValue = tb.Text;
                                }
                                else
                                {
                                    otherValue = otherCell.Text;
                                }
                               
                                if (otherValue == cellValue && cellValue != string.Empty && cellValue != "&nbsp;" && !Extension.IsNumeric(cellValue))
                                {
                                    colSpanValue++;
                                    otherCell.Visible = false;
                                }
                                else
                                {
                                    break;
                                }
                            }

                            if (colSpanValue > 1)
                            {
                                cell.ColumnSpan = colSpanValue;
                                cell.HorizontalAlign = HorizontalAlign.Center;
                            }
                        }
                    }
                }
            }

        }

        protected void MontlyInputGrid_PreRender(object sender, EventArgs e)
        {
            if (MontlyInputGrid.Rows.Count > 0)
            {
                //This replaces <td> with <th> and adds the scope attribute
                MontlyInputGrid.UseAccessibleHeader = true;

                //This will add the <thead> and <tbody> elements
                MontlyInputGrid.HeaderRow.TableSection = TableRowSection.TableHeader;

                //This adds the <tfoot> element. 
                //Remove if you don't have a footer row
                // MontlyInputGrid.FooterRow.TableSection = TableRowSection.TableFooter;                

                for (int rowIndex = MontlyInputGrid.Rows.Count - 2; rowIndex >= 0; rowIndex--)
                {
                    GridViewRow row = MontlyInputGrid.Rows[rowIndex];
                    GridViewRow previousRow = MontlyInputGrid.Rows[rowIndex + 1];

                    for (int i = 0; i < row.Cells.Count; i++)
                    {
                        string cellValue = string.Empty;
                        string previousRowValue = string.Empty;

                        if (row.Cells[i].HasControls())
                        {
                            TextBox tb = (TextBox)row.Cells[i].Controls[0];
                            cellValue = tb.Text;
                        }
                        else
                        {
                            cellValue = row.Cells[i].Text;
                        }

                        if (previousRow.Cells[i].HasControls())
                        {
                            TextBox tb = (TextBox)previousRow.Cells[i].Controls[0];
                            previousRowValue = tb.Text;
                        }
                        else
                        {
                            previousRowValue = previousRow.Cells[i].Text;
                        }

                        // Might include html formatting to replace &nbsp; reference
                        if (cellValue == previousRowValue && !string.IsNullOrEmpty(cellValue) && cellValue != "&nbsp;" && !Extension.IsNumeric(cellValue))
                       // if (row.Cells[i].Text == previousRow.Cells[i].Text && !string.IsNullOrEmpty(row.Cells[i].Text.Trim()) && row.Cells[i].Text != "&nbsp;" && !Extension.IsNumeric(row.Cells[i].Text))
                        {
                            row.Cells[i].RowSpan = previousRow.Cells[i].RowSpan < 2 ? 2 : previousRow.Cells[i].RowSpan + 1;
                            previousRow.Cells[i].Visible = false;
                        }
                    }
                }


                int cellCount = MontlyInputGrid.Rows[0].Cells.Count;

                for (int i = 0; i < MontlyInputGrid.Rows.Count; i++)
                {
                    GridViewRow cr = MontlyInputGrid.Rows[i];

                    for (int j = hiddenColOffset; j < cellCount; j++)
                    {
                        //int k = j - hiddenColOffset;

                        if (i == 0)
                        {
                            if (j == 0 + hiddenColOffset)
                            {
                                cr.Cells[j].CssClass = "d0";
                            }
                            else
                            {
                                cr.Cells[j].CssClass = "d2";
                            }
                        }

                        if (i == 1)
                        {
                            if (j == 0 + hiddenColOffset)
                            {
                                cr.Cells[j].CssClass = "d2";
                            }
                            // Change from here
                            else if (j < cellCount - 2)
                            {
                                cr.Cells[j].CssClass = "d3";
                            }
                            else
                            {
                                cr.Cells[j].CssClass = "staProduct";
                            }
                        }

                        if (i == 2)
                        {
                            if (j == 3 + hiddenColOffset || j == 4 + hiddenColOffset)
                            {
                                cr.Cells[j].CssClass = "staProduct";
                            }
                            else
                            {
                                cr.Cells[j].CssClass = "staProduct";
                            }

                        }

                        if (i > 2 && i < numberOfProduct + rowOffset)
                        {
                            if (j == 0 + hiddenColOffset)
                            {
                                cr.Cells[j].CssClass = "d3";
                            }
                            // Here
                            else if (j + rowOffset == i + colOffset)
                            // else if (j - colOffset - hiddenColOffset == i - rowOffset)
                            {
                                // Indication of red cell in matrix
                                cr.Cells[j].CssClass = "d1";
                                if (cr.Cells[j].HasControls())
                                {
                                    TextBox tx = (TextBox)cr.Cells[j].Controls[0];
                                    // IS hidden to disable control when tabbing
                                    tx.Visible = false;
                                }
                            }
                            // need this reference to identify startvalues in jquery
                            else if ((j == 0 + hiddenColOffset + 2))
                            {
                                cr.Cells[j].CssClass = "startCol staProduct";
                            }
                            else if ((j != 0 + hiddenColOffset && j < colOffset))
                            {
                                cr.Cells[j].CssClass = "staProduct";
                            }
                            else if (j < cellCount - 2)
                            {
                                cr.Cells[j].CssClass = "flowvalue";
                            }
                            else
                            {
                                cr.Cells[j].CssClass = "sumvalue";
                            }

                        }

                        if (i >= numberOfProduct + rowOffset && i < numberOfProduct + rowOffset + extraRowProducts)
                        {
                            if (j <= 2 + hiddenColOffset)
                            {
                                cr.Cells[j].CssClass = "d3";
                            }
                            else if (j > 2 + hiddenColOffset && j < numberOfProduct + colOffset)
                            {
                                cr.Cells[j].CssClass = "flowvalue";
                            }
                            else if (j >= numberOfProduct + colOffset && j < cellCount - 2)
                            {
                                cr.Cells[j].CssClass = "d1";
                                if (cr.Cells[j].HasControls())
                                {
                                    TextBox tx = (TextBox)cr.Cells[j].Controls[0];
                                    // IS hidden to disable control when tabbing
                                    tx.Visible = false;
                                }
                            }
                            else
                            {
                                cr.Cells[j].CssClass = "sumvalue";
                            }
                        }
                        if (i >= numberOfProduct + rowOffset + extraRowProducts)
                        {
                            cr.Cells[j].CssClass = "sumvalue";
                        }
                    }
                }
            }

        }

        private DataTable Transpose(DataTable dt)
        {
            DataTable dtNew = new DataTable();

            //adding columns    
            for (int i = 0; i <= dt.Rows.Count; i++)
            {
                dtNew.Columns.Add(i.ToString());
            }
            //Changing Column Captions: 
            dtNew.Columns[0].ColumnName = " ";

            //Adding Row Data
            for (int k = 1; k < dt.Columns.Count; k++)
            {
                DataRow r = dtNew.NewRow();
                r[0] = dt.Columns[k].ToString();
                for (int j = 1; j <= dt.Rows.Count; j++)
                    r[j] = dt.Rows[j - 1][k];
                dtNew.Rows.Add(r);
            }

            return dtNew;
        }

        //private DataTable TransposeProductTable(DataTable dt, string idKey)
        //{
        //    DataTable dtNew = new DataTable();

        //    //adding columns    
        //    for (int i = 0; i < dt.Rows.Count; i++)
        //    {
        //        dtNew.Columns.Add(dt.Rows[i][idKey].ToString());
        //    }
        //    //Changing Column Captions: 
        //    // dtNew.Columns[0].ColumnName = " ";

        //    //Adding Row Data
        //    for (int k = 1; k < dt.Columns.Count; k++)
        //    {
        //        DataRow r = dtNew.NewRow();
        //        //r[0] = dt.Columns[k].ToString();
        //        for (int j = 0; j < dt.Rows.Count; j++)
        //            r[j] = dt.Rows[j][k];
        //        dtNew.Rows.Add(r);
        //    }

        //    return dtNew;
        //}

        private void LoadCountries(string connectionString)
        {

            DataTable subjects = new DataTable();
           

            using (SqlConnection con = new SqlConnection(connectionString))
            {   
                
                try
                {
                    SqlDataAdapter adapter = new SqlDataAdapter("SELECT  c.[id],c.[Country] FROM [PCM].[dbo].[Countries] c JOIN[PCM].[dbo].UserCountry uc on uc.CountryId = c.id JOIN[PCM].[dbo].PCMUsers u on u.id = uc.UserId Where Active = 1 AND Upper(u.UserName) = Upper(@UserName) Order by Country", con);
                    adapter.SelectCommand.Parameters.AddWithValue("@UserName", currentUser.UserName);
                    adapter.Fill(subjects);

//                    UserName = "test";
                    countriesDdl.DataSource = subjects;
                    countriesDdl.DataTextField = "Country";
                    countriesDdl.DataValueField = "id";
                    countriesDdl.DataBind();
                }
                catch (Exception ex)
                {
                    // Handle the error
                }

                countriesDdl.Items.Insert(0, new ListItem("Select Country", "NA"));


            }

            // Add the initial item - you can add this even if the options from the
            // db were not successfully loaded
            // ddlSubject.Items.Insert(0, new ListItem("<Select Subject>", "0"));
        }

        protected void selectBtn_Click(object sender, EventArgs e)
        {

            if (countriesDdl.SelectedValue == "NA")
            {
                inputLabel.Text = "You must select a country in the countries dropdown";
                formArea.Visible = false;

            }
            else
            {
                // Reset form and remove all columns except the first 3
                if (MontlyInputGrid.Rows.Count > 0)
                {
                    // Reset            
                    MontlyInputGrid.DataSource = null;
                    MontlyInputGrid.DataBind();
                    // Clear All columns except the manually bound columns
                    for (int i = MontlyInputGrid.Columns.Count - 1; i >= colOffset; i--)
                    {
                        MontlyInputGrid.Columns.RemoveAt(i);
                    }
                }

                //Update input label
                inputLabel.Text = "Monthly Input for " + countriesDdl.SelectedItem.Text + " " + periodsDdl.SelectedItem.Text;

                formArea.Visible = true;
                // must make sure something is selected

                // We might need to store values in viewstate
                ViewState.Add("period", periodsDdl.SelectedValue);
                ViewState.Add("country", int.Parse(countriesDdl.SelectedValue));

                // Load Comments

                string sql = string.Format("SELECT Comment FROM[PCM].[dbo].[Comments] WHERE [CountryId] = '{0}' and [Period] = '{1}'", countriesDdl.SelectedValue, periodsDdl.SelectedValue);
                string comments = string.Empty;

                using (SqlConnection con = new SqlConnection(dbConnection))
                {
                    SqlCommand cmd = new SqlCommand(sql, con);
                    con.Open();
                    comments = (string)cmd.ExecuteScalar();
                    con.Close();
                }

                if (!string.IsNullOrEmpty(comments))
                {
                    txtComments.Text = comments;
                }
                else
                {
                    txtComments.Text = string.Empty;
                }


                //Clear cached columnrow
                Cache.Remove("cols");
                populateForm(countriesDdl.SelectedValue, periodsDdl.SelectedValue);

                DataTable dataSet = new DataTable();
                string sqlquery = string.Format(Queries.getFormData, countriesDdl.SelectedValue, periodsDdl.SelectedValue, "2", periodsDdl.SelectedValue.Substring(0, 4));

                using (SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlquery, dbConnection))
                {
                    // fill the DataSet using our DataAdapter 
                    dataAdapter.Fill(dataSet);
                }

                // Reset form and remove all columns except the first 3
                if (AHGrid.Rows.Count > 0)
                {
                    // Reset            
                    AHGrid.DataSource = null;
                    AHGrid.DataBind();
                    // Clear All columns except the manually bound columns
                    for (int i = AHGrid.Columns.Count - 1; i >= 1; i--)
                    {
                        AHGrid.Columns.RemoveAt(i);
                    }
                }

                Cache.Remove("Acquired Haemophilia");
                createForm("Acquired Haemophilia", "Acquired Haemophilia", AHGrid, dataSet, true, false, string.Empty, false);

                // BInd Data for Surgery CH
                DataTable dataSetSurgery = new DataTable();
                // string sqlquerySurgery = string.Format(Queries.getFormData, countriesDdl.SelectedValue, periodsDdl.SelectedValue, "4");
                string sqlquerySurgery = string.Format(Queries.getFormData, countriesDdl.SelectedValue, periodsDdl.SelectedValue, "4", periodsDdl.SelectedValue.Substring(0, 4));

                using (SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlquerySurgery, dbConnection))
                {
                    // fill the DataSet using our DataAdapter 
                    dataAdapter.Fill(dataSetSurgery);
                }

                // Reset form and remove all columns except the first 3
                if (SurgeryGrid.Rows.Count > 0)
                {
                    // Reset            
                    SurgeryGrid.DataSource = null;
                    SurgeryGrid.DataBind();
                    // Clear All columns except the manually bound columns
                    for (int i = SurgeryGrid.Columns.Count - 1; i >= 1; i--)
                    {
                        SurgeryGrid.Columns.RemoveAt(i);
                    }
                }

                Cache.Remove("Surgery CH");
                createForm("Surgery", "Surgery CH", SurgeryGrid, dataSetSurgery, true, true, "Total Surgeries", false);

                // bind data for factor 7
                DataTable dataSetFactor = new DataTable();
                string sqlqueryFactor = string.Format(Queries.getFormData, countriesDdl.SelectedValue, periodsDdl.SelectedValue, "5", periodsDdl.SelectedValue.Substring(0, 4));

                using (SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlqueryFactor, dbConnection))
                {
                    // fill the DataSet using our DataAdapter 
                    dataAdapter.Fill(dataSetFactor);
                }

                // Reset form and remove all columns except the first 3
                if (FactorGrid.Rows.Count > 0)
                {
                    // Reset            
                    FactorGrid.DataSource = null;
                    FactorGrid.DataBind();
                    // Clear All columns except the manually bound columns
                    for (int i = FactorGrid.Columns.Count - 1; i >= 1; i--)
                    {
                        FactorGrid.Columns.RemoveAt(i);
                    }
                }

                Cache.Remove("Factor VII Deficiency");
                createForm("Factor VII Deficiency", "Factor VII Deficiency", FactorGrid, dataSetFactor, false, false, string.Empty, false);


                // bind data for factor 7
                DataTable dataSetThrombosis = new DataTable();
                string sqlqueryThrombosis = string.Format(Queries.getFormData, countriesDdl.SelectedValue, periodsDdl.SelectedValue, "6", periodsDdl.SelectedValue.Substring(0, 4));

                using (SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlqueryThrombosis, dbConnection))
                {
                    // fill the DataSet using our DataAdapter 
                    dataAdapter.Fill(dataSetThrombosis);
                }

                // Reset form and remove all columns except the first 3
                if (thrombosisGrid.Rows.Count > 0)
                {
                    // Reset            
                    thrombosisGrid.DataSource = null;
                    thrombosisGrid.DataBind();
                    // Clear All columns except the manually bound columns
                    for (int i = thrombosisGrid.Columns.Count - 1; i >= 1; i--)
                    {
                        thrombosisGrid.Columns.RemoveAt(i);
                    }
                }

                Cache.Remove("Glanzmann's thrombasthenia");
                createForm("Glanzmann's Thrombasthenia", "Glanzmann's thrombasthenia", thrombosisGrid, dataSetThrombosis, false, false, string.Empty, false);

                // bind data for Age Split
                DataTable dataSetAgeSplit = new DataTable();
                string sqlqueryAgeSplit = string.Format(Queries.getFormDataCategory, countriesDdl.SelectedValue, periodsDdl.SelectedValue, "7");

                using (SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlqueryAgeSplit, dbConnection))
                {
                    // fill the DataSet using our DataAdapter 
                    dataAdapter.Fill(dataSetAgeSplit);
                }

                // Reset form and remove all columns except the first 3
                if (AgeSplitGrid.Rows.Count > 0)
                {
                    // Reset            
                    AgeSplitGrid.DataSource = null;
                    AgeSplitGrid.DataBind();
                    // Clear All columns except the manually bound columns
                    for (int i = AgeSplitGrid.Columns.Count - 1; i >= 1; i--)
                    {
                        AgeSplitGrid.Columns.RemoveAt(i);
                    }
                }



                Cache.Remove("CH Age split");
                createForm("Age split", "CH Age split", AgeSplitGrid, dataSetAgeSplit, false, true, "Total Patients", true);


                // bind data for patients Form
                DataTable dataSetPatients = new DataTable();
                string sqlqueryPatients = string.Format(Queries.getPatientsFormData, countriesDdl.SelectedValue, periodsDdl.SelectedValue, "8");

                using (SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlqueryPatients, dbConnection))
                {
                    // fill the DataSet using our DataAdapter 
                    dataAdapter.Fill(dataSetPatients);
                }

                // Reset form and remove all columns except the first 3
                if (PatientsGrid.Rows.Count > 0)
                {
                    // Reset            
                    PatientsGrid.DataSource = null;
                    PatientsGrid.DataBind();
                    // Clear All columns except the manually bound columns
                    for (int i = PatientsGrid.Columns.Count - 1; i >= 1; i--)
                    {
                        PatientsGrid.Columns.RemoveAt(i);
                    }
                }

                Cache.Remove("Patients");
                createPatientsForm("ITI and clinical trial","Patients", PatientsGrid, dataSetPatients);
            }

        }

        private void populateForm(string selectedCountry, string selectedPeriod)
        {
            // create the DataSet will be used for reference
            DataTable dataSet = new DataTable();
            DataTable dataSetAllProducts = new DataTable();
            
            string sqlquery = string.Format(@"select t1.id, t1.Product, t1.Active, t1.ProductGroup, t1.ProductType, t1.ProductGroupId from

                            (SELECT  Products.[id]
                                      ,[Product]
                                      ,[Active]
                                      ,[ProductGroup]
                                      ,[ProductType]
                                      ,ProductGroups.[id] as ProductGroupId
                                      ,ProductGroups.ProductGroupOrder
                                  FROM [PCM].[dbo].[Products]                                  
                                  left join [PCM].[dbo].ProductGroups
                                    on [PCM].[dbo].[Products].ProductGroupId = [ProductGroups].id
                                  Where [PCM].[dbo].[Products].Active = 1) t1

								  Inner Join 
									(SELECT [ProductId]
									  FROM [PCM].[dbo].[productformcountry]
									  Inner Join [PCM].[dbo].[mappings]
									  On [PCM].[dbo].[productformcountry].[FormCountryId] = [PCM].[dbo].[mappings].id
									  where CountryId = {0} and FormId = 1) t2

									  on t1.id = t2.ProductId

									  order by t1.ProductType, t1.ProductGroupOrder, t1.ProductGroupId, t1.id", selectedCountry);


            string sqlqueryAllProducts = "SELECT [id] FROM[PCM].[dbo].[Products]";

            using (SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlquery, dbConnection))
            {
                // fill the DataSet using our DataAdapter 
                dataAdapter.Fill(dataSet);
            }

            using (SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlqueryAllProducts, dbConnection))
            {
                // fill the DataSet using our DataAdapter 
                dataAdapter.Fill(dataSetAllProducts);
            }


            // Create Active and selected products query selector
            string inSQLCLause = "(";
            for (int i = 0; i < dataSet.Rows.Count; i++)
            {
                inSQLCLause += "'" + dataSet.Rows[i]["id"].ToString() + "',";
            }
            inSQLCLause += "'1000','1002','lost','losttole','losttrial')";

            string inSQLCLauseAllProducts = "(";
            for (int i = 0; i < dataSetAllProducts.Rows.Count; i++)
            {
                inSQLCLauseAllProducts += "'" + dataSetAllProducts.Rows[i]["id"].ToString() + "',";
            }
            inSQLCLauseAllProducts += "'1000','1002','lost','losttole','losttrial')";


            // Contains all active and selected products
            DataTable dtFiltered = dataSet.Copy();

            dtFiltered.Columns.Add("Value", typeof(String));

            string trial = "Congenital Haemophelia";
            string startTreatment = "Start Treatment";

            // Count number of products - might not be needed
            numberOfProduct = dtFiltered.Rows.Count;
            ViewState.Add("productCount", numberOfProduct);

            // Manually adding Header Columns - must be handled dynamically
            DataRow dr1 = dtFiltered.NewRow();
            dr1["ProductType"] = trial;
            dr1["Product"] = trial;
            dr1["Value"] = trial;

            DataRow dr2 = dtFiltered.NewRow();
            dr2["ProductType"] = startTreatment;
            dr2["Product"] = startTreatment;
            dr2["Value"] = startTreatment;

            DataRow dr3 = dtFiltered.NewRow();
            dr3["ProductType"] = startTreatment;
            dr3["Product"] = startTreatment;
            dr3["Value"] = startTreatment;

            dtFiltered.Rows.InsertAt(dr2, 0);
            dtFiltered.Rows.InsertAt(dr3, 0);
            dtFiltered.Rows.InsertAt(dr1, 0);

            // Add pup and previously tolerized rows. Might dynamically add aggregation columns.
            // Issue with databinding and reference
            string[] extraLines = new string[] { "PUP", "Previously tolerized" };
            // get distinct product groupID's and names

            for (int i = 0; i < extraLines.Count(); i++)
            {
                DataRow row = dtFiltered.NewRow();
                row["ProductType"] = extraLines[i];
                row["Product"] = extraLines[i];
                row["Value"] = extraLines[i];
                if (extraLines[i] == "PUP")
                {
                    row["id"] = 1000;
                }
                else if (extraLines[i] == "Previously tolerized")
                {
                    row["id"] = 1002;
                }
                dtFiltered.Rows.Add(row);


            }

            // Must Contain reference to productgroupId
            List<ProductGroup> aggrLines = new List<ProductGroup>();
            aggrLines.Add(new ProductGroup { productGroup = "Total Inhibitor patients", productId = "0" });
            // get distinct product groupID's and names
            for (int i = 0; i < dataSet.Rows.Count; i++)
            {
                if (aggrLines.Where(p => p.productId == dataSet.Rows[i]["ProductGroupId"].ToString()).Count() == 0)
                {
                    aggrLines.Add(new ProductGroup { productGroup = dataSet.Rows[i]["ProductGroup"].ToString(), productId = dataSet.Rows[i]["ProductGroupId"].ToString() });
                }
            }

            for (int i = 0; i < aggrLines.Count(); i++)
            {
                DataRow row = dtFiltered.NewRow();
                row["ProductType"] = aggrLines[i].productGroup;
                row["Product"] = aggrLines[i].productGroup;
                row["ProductGroupId"] = aggrLines[i].productId;
                dtFiltered.Rows.Add(row);
            }

            DataTable dtHorizontal = dataSet.Copy();


            DataColumn newColumn = new DataColumn("Switch", typeof(System.String));
            newColumn.DefaultValue = "Switch To";
            dtHorizontal.Columns.Add(newColumn);


            dtHorizontal.Columns["Switch"].SetOrdinal(1);
            dtHorizontal.Columns["ProductType"].SetOrdinal(2);
            dtHorizontal.Columns["Product"].SetOrdinal(3);


            dtHorizontal = Transpose(dtHorizontal);
            // Clean up data source
            dtHorizontal.Columns.RemoveAt(0);

            // Only keep 3 rows - must keep the ID of the column
            for (int i = dtHorizontal.Rows.Count - 1; i > 2; i--)
            {
                dtHorizontal.Rows.RemoveAt(i);
            }


            //dtHorizontal contains all products.  
            for (int i = 0; i < dtHorizontal.Columns.Count; i++)
            {
                {

                    TemplateField tfObject = new TemplateField();
                    tfObject.HeaderText = dataSet.Rows[i]["id"].ToString();//i.ToString();                   
                                                                           //tfObject.HeaderStyle.Width = Unit.Percentage(30);
                    tfObject.HeaderTemplate = new CreateItemTemplate(ListItemType.Header, dataSet.Rows[i]["id"].ToString(), 3, 4 + numberOfProduct);
                    tfObject.ItemTemplate = new CreateItemTemplate(ListItemType.Item, dataSet.Rows[i]["id"].ToString(), 3, 4 + numberOfProduct);

                    MontlyInputGrid.Columns.Add(tfObject);
                                     
                    // Add datacolumn to 
                    DataColumn dc = dtHorizontal.Columns[i];
                    dtFiltered.Columns.Add(dataSet.Rows[i]["id"].ToString(), dc.DataType);
                }
            }


            // MergeTables
            for (int i = 0; i < dtHorizontal.Rows.Count; i++)
            {
                DataRow row = dtHorizontal.Rows[i];

                for (int j = 0; j < dtHorizontal.Columns.Count; j++)
                {
                    // Must set dynamic offset
                    try
                    {
                        dtFiltered.Rows[i][j + 7] = row[j].ToString();
                    }
                    catch (Exception exp)
                    { }

                }

            }

            // Create text,id array to create new product columns
            string[,] productArray = new string[3, 2] { { "Lost", "lost" }, { "Lost to trials", "losttrial" }, { "Lost to tolerization", "losttole" } };

            for (int i = 0; i < productArray.GetLength(0); i++)
            {
                dtFiltered.Columns.Add(productArray[i, 1], typeof(String));

                TemplateField tfObject = new TemplateField();
                tfObject.HeaderText = productArray[i, 1];
                //tfObject.HeaderStyle.Width = Unit.Percentage(30);
                tfObject.HeaderTemplate = new CreateItemTemplate(ListItemType.Header, productArray[i, 1], 3, 4 + numberOfProduct);
                tfObject.ItemTemplate = new CreateItemTemplate(ListItemType.Item, productArray[i, 1], 3, 4 + numberOfProduct);
                MontlyInputGrid.Columns.Add(tfObject);
               


                dtFiltered.Rows[0][productArray[i, 1]] = "Switch To";
                dtFiltered.Rows[1][productArray[i, 1]] = productArray[i, 0];
                dtFiltered.Rows[2][productArray[i, 1]] = productArray[i, 0];

            }


            dtFiltered.Columns.Add("netflow", typeof(String));
            dtFiltered.Columns.Add("end", typeof(String));

            // Add Columns for End treatment
          /*  BoundField netflowfield = new BoundField();
              netflowfield.HeaderText = "netflow";
              netflowfield.DataField = "netflow";            
              netflowfield.ReadOnly = true;
              MontlyInputGrid.Columns.Add(netflowfield);

            BoundField endfield = new BoundField();
            endfield.HeaderText = "end";
            endfield.DataField = "end";
            endfield.ReadOnly = true;
            MontlyInputGrid.Columns.Add(endfield);*/

            TemplateField netf = new TemplateField();
            netf.HeaderText = "netflow";
            netf.HeaderStyle.Width = Unit.Percentage(30);                       
            netf.HeaderTemplate = new CreateItemTemplate(ListItemType.Header, "netflow", 3,99);
            netf.ItemTemplate = new CreateItemTemplate(ListItemType.Item, "netflow", 3,99);
            MontlyInputGrid.Columns.Add(netf);

            TemplateField end = new TemplateField();
            end.HeaderText = "end";
            end.HeaderStyle.Width = Unit.Percentage(30);
            end.HeaderTemplate = new CreateItemTemplate(ListItemType.Header, "end", 3,99);
            end.ItemTemplate = new CreateItemTemplate(ListItemType.Item, "end", 3,99);
            MontlyInputGrid.Columns.Add(end);



            dtFiltered.Rows[0]["netflow"] = "End treatment";
            dtFiltered.Rows[0]["end"] = "End treatment";

            dtFiltered.Rows[1]["netflow"] = "Net flow";
            dtFiltered.Rows[1]["end"] = "End";

            dtFiltered.Rows[2]["netflow"] = "Net flow";
            dtFiltered.Rows[2]["end"] = "End";

            //Bind actual value to datatable
            // create the DataSet will be used for reference
            DataTable dataValues = new DataTable();


            string sqlQuery = string.Format(@"SELECT        
                                               [X]
                                              ,[Y]
                                              ,[Value]      
                                          FROM [PCM].[dbo].[FormData]
                                          Where CountryId = {0} and Period = '{1}' and FormId=1 and [X] in {2} and [Y] in {2}", selectedCountry, selectedPeriod, inSQLCLause);


            using (SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlQuery, dbConnection))
            {
                // fill the DataSet using our DataAdapter 
                dataAdapter.Fill(dataValues);
            }

            for (int i = 0; i < dataValues.Rows.Count; i++)
            {
                //Find row Where the Y id
                string searchString = "id =" + dataValues.Rows[i]["Y"].ToString();
                DataRow[] foundRows = dtFiltered.Select(searchString);
                if (foundRows.Count() == 1)
                {
                    int row = dtFiltered.Rows.IndexOf(foundRows[0]);
                    string xValue = dataValues.Rows[i]["X"].ToString();
                    dtFiltered.Rows[row][xValue] = dataValues.Rows[i]["Value"].ToString();
                }
            }

            // Bind start Values. Only include active and selected products. This query returns all products with data
            // Must check that values products are valid both from and x and y perspective
            string sqlQueryStartValues = string.Format(@"Select  (case when t1.Product is null then t2.Product else t1.Product END) as Product, isnull(t1.XValue,0) as XValue, isnull(t2.YValue,0) as YValue, t2.[ProductGroupId]
                                                                        from
	                                                                        (SELECT      
                                                                               [X] as Product     
                                                                              ,Sum([Value]) as XValue
                                                                          FROM [PCM].[dbo].[FormData]
                                                                          Where Period < {0} and FormId =1 and CountryId = {1} and [X] in {2} and [Y] in {3}
                                                                          Group by [X]) t1

                                                                           FULL OUTER JOIN

                                                                            (SELECT      
                                                                               [Y] as Product,
	                                                                           [ProductGroupId],    
                                                                              Sum([Value]) as YValue
                                                                          FROM [PCM].[dbo].[FormData]
                                                                          Left Join [PCM].[dbo].[Products]
                                                                          on [Y] = [PCM].[dbo].[Products].[id]
                                                                          Where Period < {0} and FormId =1 and CountryId = {1} and [Y] in {2} and [X] in {3}
                                                                          Group by [Y],[ProductGroupId]) t2
                                                                          on
                                                                          t1.Product = t2.Product order by t2.ProductGroupId, t1.Product,t2.Product", selectedPeriod, selectedCountry, inSQLCLause, inSQLCLauseAllProducts);

            // Must check that values products are valid both from and x and y perspective
            string sqlQueryNetFlowValues = string.Format(@"Select  (case when t1.Product is null then t2.Product else t1.Product END) as Product,  isnull(t1.XValue,0) as XValue, isnull(t2.YValue,0) as YValue, t2.[ProductGroupId]
                                                                        from
	                                                                        (SELECT      
                                                                               [X] as Product     
                                                                              ,Sum([Value]) as XValue
                                                                          FROM [PCM].[dbo].[FormData]
                                                                          Where Period = {0} and FormId =1 and CountryId = {1} and [X] in {2} and [Y] in {3}
                                                                          Group by [X]) t1

                                                                           FULL OUTER JOIN

                                                                            (SELECT      
                                                                               [Y] as Product,
	                                                                           [ProductGroupId],    
                                                                              Sum([Value]) as YValue
                                                                          FROM [PCM].[dbo].[FormData]
                                                                          Left Join [PCM].[dbo].[Products]
                                                                          on [Y] = [PCM].[dbo].[Products].[id]
                                                                          Where Period = {0} and FormId =1 and CountryId = {1} and [Y] in {2} and [X] in {3}
                                                                          Group by [Y],[ProductGroupId]) t2
                                                                          on
                                                                          t1.Product = t2.Product order by t2.ProductGroupId, t1.Product,t2.Product", selectedPeriod, selectedCountry, inSQLCLause, inSQLCLauseAllProducts);

            DataTable dataStartValues = new DataTable();
            DataTable dataNetFlowValues = new DataTable();

            using (SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlQueryStartValues, dbConnection))
            {
                // fill the DataSet using our DataAdapter 
                dataAdapter.Fill(dataStartValues);
            }

            using (SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlQueryNetFlowValues, dbConnection))
            {
                // fill the DataSet using our DataAdapter 
                dataAdapter.Fill(dataNetFlowValues);
            }

            int value;

            Dictionary<string, int> dicSum = new Dictionary<string, int>();
            Dictionary<string, int> dicSumNet = new Dictionary<string, int>();


            // Handle 3 cases
            // 1. startValues and netFlow Values exist. Then Bind data
            // 2. startvalues exist, but nothing entered for current period.Then enter zero for net values and end
            // 3. Nothing entered. Then bind zero to start,netflow and end 

            bool periodContaisData = false;
            bool countryContaisData = false;

            if (dataStartValues.Rows.Count > 0)
            {
                countryContaisData = true;
            }

            if (dataNetFlowValues.Rows.Count > 0)
            {
                periodContaisData = true;
            }


            if (countryContaisData)
            {
                // Bind start values to products
                for (int i = 0; i < dataStartValues.Rows.Count; i++)
                {
                    if (!DBNull.Value.Equals(dataStartValues.Rows[i]["ProductGroupId"]))
                    {
                        // Get value to insert

                        string searchString = "id =" + dataStartValues.Rows[i]["Product"].ToString();
                        DataRow[] foundRows = dtFiltered.Select(searchString);
                        if (foundRows.Count() == 1)
                        {
                            int row = dtFiltered.Rows.IndexOf(foundRows[0]);
                            int startValue = int.Parse(dataStartValues.Rows[i]["XValue"].ToString()) - int.Parse(dataStartValues.Rows[i]["YValue"].ToString());
                            dtFiltered.Rows[row]["Value"] = startValue;

                            int netflowValue = 0;

                            if (periodContaisData)
                            {
                                string searchStringNetFlow = "Product ='" + dataStartValues.Rows[i]["Product"].ToString() + "'";
                                DataRow[] foundNetRows = dataNetFlowValues.Select(searchStringNetFlow);
                                if (foundNetRows.Count() == 1)
                                {
                                    int rowindex = dataNetFlowValues.Rows.IndexOf(foundNetRows[0]);
                                    netflowValue = int.Parse(dataNetFlowValues.Rows[rowindex]["XValue"].ToString()) - int.Parse(dataNetFlowValues.Rows[rowindex]["YValue"].ToString());
                                }

                                    // The error appears here. Data has been added for this month previously on old and fewer products and now it is decided to go back in time and change values based on
                                    // new settings. The result is that the returned products and index does not match.
                                    // 1. consider lookup and find product and associated row
                                    // 2. Alternatively ensure that the sql returns a row per product in the in statement  
                                  //  netflowValue = int.Parse(dataNetFlowValues.Rows[i]["XValue"].ToString()) - int.Parse(dataNetFlowValues.Rows[i]["YValue"].ToString());
                            }

                            dtFiltered.Rows[row]["netflow"] = netflowValue;
                            dtFiltered.Rows[row]["end"] = startValue + netflowValue;
                            string group = dataStartValues.Rows[i]["ProductGroupId"].ToString().ToString();

                            if (dicSum.ContainsKey(group))
                            {
                                dicSum[group] += startValue;
                                dicSumNet[group] += netflowValue;

                            }
                            else
                            {
                                dicSum.Add(group, startValue);
                                dicSumNet.Add(group, netflowValue);
                            }
                        }
                    }
                    else if (int.TryParse(dataStartValues.Rows[i]["Product"].ToString(), out value))
                    {
                        if (value >= 1000)
                        {
                            string searchString = "id =" + value.ToString();
                            DataRow[] foundRows = dtFiltered.Select(searchString);
                            if (foundRows.Count() == 1)
                            {
                                int row = dtFiltered.Rows.IndexOf(foundRows[0]);
                                if (periodContaisData)
                                {
                                    dtFiltered.Rows[row]["netflow"] = int.Parse(dataNetFlowValues.Rows[i]["YValue"].ToString());
                                }
                                else
                                {
                                    dtFiltered.Rows[row]["netflow"] = 0;
                                }
                            }
                        }
                    }
                }
            }
            // What happend when start values are zero. We should still calculate end values
            else
            {

               for (int i = 0; i < dataNetFlowValues.Rows.Count; i++)
                {
                    if (!DBNull.Value.Equals(dataNetFlowValues.Rows[i]["ProductGroupId"]))
                    {
                        // Get value to insert

                        string searchStringNetFlow = "id ='" + dataNetFlowValues.Rows[i]["Product"].ToString() + "'";
                        DataRow[] foundRows = dtFiltered.Select(searchStringNetFlow);
                        if (foundRows.Count() == 1)
                        {
                            int row = dtFiltered.Rows.IndexOf(foundRows[0]);
                            int startValue = 0;
                            dtFiltered.Rows[row]["Value"] = startValue;

                            int netflowValue = 0;

                            if (periodContaisData)
                            {                              
                                  netflowValue = int.Parse(dataNetFlowValues.Rows[i]["XValue"].ToString()) - int.Parse(dataNetFlowValues.Rows[i]["YValue"].ToString());
                               
                             }

                            dtFiltered.Rows[row]["netflow"] = netflowValue;
                            dtFiltered.Rows[row]["end"] = startValue + netflowValue;
                            string group = dataNetFlowValues.Rows[i]["ProductGroupId"].ToString();

                            if (dicSum.ContainsKey(group))
                            {
                                dicSum[group] += startValue;
                                dicSumNet[group] += netflowValue;

                            }
                            else
                            {
                                dicSum.Add(group, startValue);
                                dicSumNet.Add(group, netflowValue);
                            }
                        }
                    }
                    else if (int.TryParse(dataNetFlowValues.Rows[i]["Product"].ToString(), out value))
                    {
                        if (value >= 1000)
                        {
                            string searchString = "id =" + value.ToString();
                            DataRow[] foundRows = dtFiltered.Select(searchString);
                            if (foundRows.Count() == 1)
                            {
                                int row = dtFiltered.Rows.IndexOf(foundRows[0]);
                                if (periodContaisData)
                                {
                                    dtFiltered.Rows[row]["netflow"] = int.Parse(dataNetFlowValues.Rows[i]["YValue"].ToString());
                                }
                                else
                                {
                                    dtFiltered.Rows[row]["netflow"] = 0;
                                }
                            }
                        }
                    }
                }




                /*   for (int i = rowOffset; i < dtFiltered.Rows.Count; i++)
                   {
                       dtFiltered.Rows[i]["netflow"] = 0;

                       if (dtFiltered.Rows[i]["Product"].ToString() != "PUP" && dtFiltered.Rows[i]["Product"].ToString() != "Previously tolerized")
                       {
                           dtFiltered.Rows[i]["Value"] = 0;
                           dtFiltered.Rows[i]["end"] = 0;
                       }

                   }*/

            }



            int Total = 0;
            int netFlowTotal = 0;

            // Bind group logic
            foreach (string g in dicSum.Keys)
            {
                string searchString = "ProductGroupId =" + g.ToString() + " and id is null";
                DataRow[] foundRows = dtFiltered.Select(searchString);
                if (foundRows.Count() == 1)
                {
                    int row = dtFiltered.Rows.IndexOf(foundRows[0]);
                    dtFiltered.Rows[row]["Value"] = dicSum[g];
                    dtFiltered.Rows[row]["netflow"] = dicSumNet[g];
                    dtFiltered.Rows[row]["end"] = dicSum[g] + dicSumNet[g];
                    Total += dicSum[g];
                    netFlowTotal += dicSumNet[g];
                }

            }

            //Update Total column           
            DataRow[] totalRow = dtFiltered.Select("ProductGroupId = 0");
            if (totalRow.Count() == 1)
            {
                int row = dtFiltered.Rows.IndexOf(totalRow[0]);
                dtFiltered.Rows[row]["Value"] = Total;
                dtFiltered.Rows[row]["netflow"] = netFlowTotal;
                dtFiltered.Rows[row]["end"] = Total + netFlowTotal;
            }


            // Get Netflow and end values


            // Data bind
            MontlyInputGrid.DataSource = dtFiltered;
            MontlyInputGrid.DataBind();

            // Store
            DataControlFieldCollection columns = MontlyInputGrid.Columns;
            Cache.Insert("cols", columns);
        }

        private void createPatientsForm(string formName, string cacheName, GridView grid, DataTable dt)
        {
            // Create Template Column
            TemplateField tfObject = new TemplateField();
            tfObject.HeaderText = "Value";
            tfObject.HeaderTemplate = new CreateItemTemplate(ListItemType.Header, "Value", 1, 2);
            tfObject.ItemTemplate = new CreateItemTemplate(ListItemType.Item, "Value", 1, 2);
            grid.Columns.Add(tfObject);

            DataRow titleRow = dt.NewRow();           
            titleRow["Name"] = formName;
            titleRow["Value"] = string.Empty;
            dt.Rows.InsertAt(titleRow, 0);

            grid.DataSource = dt;
            grid.DataBind();

            DataControlFieldCollection columns = grid.Columns;
            Cache.Insert(cacheName, columns);

        }

        private void createForm(string formName, string cacheName, GridView grid, DataTable dt, bool includeYTD, bool includeTotalLine, string totalLineText, bool includeProductCategory)
        {
            // ADD product columns to dataset
            // 1. Must transpose table
            // 2. Add products to table 
            // 3. Create two datarows, one with title and one with product names
            // 4. Must handle textbox template
            // 5. Must handle YTD, also in title 
            // 6. Bind actual values (consider doint it in one sql load)  

            // 8. Handle Format of chart

            // 7. Handle viewstate 
            // 9. Save Data
            // 10. Create JQuery to update YTD


            int rowCount = dt.Rows.Count;
            ArrayList addedId = new ArrayList();
            ArrayList addedProduct = new ArrayList();
            ArrayList categories = new ArrayList();
            ArrayList productCat = new ArrayList();

            int catCount = dt.AsEnumerable().Select(r => r.Field<string>("Name")).Distinct().Count();

            for (int i = 0; i < rowCount; i++)
            {
                if(!addedId.Contains(dt.Rows[i]["id"].ToString()))
                {
                    TemplateField tfObject = new TemplateField();
                    tfObject.HeaderText = dt.Rows[i]["id"].ToString();
                    tfObject.HeaderTemplate = new CreateItemTemplate(ListItemType.Header, dt.Rows[i]["id"].ToString(), 2, 1+catCount);
                    tfObject.ItemTemplate = new CreateItemTemplate(ListItemType.Item, dt.Rows[i]["id"].ToString(), 2, 1 + catCount);
                    grid.Columns.Add(tfObject);

                    addedId.Add(dt.Rows[i]["id"].ToString());
                    addedProduct.Add(dt.Rows[i]["Product"].ToString());
                    if(includeProductCategory)
                    {
                        productCat.Add(dt.Rows[i]["ProductType"].ToString());
                    }

                }
            }

            if (includeYTD)
            {                
                  for (int i = 0; i < addedId.Count; i++)                    
                    {
                        string id = "ytd-" + addedId[i].ToString();                       

                        TemplateField tfObject = new TemplateField();
                        tfObject.HeaderText = id;
                        tfObject.HeaderTemplate = new CreateItemTemplate(ListItemType.Header, id, 2, 4);
                        tfObject.ItemTemplate = new CreateItemTemplate(ListItemType.Item, id, 2, 4);                       
                        grid.Columns.Add(tfObject);
                    }
              }

                DataTable xTransformed = new DataTable();

                for (int i = 0; i < addedId.Count; i++)
                {
                    xTransformed.Columns.Add(addedId[i].ToString(), typeof(String));
                    if (includeYTD)
                    {
                        xTransformed.Columns.Add("ytd-" + addedId[i].ToString(), typeof(String));
                    }
                }

                xTransformed.Columns.Add("label", typeof(String));

                for (int i = 0; i < rowCount; i++)
                {
                    // Only create rows for the first productID.
                    if (!categories.Contains(dt.Rows[i]["Name"].ToString()))
                    {
                        DataRow r = xTransformed.NewRow();
                        r[dt.Rows[i]["id"].ToString()] = dt.Rows[i]["value"];
                        if (includeYTD)
                        {
                            r["ytd-" + dt.Rows[i]["id"].ToString()] = dt.Rows[i]["ytd"];
                        }
                        r["label"] = dt.Rows[i]["Name"].ToString();
                        xTransformed.Rows.Add(r);

                        categories.Add(dt.Rows[i]["Name"].ToString());
                    }
                    else
                    {
                        DataRow[] result = xTransformed.Select("label = '" + dt.Rows[i]["Name"].ToString() + "'");
                        if (result.Count() > 0)
                        {
                            DataRow r = result[0];
                            r[dt.Rows[i]["id"].ToString()] = dt.Rows[i]["value"];
                            if (includeYTD)
                            {
                                r["ytd-" + dt.Rows[i]["id"].ToString()] = dt.Rows[i]["ytd"];
                            }
                        }

                    }

                    // dt.Rows[i]["id"].ToString();
                }

                DataRow titleRow = xTransformed.NewRow();
                DataRow labelRow = xTransformed.NewRow();

                titleRow["label"] = formName;
                labelRow["label"] = formName;

                for (int i = 0; i < addedId.Count; i++)
                {
                        // Note assumption is that YTD and display of categories cannot exist together 
                        if (includeProductCategory)
                        {
                            titleRow[addedId[i].ToString()] = productCat[i].ToString();
                        }
                        else
                        {
                    
                        string reportingInterval = string.Empty;
                        string period = string.Empty;

                    if (ViewState["period"] != null)
                        {
                            period = (string)ViewState["period"];
                            
                        }

                    if (ViewState["reportinginterval"] != null)
                        {
                            if((string)ViewState["reportinginterval"] == "Quarterly")
                            {
                                int quarter = int.Parse(period.Substring(4, 2)) / 3;
                                reportingInterval = quarter + "Q-" + period.Substring(0, 4);
                            }
                            else
                            { 
                                
                                reportingInterval = DateTime.ParseExact(period, "yyyyMM", null).ToString("MMM-yyyy", CultureInfo.InvariantCulture); 
                             }

                        }

                        titleRow[addedId[i].ToString()] = reportingInterval;

                    if (includeYTD)
                            {
                                titleRow["ytd-" + addedId[i].ToString()] = "YTD";
                            }
                        }
                }

                for (int i = 0; i < addedProduct.Count; i++)
                {
                    labelRow[addedId[i].ToString()] = addedProduct[i].ToString();
                    if (includeYTD)
                    { labelRow["ytd-" + addedId[i].ToString()] = addedProduct[i].ToString(); }
                }

                if (includeTotalLine)
                {
                    DataRow totalRow = xTransformed.NewRow();
                    totalRow["label"] = totalLineText;
                    // Get sum of products
                    for (int i = 0; i < addedId.Count; i++)
                    {
                        int sum = 0;
                        int ytdSum = 0;

                        for (int j = 0; j < xTransformed.Rows.Count; j++)
                        {
                            int value;
                            if (int.TryParse(xTransformed.Rows[j][addedId[i].ToString()].ToString(), out value))
                            {
                                sum += value;
                            }
                          
                            if (includeYTD)
                            {
                                int YTDvalue;
                                if (int.TryParse(xTransformed.Rows[j]["ytd-" + addedId[i].ToString()].ToString(), out YTDvalue))
                                {
                                    ytdSum += YTDvalue;
                                }
                            }
                        }

                        totalRow[addedId[i].ToString()] = sum;

                        if (includeYTD)
                        {
                            totalRow["ytd-" + addedId[i].ToString()] = ytdSum;
                        }
                    }

                    xTransformed.Rows.Add(totalRow);
                }

                xTransformed.Rows.InsertAt(labelRow, 0);
                xTransformed.Rows.InsertAt(titleRow, 0);

                grid.DataSource = xTransformed;
                grid.DataBind();
            

            DataControlFieldCollection columns = grid.Columns;
            Cache.Insert(cacheName, columns);
        }

        protected void restore_columns()
        {
            // Restore all columns except netflow and end
            if (Cache["cols"] != null && !Cache["cols"].Equals("-1"))
            {
                DataControlFieldCollection columns = (DataControlFieldCollection)Cache["cols"];

                int colCount = MontlyInputGrid.Columns.Count;
                /* for(int i = colCount-3; i >= 0; i--)
                 {
                     MontlyInputGrid.Columns.RemoveAt(i);
                 }*/
                                 
                MontlyInputGrid.Columns.Clear(); 

                foreach (DataControlField field in columns)
                {
                        MontlyInputGrid.Columns.Add(field);
                    if (field.GetType() == typeof(TemplateField))
                    {
                        TemplateField tf = (TemplateField)field;
                        int i = MontlyInputGrid.Columns.IndexOf(tf);
                        if (tf.HeaderTemplate != null)
                        {
                            try
                            {
                                tf.HeaderTemplate.InstantiateIn(MontlyInputGrid.HeaderRow.Cells[i]);
                            }
                            catch (Exception exp) { }
                        }
                        foreach (GridViewRow row in MontlyInputGrid.Rows)
                        {
                            if (tf.ItemTemplate != null)
                            {
                                tf.ItemTemplate.InstantiateIn(row.Cells[i]);
                            }
                        }
                        if (tf.FooterTemplate != null)
                        {
                            tf.FooterTemplate.InstantiateIn(MontlyInputGrid.FooterRow.Cells[i]);
                        }
                    }      
                    
                }
            }
        }

        protected void restore_columns(string formname, GridView grid)
        {
            if (Cache[formname] != null && !Cache[formname].Equals("-1"))
            {
                DataControlFieldCollection columns = (DataControlFieldCollection)Cache[formname];
                grid.Columns.Clear(); // gvSchluessel is the GridView
                foreach (DataControlField field in columns)
                {
                    grid.Columns.Add(field);
                    if (field.GetType() == typeof(TemplateField))
                    {
                        TemplateField tf = (TemplateField)field;
                        int i = grid.Columns.IndexOf(tf);
                        if (tf.HeaderTemplate != null)
                        {
                            try
                            {
                                tf.HeaderTemplate.InstantiateIn(grid.HeaderRow.Cells[i]);
                            }
                            catch (Exception exp) { }
                        }
                        foreach (GridViewRow row in grid.Rows)
                        {
                            if (tf.ItemTemplate != null)
                            {
                                tf.ItemTemplate.InstantiateIn(row.Cells[i]);
                            }
                        }
                        if (tf.FooterTemplate != null)
                        {
                            tf.FooterTemplate.InstantiateIn(grid.FooterRow.Cells[i]);
                        }
                    }
                }
            }
        }

        // Save 
        protected void saveBtn_Click(object sender, EventArgs e)
        {
            // Save data back to database
            // number of products + 2 extra lines

            int productCount = (int)ViewState["productCount"];

            int rowCount = productCount + rowOffset + extraRowProducts;
            // Number of products + 3 extra lines
            int colCount = productCount + colOffset + extraColProducts;
            for (int i = rowOffset; i < rowCount; i++)
            {
                GridViewRow grow = MontlyInputGrid.Rows[i];

                for (int j = colOffset; j < colCount; j++)
                {
                    if (grow.Cells[j].HasControls())
                    {
                        TextBox tb = (TextBox)grow.Cells[j].Controls[0];
                        // Only save values for cells that are not read-only

                        // get referece to the product x and y id
                        string xValue = MontlyInputGrid.HeaderRow.Cells[j].Text;
                        string yValue = MontlyInputGrid.Rows[i].Cells[0].Text;
                        string period = (string)ViewState["period"];
                        int country = (int)ViewState["country"];
                        // Must validate data
                        string cellValue = tb.Text;
                        if (xValue != yValue)
                        {
                            if (cellValue == string.Empty)
                            {
                                cellValue = "0";
                            }
                            // Must also filter other red areas. Also save zero values
                            saveData(period, 1, country, xValue, yValue, cellValue);
                        }

                    }
                }


            }
        }

        private void saveData(string period, int formID, int countryId, string xValue, string yValue, string value)
        {
            SqlConnection connection = new SqlConnection(dbConnection);
            string saveQuery = string.Format(@"IF EXISTS(SELECT * FROM[PCM].[dbo].[FormData] WHERE[FormId] = '{0}' and [CountryId] = '{1}' and [Period] = '{2}' and [X] = '{3}' and Y = '{4}')
                                                    UPDATE[PCM].[dbo].[FormData]
                                                        SET Value = {5}
                                                        WHERE[FormId] = '{0}' and [CountryId] = '{1}' and[Period] = '{2}' and [X] = '{3}' and Y = '{4}';
                                                    Else
                                                        INSERT INTO[PCM].[dbo].[FormData](FormId, CountryId, Period, X, Y, Value)
                                                        VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');", formID, countryId, period, xValue, yValue, value);
            SqlCommand command = new SqlCommand(saveQuery, connection);
            command.Connection.Open();
            command.ExecuteNonQuery();
            connection.Close();

            // LabelSave.Text += "Save Line: Period:" + period + " countryId: " + countryId.ToString() + " xvalue: " + xValue + " yvalue: " + yValue + " value: " + value + Environment.NewLine; 


        }

        protected void Grid_PreRender(object sender, EventArgs e)
        {
            GridView grid = (GridView)sender;
            if (grid.Rows.Count > 0)
            {
                for (int rowIndex = grid.Rows.Count - 2; rowIndex >= 0; rowIndex--)
                {
                    GridViewRow row = grid.Rows[rowIndex];
                    GridViewRow previousRow = grid.Rows[rowIndex + 1];

                    for (int i = 0; i < row.Cells.Count; i++)
                    {
                        // Might include html formatting to replace &nbsp; reference
                        if (row.Cells[i].Text == previousRow.Cells[i].Text && !string.IsNullOrEmpty(row.Cells[i].Text.Trim()) && row.Cells[i].Text != "&nbsp;" && !Extension.IsNumeric(row.Cells[i].Text))
                        {
                            row.Cells[i].RowSpan = previousRow.Cells[i].RowSpan < 2 ? 2 : previousRow.Cells[i].RowSpan + 1;
                            previousRow.Cells[i].Visible = false;
                        }
                    }
                }

                for (int i = 0; i < grid.Rows.Count; i++)
                {
                    GridViewRow cr = grid.Rows[i];

                    for (int j = 0; j < grid.Columns.Count; j++)
                    {
                        // First row
                        if (i == 0)
                        {
                            if (j == 0)
                            {
                                cr.Cells[j].CssClass = "d2";
                            }
                            else
                            {
                                cr.Cells[j].CssClass = "d3";
                            }
                        }
                        else if (i == 1)
                        {
                            if (j == 0)
                            {
                                cr.Cells[j].CssClass = "d3";
                            }
                            else
                            {
                                if (cr.Cells[j].HasControls())
                                {
                                    TextBox tb = (TextBox)cr.Cells[j].Controls[0];
                                    if (tb.Attributes["readonly"] == "readonly" || tb.ReadOnly)
                                    {
                                        cr.Cells[j].CssClass = "staProduct";
                                       // tb.ToolTip = tb.Text;
                                    }
                                    else
                                    {
                                        cr.Cells[j].CssClass = "flowvalue";
                                    }
                                }
                                else
                                {

                                    cr.Cells[j].CssClass = "staProduct";
                                }
                            }
                        }
                        else if (i >= 2)
                        {
                            if (j == 0)
                            {
                                cr.Cells[j].CssClass = "d3";
                            }
                            else
                            {
                                if (cr.Cells[j].HasControls())
                                {
                                    TextBox tb = (TextBox)cr.Cells[j].Controls[0];
                                    if (tb.Attributes["readonly"] == "readonly" || tb.ReadOnly)
                                    {
                                        cr.Cells[j].CssClass = "sumvalue";
                                    }
                                    else
                                    {
                                        cr.Cells[j].CssClass = "flowvalue";
                                    }
                                }
                                else
                                {
                                    cr.Cells[j].CssClass = "sumvalue";
                                }
                            }
                        }
                    }
                }
            }
        }

        protected void btnSaveData_Click(object sender, EventArgs e)
        {
            Button thisButt = (Button)sender;
            GridView thisGrid; 
            int formID;

            switch (thisButt.ID)
            {
                case "btnSaveAH":
                    thisGrid = AHGrid;
                    formID = 2;
                    break;
                case "btnSaveSurgery":
                    thisGrid = SurgeryGrid;
                    formID = 4;
                    break;
                case "btnSaveFactor":
                    thisGrid = FactorGrid;
                    formID = 5;
                    break;
                case "btnSaveThrombosis":
                    thisGrid = thrombosisGrid;
                    formID = 6;
                    break;
                case "btnSaveAgeSplit":
                    thisGrid = AgeSplitGrid;
                    formID = 7;
                    break;
                case "btnSavePatients":
                    thisGrid = PatientsGrid;
                    formID = 8;
                    break;
                default:
                    thisGrid = new GridView();
                    formID = 0;
                    break;
            }

            int rowCount = thisGrid.Rows.Count;
            int colCount = thisGrid.Columns.Count;

            string period = (string)ViewState["period"];
            int country = (int)ViewState["country"];

            for (int i = 1; i < rowCount; i++)
            {
                
                GridViewRow grow = thisGrid.Rows[i];

                for (int j = 0; j < colCount; j++)
                {
                    if (grow.Cells[j].HasControls())
                    {
                        TextBox tb = (TextBox)grow.Cells[j].Controls[0];
                        // Only save values for cells that are not read-only

                        // get referece to the product x and y id
                        string xValue = thisGrid.HeaderRow.Cells[j].Text;
                        string yValue = thisGrid.Rows[i].Cells[0].Text;
                       
                        // Must validate data
                        string cellValue = tb.Text;

                       
                        if (tb.ReadOnly == false && tb.Attributes["readonly"] != "readonly")
                        {
                            if (cellValue == string.Empty)
                            {
                                cellValue = "0";
                            }
                            saveData(period, formID, country, xValue, yValue, cellValue);

                        }

                    }
                }
            }
        }

        protected void countriesDdl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (countriesDdl.SelectedValue != "NA")
            {
                string sql = string.Format("SELECT [ReportingInterval] FROM[PCM].[dbo].[Countries] Where id = {0}", countriesDdl.SelectedValue);
                string reportingInterval = string.Empty;

                using (SqlConnection con = new SqlConnection(dbConnection))
                {
                    SqlCommand cmd = new SqlCommand(sql, con);
                    con.Open();
                    reportingInterval = (string)cmd.ExecuteScalar();
                    con.Close();
                }

                if (reportingInterval == "Quarterly")
                {
                    for (int i = 0; i < periodsDdl.Items.Count; i++)
                    {
                        if (int.Parse(periodsDdl.Items[i].Value.Substring(4, 2)) % 3 != 0)
                        {
                            periodsDdl.Items[i].Enabled = false;
                            //periodsDdl.Items[i].Attributes.Add("disabled", "disabled");
                        }
                        else
                        {
                            // Change the format to correspont to quarter data
                            int quarter = int.Parse(periodsDdl.Items[i].Value.Substring(4, 2)) / 3;
                            periodsDdl.Items[i].Text = quarter + "Q-" + periodsDdl.Items[i].Value.Substring(0, 4);
                        }


                    }
                }
                else
                {
                    for (int i = 0; i < periodsDdl.Items.Count; i++)
                    {
                        periodsDdl.Items[i].Enabled = true;
                        // Change the format to original format
                        periodsDdl.Items[i].Text = DateTime.ParseExact(periodsDdl.Items[i].Value, "yyyyMM", null).ToString("MMM-yyyy", CultureInfo.InvariantCulture);
                        // periodsDdl.Items[i].Attributes.Remove("disabled");

                    }
                }

                // Add period selection to viewstate
                ViewState.Add("reportinginterval", reportingInterval);
            }
        }              

        protected void btnSaveComments_Click(object sender, EventArgs e)
        {
            // Save Comments
            if (!string.IsNullOrEmpty(txtComments.Text))
            {
                string period = (string)ViewState["period"];
                int countryId = (int)ViewState["country"];
                string textValue = txtComments.Text;

                SqlConnection connection = new SqlConnection(dbConnection);
                string saveQuery = string.Format(@"IF EXISTS(SELECT * FROM[PCM].[dbo].[Comments] WHERE [CountryId] = '{0}' and [Period] = '{1}')
                                                    UPDATE[PCM].[dbo].[Comments]
                                                        SET Comment = '{2}'
                                                        WHERE [CountryId] = '{0}' and [Period] = '{1}';
                                                    Else
                                                        INSERT INTO[PCM].[dbo].[Comments](CountryId, Period, Comment)
                                                        VALUES('{0}', '{1}', '{2}');", countryId, period, textValue);
                SqlCommand command = new SqlCommand(saveQuery, connection);
                command.Connection.Open();
                command.ExecuteNonQuery();
                connection.Close();


            }
        }
    }

    public static class Extension
    {
        public static bool IsNumeric(this string s)
        {
            float output;
            return float.TryParse(s, out output);
        }
    }

    public class CreateItemTemplate : ITemplate
    {

        //Field to store the ListItemType value
        private ListItemType myListItemType;
        string _columnName;
        int _rowOffset;
        int _rowMax;

        public CreateItemTemplate()
        {
            //
            // TODO: Add default constructor logic here
            //
        }

        //Parameterrised constructor
        
        public CreateItemTemplate(ListItemType Item, string colname, int rowOffset, int rowMax)
        {
            myListItemType = Item;
            _columnName = colname;
            _rowOffset = rowOffset;
            _rowMax = rowMax;
        }

        //Overwrite the InstantiateIn() function of the ITemplate interface.
        public void InstantiateIn(System.Web.UI.Control container)
        {
            if (myListItemType == ListItemType.Header)
            {
                Label lbl = new Label();
                lbl.Text = _columnName;             //Assigns the name of the column in the lable.<o:p>
                container.Controls.Add(lbl);
            }//Adds the newly created label control to the container.<o:p>

            //Code to create the ItemTemplate and its field.
            if (myListItemType == ListItemType.Item)
            {
                if (_columnName != "end" && _columnName != "netflow")
                {
                    // Style textbox to look like a label
                    TextBox txtProduct = new TextBox();
                    txtProduct.DataBinding += new EventHandler(tb1_DataBinding);
                    txtProduct.CssClass = "formText";
                    txtProduct.Attributes.Add("productId", _columnName);
                    txtProduct.BackColor = System.Drawing.Color.Transparent;
                    txtProduct.BorderStyle = BorderStyle.None;
                    txtProduct.BorderWidth = 0;
                    txtProduct.Width = Unit.Percentage(99);
                    container.Controls.Add(txtProduct);//Unit.Pixel(75);
                }
                else
                {
                    TextBox txtProduct = new TextBox();
                    txtProduct.DataBinding += new EventHandler(tb1_DataBinding);
                    txtProduct.CssClass = "sumColumnn";                  
                    txtProduct.BackColor = System.Drawing.Color.Transparent;
                    txtProduct.BorderStyle = BorderStyle.None;
                    txtProduct.BorderWidth = 0;
                    txtProduct.Width = Unit.Percentage(99);
                    container.Controls.Add(txtProduct);//Unit.Pixel(75);
                }
                //txtCashCheque.Columns
                //txtCashCheque.ReadOnly = true;
               // container.Controls.Add(txtProduct);
            }
        }
        void tb1_DataBinding(object sender, EventArgs e)
        {           
            TextBox txtdata = (TextBox)sender;
            GridViewRow container = (GridViewRow)txtdata.NamingContainer;
            object dataValue = DataBinder.Eval(container.DataItem, _columnName);

            // Index is noOfProducts + 2 extra lines + 3 headerlines -1 for index            
            //if (container.RowIndex < 3 || container.RowIndex > (_noOfProducts + 4  ))
            if (container.RowIndex < _rowOffset)
            {
                txtdata.ReadOnly = true;
              //  txtdata.ToolTip = dataValue.ToString();
               // txtdata.ReadOnly = false;
               // txtdata.Attributes.Add("readonly", "readonly");
            }
            else if(container.RowIndex > (_rowMax))
            {
                txtdata.ReadOnly = false;
                txtdata.Attributes.Add("readonly", "readonly");
                txtdata.Attributes["productId"] += "-total";               
                txtdata.TabIndex = -1;

            }
            else if(_columnName.Contains("ytd-"))
            {
                txtdata.ReadOnly = false;              
                txtdata.Attributes.Add("readonly", "readonly");
                // Prevent tabindex
                txtdata.TabIndex = -1;               
            }
            else if (_columnName == "end" || _columnName == "netflow")
            {
                txtdata.ReadOnly = false;
                txtdata.Attributes.Add("readonly", "readonly");               
                txtdata.TabIndex = -1;
                if (string.IsNullOrEmpty(txtdata.Text))
                {
                    txtdata.Text = "0";
                }
            }

           /* else if (_columnName.Contains("nertflow"))
            {
                txtdata.ReadOnly = false;
                txtdata.Attributes.Add("readonly", "readonly");
                // Prevent tabindex
                txtdata.TabIndex = -1;
            }*/

            if (dataValue != DBNull.Value)
            {
                txtdata.Text = dataValue.ToString();
            }
    }

    }

    public class ProductGroup
    {
        public string productGroup { get; set; }
        public string productId { get; set; }
    }
}
