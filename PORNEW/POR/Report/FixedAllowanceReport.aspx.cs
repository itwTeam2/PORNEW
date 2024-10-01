using Microsoft.Reporting.WebForms;
using POR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ReportData.DAL;
using System.Data;

namespace POR.Report
{
    public partial class FixedAllowanceReport : System.Web.UI.Page
    {
        //ReportData.DAL; namespace add to the page
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                DataSet ds = new DataSet();
                string EffectiveDateFrom = Request.Params["EffectiveDateFrom"].ToString();
                string EffectiveDateTo = Request.Params["EffectiveDateTo"].ToString();
                string EstablishmentId = Request.Params["EstablishmentId"].ToString();        

                //Database Connection and Common Quary Class in Report Data Project          

                ds = new DALCommanQuery().FixedAllowanceReport(EffectiveDateFrom, EffectiveDateTo, EstablishmentId);

                //RDLC Report Path
                RV_FixedAllowance.LocalReport.ReportPath = Server.MapPath("~/Report/Report1.rdlc");
                RV_FixedAllowance.LocalReport.DataSources.Clear();

                //DataSet Create inside the RDLC Report
                //Vw_AllowanceDataSet is Dataset Name
                ReportDataSource rdc = new ReportDataSource("Vw_FixedAllowanceDetail_DS", ds.Tables[0]);
                RV_FixedAllowance.LocalReport.DataSources.Add(rdc);
                RV_FixedAllowance.LocalReport.Refresh();
                RV_FixedAllowance.DataBind();

            }  
        }
    }
}