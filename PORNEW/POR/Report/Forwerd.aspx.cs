using Microsoft.Reporting.WebForms;
using ReportData.DAL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace POR.Report
{
    public partial class Forwerd : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                DALCommanQuery objDALCommanQuery = new DALCommanQuery();

                Forwerd_DS ds1 = new Forwerd_DS();

                string FromDate_ = Request.Params["FromDate"].ToString();
                string ToDate_ = Request.Params["ToDate"].ToString();
                string UID =  Request.Params["UID"].ToString();
                string Location =  Request.Params["Location"].ToString();               

                var connectionString = ConfigurationManager.ConnectionStrings["PORConnectionString"].ConnectionString;
                SqlConnection conx = new SqlConnection(connectionString);
                string select = " SELECT ServiceNo, Rank, Name, AllowanceName, CreatedDate, EffectiveDate,EndDate,CampAuthority,RoleName  from "
                              + " Vw_FixedAllowanceDetail where CreatedDate between '" + FromDate_ + "' and '" + ToDate_ + "' and FADFS_CreatedBy='" + UID + "' ";
               
                 SqlDataAdapter adp = new SqlDataAdapter(select, conx);
                 adp.Fill(ds1, "Vw_FixedAllowance_FLowStatus");
                 ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Report/ForwerdList.rdlc");
                 ReportViewer1.LocalReport.DataSources.Clear();

                 ReportDataSource rdc = new ReportDataSource("Forwerd_DS", ds1.Tables[0]);

                 ReportViewer1.LocalReport.DataSources.Add(rdc);
                 ReportViewer1.LocalReport.Refresh();
                 ReportViewer1.DataBind();

            }
        }

        protected void ObjectDataSource1_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {

        }
    }
}