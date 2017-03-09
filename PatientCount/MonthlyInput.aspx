<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MonthlyInput.aspx.cs" Inherits="PatientCount.MonthlyInput" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

    <meta http-equiv="X-UA-Compatible" content="IE=edge"/>
    <title>NovoSeven&reg; Patient Estimation Model</title>

      <link href="css/bootstrap.css" rel="stylesheet" />
      <link href="css/PatientCount.css" rel="stylesheet" />
    <!--<link rel="stylesheet" type="text/css" href="css/editor.dataTables.min.css"/>
    <link rel="stylesheet" type="text/css" href="https://cdn.datatables.net/1.10.12/css/jquery.dataTables.min.css"/>
	<link rel="stylesheet" type="text/css" href="https://cdn.datatables.net/buttons/1.2.0/css/buttons.dataTables.min.css"/>
	<link rel="stylesheet" type="text/css" href="https://cdn.datatables.net/select/1.2.0/css/select.dataTables.min.css"/>-->

     <script src="js/jquery-2.1.4.js"></script> 
     <script src="js/bootstrap.min.js"></script>
    <!--
    <script type="text/javascript" src="https://cdn.datatables.net/1.10.12/js/jquery.dataTables.min.js"></script>	
	<script type="text/javascript" src="https://cdn.datatables.net/buttons/1.2.0/js/dataTables.buttons.min.js"></script>	
	<script type="text/javascript" src="https://cdn.datatables.net/select/1.2.0/js/dataTables.select.min.js"></script>	
    <script type="text/javascript" src="js/dataTables.editor.min.js"></script>
	-->
    <script>
        $(document).ready(function () {

            $("#MontlyInputGrid").on('input', '.formText', function () {

                //________________ X Product Calc___________________

                // Get reference to selected x Product
                var getXProduct = $(this).attr('productId');
                var xSum = 0;
                var YSum = 0;

                if (!isNaN(getXProduct)) {

                    //console.log(getXProduct);

                    $('#MontlyInputGrid input[productId=' + getXProduct + ']').each(function (index) {
                        //Check if number is not empty                
                        if ($.trim($(this).val()) != "") {
                            //Check if number is a valid integer                    
                            if (!isNaN($(this).val())) {
                                xSum = xSum + parseFloat($(this).val());
                            }
                        }
                    });

                    // Find row where xProduct is
                    var yRow = $('#MontlyInputGrid td:first-child').filter(function () {
                        return $(this).text() === getXProduct
                    }).closest("tr");

                    //Code to get the Sum of the row
                    $(".formText", yRow).each(function (index) {
                        //Check if number is not empty                
                        if ($.trim($(this).val()) != "") {
                            //Check if number is a valid integer                    
                            if (!isNaN($(this).val())) {
                                YSum = YSum + parseFloat($(this).val());
                            }
                        }
                    });


                    // Update New Flow
                    var netFlow = xSum - YSum;
                    var currentValue = parseInt($(yRow).find('td:nth-last-child(2)').text());
                    $(yRow).find('td:nth-last-child(2)').html(netFlow);
                    // Get Start Values
                    //console.log($(yRow).find('td.startCol').text());
                    var startValue = parseInt($(yRow).find('td.startCol').text());
                    var endValue = startValue + netFlow;
                    $(yRow).find('td:last-child').html(endValue);

                    // Update ProductGroup. Find associated productgroup, find all rows where product group match and use last index to get the actual sum row.
                    // Use the netflow value together with existing netflow value for product and existing product group value to calculate new product group value

                    // Get reference to associated productGroup
                    var xGroup = $(yRow).find('td:nth-child(2)').text();
                    // Find the rows matching the productgroupid
                    var xGroupRow = $('#MontlyInputGrid td:nth-child(2).productGroup').filter(function () {
                        return $(this).text() === xGroup
                    }).closest("tr");

                    // Get the current value of the associated group
                    var currentGroupValue = parseInt($(xGroupRow).last().find('td:nth-last-child(2)').text());
                    var groupValue = currentGroupValue + (netFlow - currentValue);
                    //console.log(groupValue);

                    // Update netflow
                    $(xGroupRow).last().find('td:nth-last-child(2)').html(groupValue);

                    // Update End
                    $(xGroupRow).last().find('td:last-child').html((netFlow - currentValue) + parseInt($(xGroupRow).last().find('td:last-child').text()));
                }

                // Might be issues updating when x and y are the same product group. 

                //________________ Y Product Calc___________________

                var thisRow = $(this).closest("tr");
                var yProduct = $(thisRow).find('td:first-child').text();

                var yProductXSum = 0;
                var yProductYSum = 0;

                //Get Initial y-group values
                var YcurrentValueNetFlow = parseInt($(thisRow).find('td:nth-last-child(2)').text());

                $(".formText", thisRow).each(function (index) {
                    //Check if number is not empty                
                    if ($.trim($(this).val()) != "") {
                        //Check if number is a valid integer                    
                        if (!isNaN($(this).val())) {
                            yProductYSum = yProductYSum + parseFloat($(this).val());
                        }
                    }
                });



                $('#MontlyInputGrid input[productId=' + yProduct + ']').each(function (index) {
                    //Check if number is not empty                
                    if ($.trim($(this).val()) != "") {
                        //Check if number is a valid integer                    
                        if (!isNaN($(this).val())) {
                            yProductXSum = yProductXSum + parseFloat($(this).val());
                        }
                    }
                });

                var YnetFlow;
                if (yProduct < 1000)
                { YnetFlow = yProductXSum - yProductYSum; }
                else
                { YnetFlow = Math.abs(yProductXSum - yProductYSum); }

                $(thisRow).find('td:nth-last-child(2)').html(YnetFlow);
                // Get Start Values               
                var YstartValue = parseInt($(thisRow).find('td.startCol').text());
                var YendValue = YstartValue + YnetFlow;

                //   $(thisRow).find('td:last-child').html(YendValue);

                // Update Product Group
                // Update ProductGroup. Find associated productgroup, find all rows where product group match and use last index to get the actual sum row.
                // Use the netflow value together with existing netflow value for product and existing product group value to calculate new product group value

                // Get reference to associated productGroup
                var yGroup = $(thisRow).find('td:nth-child(2)').text().trim();
                var YgroupValue = 0;

                // Do not update grouprow if yGroup is null or empty referring to 1000
                if (typeof (yGroup) !== "undefined" && yGroup) {
                    // Find the rows matching the productgroupid
                    var yGroupRow = $('#MontlyInputGrid td:nth-child(2).productGroup').filter(function () {
                        return $(this).text() === yGroup
                    }).closest("tr");

                    // Get the current value of the associated group
                    var YcurrentGroupValue = parseInt($(yGroupRow).last().find('td:nth-last-child(2)').text());
                    // Neets                                                     
                    YgroupValue = (YnetFlow - YcurrentValueNetFlow) + YcurrentGroupValue;
                    $(yGroupRow).last().find('td:nth-last-child(2)').html(YgroupValue);
                    $(yGroupRow).last().find('td:last-child').html((YnetFlow - YcurrentValueNetFlow) + parseInt($(yGroupRow).last().find('td:last-child').text()));
                }

                $(thisRow).find('td:last-child').html(YendValue);

                // Update totals col by getting the total's col index 

                var totalGroupValue = 0;
                var totalNetFlowValue = 0;

                var totalRow = $('#MontlyInputGrid td:nth-child(2).productGroup').filter(function () {
                    return $(this).text() === '0'
                }).closest("tr");

                var nextGroupRows = totalRow.nextAll();
                nextGroupRows.each(function (index) {

                    var myvalFlow = parseInt($(this).find('td:nth-last-child(2)').text());
                    var myvalTotal = parseInt($(this).find('td:last-child').text());
                    totalNetFlowValue += myvalFlow;
                    totalGroupValue += myvalTotal;
                });

                totalRow.last().find('td:nth-last-child(2)').html(totalNetFlowValue);
                totalRow.last().find('td:last-child').html(totalGroupValue);

            });

            $("#MontlyInputGrid td:nth-child(1)").hide();
            $("#MontlyInputGrid td:nth-child(2)").hide();  

            // Functions for calculating YTD Functionality
            $("#AHGrid, #SurgeryGrid").on('input', '.formText', function () {
              
                // Get current prodID
                var prod = $(this).attr('productId');
                var value = 0;
                var ytdValue = 0;
                var existingValue = 0;

                if ($.trim($(this).attr("value")) != "") {
                    //Check if number is a valid integer                    
                    if (!isNaN($(this).attr("value"))) {
                        existingValue = parseFloat($(this).attr("value"));
                    }
                }


                // Get inserted value
                if ($.trim($(this).val()) != "") {
                    //Check if number is a valid integer                    
                    if (!isNaN($(this).val())) {
                        value = parseFloat($(this).val());
                    }
                }

                var txt = $(this).closest("tr").find('input[productId="ytd-' + prod + '"]');
                
                // Use the aattribute to access existing values
                if ($.trim($(txt).attr("value")) != "") {
                    //Check if number is a valid integer                    
                    if (!isNaN($(txt).attr("value"))) {
                        ytdValue = parseFloat($(txt).attr("value"));
                    }
                }

                console.log(ytdValue);
                console.log(value);
                console.log(existingValue);

                $(this).closest("tr").find('input[productId="ytd-' + prod + '"]').val(ytdValue + value - existingValue);
                if ($(this).closest("tr").find('input[productId="ytd-' + prod + '"]').closest('table').attr('id') == "SurgeryGrid") {
                    $(this).closest("tr").find('input[productId="ytd-' + prod + '"]').updateTotal();
                }
              
            });

            $.fn.updateTotal = function () {

                var prod = $(this).attr('productId');
                var grid = $(this).closest('table').attr('id');
                console.log(grid);
                var colSum = 0;
                
                $('#' + grid + ' input[productId=' + prod + ']').each(function (index) {
                    //Check if number is not empty                
                    if ($.trim($(this).val()) != "") {
                        //Check if number is a valid integer                    
                        if (!isNaN($(this).val())) {
                            colSum = colSum + parseFloat($(this).val());
                        }
                    }
                });

                // Find textbox where total value is stored
                $('#' + grid + ' input[productId=' + prod + '-total]').val(colSum);
            };
           
            $("#SurgeryGrid, #AgeSplitGrid").on('input', '.formText', function () {

                $(this).updateTotal();
            });

            // Event for activating save button for active Grid.
            $(".gridLayout").on('change', '.formText', function () {

                var gridId = $(this).closest(".gridLayout").attr('id');
                var btn = $("[GridID='" + gridId + "']");

                $(btn).removeClass('disabled');
                $(btn).addClass('btn-primary');
            });

            $("#txtComments").on('change', function () {
                
                var btn = $("[GridID='txtComments']");

                $(btn).removeClass('disabled');
                $(btn).addClass('btn-primary');
            });

            // $('.formText').keyup(function () {
            $(".formText").on('input', function () {
                if (this.value != this.value.replace(/[^0-9]/g, '')) {
                    this.value = this.value.replace(/[^0-9]/g, '');
                }
            });

            // Set tooltip on products
            $("td.staProduct Input").hover(function () {
               // $(this).attr('title', $(this).val());
                $(this).tooltip({ 'trigger': 'hover', 'title': $(this).val() });
            });
           
           
        });               

      

    </script>

</head>
<body>

       <nav class="navbar navbar-default navbar-top" style="margin-bottom:2px">
                    <div class="container">
                        <div class="navbar-header">                         
                             <a class="navbar-brand" href="#">NovoSeven&reg; Patient Count</a>
                         </div>                   
                        <div id="navbar" class="navbar-collapse collapse">
                            <ul class="nav navbar-nav">                             
                                <li ><a href="Index.html">Home</a></li>
                                <li class="active" ><a href="MonthlyInput.aspx">Input</a></li>
                                <li ><a href="Analytics/index.html">Reports</a></li>                                    
                            </ul>
                            <ul class="nav navbar-nav navbar-right">                               
                                <li ><a href="administration.aspx">Administration</a></li>                            
                            </ul>
                        </div>                  
                    </div>             
            </nav>

       <div class="container"> 

         <form id="form1" runat="server">        
        <!-- Selection bar -->
         
         <div class="row">
            <div>
                   <div class="col-md-6">  
                       <h4>
                          <asp:Label ID="inputLabel" runat="server" Text="Monthly Input"></asp:Label>   
                       </h4>                   
                   </div> 
                   <div class="col-md-6"> 
                       <div class="pull-right">
                           <label>Country:</label>
                           <asp:DropDownList CssClass="btn btn-default" style="margin-right:20px" AutoPostBack="true" OnSelectedIndexChanged="countriesDdl_SelectedIndexChanged" ID="countriesDdl" runat="server"></asp:DropDownList>
                           <label>Period:</label>
                           <asp:DropDownList CssClass="btn btn-default" style="margin-right:20px" ID="periodsDdl" runat="server"></asp:DropDownList>
                           <asp:Button class="btn btn-default" OnClick="selectBtn_Click" ID="selectBtn" runat="server" Text="Select" />                          
                       </div>
                  </div> 
                </div>   
        </div>

         <div class="hr"></div>
             
          <div id="formArea" runat="server">

          <!-- CH -->
                       
          <div class="row">
                    <div class="col-md-12">                                         
                       <asp:GridView  CssClass="table table-bordered table-condensed gridLayout" Font-Size="10" ShowHeader="false" OnRowDataBound="MontlyInputGrid_RowDataBound"  OnPreRender="MontlyInputGrid_PreRender"  AutoGenerateColumns="False" ID="MontlyInputGrid" runat="server">
                           <Columns>   
                                <asp:BoundField DataField="id" ReadOnly="true"  HeaderText="id" SortExpression="id" />
                                <asp:BoundField DataField="ProductGroupId" ItemStyle-CssClass="productGroup" ReadOnly="true"  HeaderText="id" SortExpression="id" />                                                                                 
                                <asp:BoundField DataField="ProductType" ItemStyle-Width="85" ItemStyle-HorizontalAlign="Left" ReadOnly="true" HeaderText="Category" />
                                <asp:BoundField DataField="Product" ItemStyle-Width="175" ItemStyle-HorizontalAlign="Left" ReadOnly="true"  HeaderText="Product" SortExpression="Value" />                                      
                                <asp:BoundField DataField="Value" ItemStyle-Width="35" ItemStyle-CssClass="startCol" ReadOnly="true"  HeaderText="Value" SortExpression="Value" />
                               
                            </Columns>                    
                       </asp:GridView>                   
                </div>
          </div>
            
          <div class="row">
                <div class="col-md-12">
                    <div class="pull-right">
                        <asp:Button class="btn btn-default disabled" OnClick="saveBtn_Click" GridID="MontlyInputGrid" ID="saveBtn" runat="server" Text="Save CH" />                       
                     </div>
                </div>
           </div>

          <div class="hr"></div>

          <!-- PatientsForm -->
          <div class="row">
                  <div class="col-md-12">                                
                       <asp:GridView  CssClass="table table-bordered table-condensed gridLayout" Font-Size="10" ShowHeader="false" OnPreRender="Grid_PreRender" OnRowDataBound="MontlyInputGrid_RowDataBound"  AutoGenerateColumns="False" ID="PatientsGrid" runat="server">
                           <Columns>   
                                <asp:BoundField ItemStyle-Width="295px" DataField="Name" ReadOnly="true"  HeaderText="label" SortExpression="label" /> 
                            </Columns>                    
                       </asp:GridView>                   
                </div>
          </div>

          <div class="row">
                <div class="col-md-12">
                    <div class="pull-right">
                        <asp:Button class="btn btn-default disabled" OnClick="btnSaveData_Click" GridID="PatientsGrid" ID="btnSavePatients" runat="server" Text="Save Pat" />                       
                     </div>
                </div>
           </div>
          
           <div class="hr"></div>

           <!-- AH -->
          <div class="row">
                  <div class="col-md-12">                                
                       <asp:GridView  CssClass="table table-bordered table-condensed gridLayout" Font-Size="10" ShowHeader="false" OnPreRender="Grid_PreRender" OnRowDataBound="MontlyInputGrid_RowDataBound"  AutoGenerateColumns="False" ID="AHGrid" runat="server">
                           <Columns>   
                                <asp:BoundField ItemStyle-Width="295px" DataField="label" ReadOnly="true"  HeaderText="label" SortExpression="label" /> 
                            </Columns>                    
                       </asp:GridView>                   
                </div>
          </div>

          <div class="row">
                <div class="col-md-12">
                    <div class="pull-right">
                        <asp:Button class="btn btn-default disabled" OnClick="btnSaveData_Click" GridID="AHGrid" ID="btnSaveAH" runat="server" Text="Save AH" />                       
                     </div>
                </div>
           </div>
          
           <div class="hr"></div>
              
           <!-- Surgery -->   

          <div class="row">
                  <div class="col-md-12">                                
                       <asp:GridView  CssClass="table table-bordered table-condensed gridLayout" Font-Size="10" ShowHeader="false" OnPreRender="Grid_PreRender" OnRowDataBound="MontlyInputGrid_RowDataBound"  AutoGenerateColumns="False" ID="SurgeryGrid" runat="server">
                           <Columns>   
                                <asp:BoundField ItemStyle-Width="295px" DataField="label" ReadOnly="true"  HeaderText="label" SortExpression="label" /> 
                            </Columns>                    
                       </asp:GridView>                   
                </div>
          </div>

          <div class="row">
                <div class="col-md-12">
                    <div class="pull-right">
                        <asp:Button class="btn btn-default disabled" OnClick="btnSaveData_Click" GridID="SurgeryGrid" ID="btnSaveSurgery" runat="server" Text="Save SH" />                       
                     </div>
                </div>
           </div>

          <div class="hr"></div>
              
           <!-- Factor7Deficiency -->   

          <div class="row">
                  <div class="col-md-12">                                
                       <asp:GridView  CssClass="table table-bordered table-condensed gridLayout" Font-Size="10" ShowHeader="false" OnPreRender="Grid_PreRender" OnRowDataBound="MontlyInputGrid_RowDataBound"  AutoGenerateColumns="False" ID="FactorGrid" runat="server">
                           <Columns>   
                                <asp:BoundField ItemStyle-Width="295px" DataField="label" ReadOnly="true"  HeaderText="label" SortExpression="label" /> 
                            </Columns>                    
                       </asp:GridView>                   
                </div>
          </div>

          <div class="row">
                <div class="col-md-12">
                    <div class="pull-right">
                        <asp:Button class="btn btn-default disabled" OnClick="btnSaveData_Click" GridID="FactorGrid" ID="btnSaveFactor" runat="server" Text="Save Factor" />                       
                     </div>
                </div>
           </div>

          <!-- Glanzmann's thrombasthenia -->   

          <div class="row">
                  <div class="col-md-12">                                
                       <asp:GridView  CssClass="table table-bordered table-condensed gridLayout" Font-Size="10" ShowHeader="false" OnPreRender="Grid_PreRender" OnRowDataBound="MontlyInputGrid_RowDataBound"  AutoGenerateColumns="False" ID="thrombosisGrid" runat="server">
                           <Columns>   
                                <asp:BoundField DataField="label" ItemStyle-Width="295px" ReadOnly="true"  HeaderText="label" SortExpression="label" /> 
                            </Columns>                    
                       </asp:GridView>                   
                </div>
          </div>

          <div class="row">
                <div class="col-md-12">
                    <div class="pull-right">
                        <asp:Button class="btn btn-default disabled" OnClick="btnSaveData_Click" GridID="thrombosisGrid" ID="btnSaveThrombosis" runat="server" Text="Save GT" />                       
                     </div>
                </div>
           </div>

          <!-- CH Age Split -->   

          <div class="row">
                  <div class="col-md-12">                                
                       <asp:GridView  CssClass="table table-bordered table-condensed gridLayout" Font-Size="10" ShowHeader="false" OnPreRender="Grid_PreRender" OnRowDataBound="MontlyInputGrid_RowDataBound"  AutoGenerateColumns="False" ID="AgeSplitGrid" runat="server">
                           <Columns>   
                                <asp:BoundField DataField="label" ItemStyle-Width="295px" ReadOnly="true"  HeaderText="label" SortExpression="label" /> 
                            </Columns>                    
                       </asp:GridView>                   
                </div>
          </div>

          <div class="row">
                <div class="col-md-12">
                    <div class="pull-right">
                        <asp:Button class="btn btn-default disabled" OnClick="btnSaveData_Click" GridID="AgeSplitGrid" ID="btnSaveAgeSplit" runat="server" Text="Save AS" />                       
                     </div>
                </div>
           </div>

         <!-- Comments -->
           <div class="row">
                  <div class="col-md-12">  
                      <div class="d2" style="width:295px;padding:5px;font-size:10pt;">
                            Comments
                       </div>
                  </div>                  
            </div>
           <div class="row">
               <div class="col-md-12">  
                      <asp:TextBox ID="txtComments" CssClass="flowvalue" Width="100%" Height="100px" TextMode="MultiLine"  runat="server"></asp:TextBox>
                  </div>
            </div>

           <div class="row">
                <div class="col-md-12">
                    <div class="pull-right">
                        <asp:Button class="btn btn-default disabled" OnClick="btnSaveComments_Click" GridID="txtComments" ID="btnSaveComments" runat="server" Text="Save Comments" />                       
                     </div>
                </div>
           </div>

           </div>

         </form>
   
        </div>
        <div class="container">

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
