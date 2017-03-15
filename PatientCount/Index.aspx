﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="PatientCount.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>NovoSeven&reg; Patient Estimation Model</title>
<meta http-equiv="X-UA-Compatible" content="IE=edge" />

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
	
   

</head>
<body>

  
     <form id="form1" runat="server">

          
            <nav class="navbar navbar-default navbar-top" style="margin-bottom:2px">
                    <div class="container">
                        <div class="navbar-header">             
                         <a class="navbar-brand" href="#"><img src="/img/nn.png" style="height: 50px;float:left;margin-top:-17px"/>   NovoSeven&reg; Patient Estimation</a>
                         </div>                   
                        <div id="navbar" class="navbar-collapse collapse">
                            <ul class="nav navbar-nav">                             
                                <li class="active"><a href="Index.html">Home</a></li>
                                <li ><a href="MonthlyInput.aspx">Input</a></li>
                                <li ><a href="Analytics/index.html">Reports</a></li>                                    
                            </ul>
                            <ul class="nav navbar-nav navbar-right">   
                                 <li> <%=administrationLink %>   </li>                         
                            </ul>
                        </div>                  
                    </div>             
            </nav>
        <div class="container"> 
           
                       <div class="alert alert-danger" id="message" style="display:none"></div>

                        
                  
            <section  >
                <div class="row">
			        <h1 class="page-header" id="usersSection">Patient Estimatation Model</h1>
                    
                </div>
                <div class="row">
                    <div class="col-xs-8">
Welcome to the new patient estimation model which is owned by the NovoSeven® global brand team.  The main strategic goal is to provide both the global HQ and affiliates to estimate and track patient trend, allowing analysis that guides business decisions. 
<br /> 

This new patient estimation model has been developed to support the following; 
<ul>
<li>Patient estimates are entered once a quarter basis across all licensed and approved indications for NovoSeven® per-country* 
</li>
<li>Submission deadlines for data are set as quarterly. 

</li>
<li>The number of patients at the end of quarter becomes the number of the patients at the start of next quarter

</li>
<li>The patient estimation model will also automatically generate a number of standard graphs. Plus a quarterly report can be downloaded as a Power Point Presentation
</li>
<li>Additional reports and data analysis can also be accessed via the Qlik Sense® partnership
</li>
<li>For additional support please download the following “NovoSeven® Patient Estimation Model Training toolkit”
</li>
</ul>
<div class="page-header"><h1><small>Input</small> <h1></div>
As an approved user for the NovoSeven® - Patient Estimation Model you are request to please input your data quarterly/monthly by the 20th
<ul>
<li>
Select the country/ month/ year of your input. Please put the latest month in the quarter
</li>
<li>Please ensure that you save each input per indication before you close the page or move tabs 
</li>
</ul>
<div class="page-header"><h1><small>Administration</small> <h1></div>

For assistance, training and additional support please contact TKHE and SKEY

                    </div>
                    <div class="col-xs-4">
                    <img src="/img/NovoSevenLogo.jpg"/>
<div class="page-header"><h1><small>Introduction Video</small> <h1></div>
                   <iframe  src="https://www.youtube.com/embed/IyI6dxWrWcc" frameborder="0" allowfullscreen></iframe>
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
         </form>
     <script >
        $.urlParam = function (name) {
            var results = new RegExp('[\?&]' + name + '=([^&#]*)').exec(window.location.href);
            return results[1] || 0;
        }
        // Get URL
        var url = window.location.href;
        // Check if URL contains the keyword
        if (url.search('message') > 0) {
            // Display the message
            $('#message').show();
            $('#message').html('<strong>Warning!</strong>' + ($.urlParam('message')));
        }
    </script>
</body>
</html>

