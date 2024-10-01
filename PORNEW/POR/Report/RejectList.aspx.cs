using Microsoft.Reporting.WebForms;
using ReportData.DAL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace POR.Report
{
    public partial class RejectList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                DALCommanQuery objDALCommanQuery = new DALCommanQuery();

                Reject_DS objReject_DS = new Reject_DS();

                string EffectiveDateFrom = Request.Params["FromDate"].ToString();
                string EffectiveDateTo = Request.Params["ToDate"].ToString();
                string UID = Request.Params["UID"].ToString();
                string Location = Request.Params["Location"].ToString();


                var connectionString = ConfigurationManager.ConnectionStrings["PORConnectionString"].ConnectionString;
                SqlConnection conx = new SqlConnection(connectionString);

                string sql = "select FAD.ServiceNo,FAD.Rank,FAD.Name,FAD.AllowanceName,FAD.EffectiveDate,FAD.EndDate,FAD.CampAuthority,FAD.Comment from Vw_FixedAllowanceDetail FAD "
                           + " Where FAD.EstablishmentId='" + Location + "' and FAD.RecordStatusId = 3000 and FAD.CreatedDate between'" + EffectiveDateFrom + "' and '" + EffectiveDateTo + "' and FAD.Active != 0";

                SqlDataAdapter adp = new SqlDataAdapter(sql, conx);
                adp.Fill(objReject_DS, "DataTable1");
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Report/RejectList.rdlc");
                ReportViewer1.LocalReport.DataSources.Clear();

                ReportDataSource rdc = new ReportDataSource("Reject_DS", objReject_DS.Tables[0]);

                ReportViewer1.LocalReport.DataSources.Add(rdc);
                ReportViewer1.LocalReport.Refresh();
                ReportViewer1.DataBind();
            }
        }
    }
}