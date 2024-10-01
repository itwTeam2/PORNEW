<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FixedAllowanceEligibleList.aspx.cs" Inherits="POR.Report.FixedAllowanceEligibleList" %>

<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
        <style type="text/css">
        #form1 {
            height: 400px;
            width: 1220px;
            margin-left:auto;
            margin-right:auto;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>    
        <rsweb:ReportViewer ID="RV_FAEligibleList" runat="server" Height="600px" Width="1220px" style="margin-left: 0px">
        </rsweb:ReportViewer>    
    </div>
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
    </form>
</body>
</html>
