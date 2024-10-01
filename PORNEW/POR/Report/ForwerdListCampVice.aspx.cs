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
    public partial class ForwerdListCampVice : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                DALCommanQuery objDALCommanQuery = new DALCommanQuery();

                CampForwardList objCampForwardList = new CampForwardList();

                string EffectiveDateFrom = Request.Params["FromDate"].ToString();
                string EffectiveDateTo = Request.Params["ToDate"].ToString();
                string UID = Request.Params["UID"].ToString();
                string Location = Request.Params["Location"].ToString();
              //  string Rid =Request.Params["RID"].ToString();


                var connectionString = ConfigurationManager.ConnectionStrings["PORConnectionString"].ConnectionString;
                SqlConnection conx = new SqlConnection(connectionString);

                string sql = "Select VFS.ServiceNo,VFS.Rank,VFS.Name,VFS.AllowanceName,VFS.EffectiveDate,VFS.EndDate,VFS.CampAuthority from Vw_FixedAllowanceDetail VFS "
                           + " where VFS.EstablishmentId='"+ Location + "' and VFS.RecordStatusId !=3000 and VFS.FMSID >= 131 and VFS.Active != 0 "
                           + " and VFS.CreatedDate between'" + EffectiveDateFrom + "' and '" + EffectiveDateTo + "'";
                                              

                SqlDataAdapter adp = new SqlDataAdapter(sql, conx);
                adp.Fill(objCampForwardList, "Vw_FixedAllowance_FLowStatus");
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Report/CampForwardList.rdlc");
                ReportViewer1.LocalReport.DataSources.Clear();

                ReportDataSource rdc = new ReportDataSource("CampForwardList", objCampForwardList.Tables[0]);

                ReportViewer1.LocalReport.DataSources.Add(rdc);
                ReportViewer1.LocalReport.Refresh();
                ReportViewer1.DataBind();
            }
        }
    }
}