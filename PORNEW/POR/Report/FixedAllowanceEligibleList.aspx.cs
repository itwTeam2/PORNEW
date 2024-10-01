using Microsoft.Reporting.WebForms;
using ReportData.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace POR.Report
{
    public partial class FixedAllowanceEligibleList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                DataSet ds = new DataSet();
                string ServiceNo = Request.Params["ServiceNo"].ToString();
                string EstablishmentId = Request.Params["EstablishmentId"].ToString();    

                //Database Connection and Common Quary Class in Report Data Project          

                ds = new DALCommanQuery().FAEligibleList(EstablishmentId, ServiceNo);

                //RDLC Report Path
                RV_FAEligibleList.LocalReport.ReportPath = Server.MapPath("~/Report/FixedAllowanceEligibleList.rdlc");
                RV_FAEligibleList.LocalReport.DataSources.Clear();

                //DataSet Create inside the RDLC Report
                //Vw_AllowanceDataSet is Dataset Name
                ReportDataSource rdc = new ReportDataSource("FAEligibleList_DS", ds.Tables[0]);
                RV_FAEligibleList.LocalReport.DataSources.Add(rdc);
                RV_FAEligibleList.LocalReport.Refresh();
                RV_FAEligibleList.DataBind();

            }  
        }
    }
}