<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LivinInOutDetailsPrint.aspx.cs" Inherits="POR.Report.LivinInOut.LivinInOutDetailsPrint" %>
<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>

<body>
     <form id="form1" runat="server">
         <div>             
            <rsweb:reportviewer id="ReportViewerLivinInOut" runat="server" font-names="Verdana" font-size="8pt" height="400px" waitmessagefont-names="Verdana" waitmessagefont-size="14pt" width="1000px">
           
                 <LocalReport ReportPath="Report\LivinInOut\LivinInOutDetailsPrint.rdlc">
            </LocalReport>
        </rsweb:reportviewer>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
        </div>
    </form>

</body>

</html>
