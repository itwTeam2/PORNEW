<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Forwerd.aspx.cs" Inherits="POR.Report.Forwerd" %>

<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div style="width: 1365px; height: 792px">
    
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" Font-Names="Verdana" Font-Size="8pt" Height="783px" WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt" Width="1366px">
            <LocalReport ReportEmbeddedResource="POR.Report.ForwerdList.rdlc">
                <DataSources>
                    <rsweb:ReportDataSource DataSourceId="ObjectDataSource1" Name="DataSet1" />
                </DataSources>
            </LocalReport>
        </rsweb:ReportViewer>
        <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" SelectMethod="GetData" TypeName="Forwerd_DSTableAdapters.Vw_FixedAllowance_FLowStatusTableAdapter" OnSelecting="ObjectDataSource1_Selecting"></asp:ObjectDataSource>
    
    </div>
    </form>
</body>
</html>
