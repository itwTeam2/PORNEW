using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace POR.Report.LivinInOut
{
    public partial class GSQDetailPrint : System.Web.UI.Page
    {

        ///Created BY   : FLT LT WAKY Wickramasinghe
        ///Created Date : 2022/09/05
        /// Description : Load RDLC Report to get GSQ details as print out

        public string sqlQuery;
        POR.Models.LivingInOutHeader objLivingIN = new Models.LivingInOutHeader();
        POR.Models.LivingInOut._GSQHeader obj_GSQHeader = new Models.LivingInOut._GSQHeader();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Session["GSQHID"] != null)
                {
                    int GSQHID = (int)Session["GSQHID"];

                    DataTable dt = new DataTable();
                    DataTable dt2 = new DataTable();

                    ReportData.DAL.DALCommanQuery objDALCommanQuery = new ReportData.DAL.DALCommanQuery();
                    dt = objDALCommanQuery.CallGSQSP(0);
                    dt2 = dt.AsEnumerable().Where(x => x.Field<int>("Active") == 1 && x.Field<int>("GSQHID") == GSQHID).CopyToDataTable();
                    //ReportParameter[] param = new ReportParameter[4];

                    for (int i = 0; i < dt2.Rows.Count; i++)
                    {
                        ReportParameter[] param = new ReportParameter[2];
                        int gsqStatus = Convert.ToInt32(dt2.Rows[i]["GSQStatus"]);
                        if (gsqStatus == (int)POR.Enum.GSQStatus.Allocate)
                        {
                            obj_GSQHeader.AllocatedDate = Convert.ToDateTime(dt2.Rows[i]["AllocatedDate"].ToString());
                        }
                        else
                        {
                            obj_GSQHeader.VacantDate = Convert.ToDateTime(dt2.Rows[i]["VacantDate"].ToString());
                        }
                        obj_GSQHeader.GSQHID = Convert.ToInt32(dt2.Rows[i]["GSQHID"]);
                        obj_GSQHeader.ServiceNo = dt2.Rows[i]["ServiceNo"].ToString();
                        obj_GSQHeader.Rank = dt2.Rows[i]["Rank"].ToString();
                        obj_GSQHeader.Name = dt2.Rows[i]["Name"].ToString();
                        obj_GSQHeader.Branch = dt2.Rows[i]["Branch"].ToString();
                        obj_GSQHeader.GSQLocation = dt2.Rows[i]["GSQLocation"].ToString();
                        obj_GSQHeader.GSQNo = dt2.Rows[i]["GSQNo"].ToString();
                        obj_GSQHeader.GSQStatusName = dt2.Rows[i]["StatusName"].ToString();                
                        obj_GSQHeader.RefNo = dt2.Rows[i]["RefNo"].ToString();
                        obj_GSQHeader.RSID = Convert.ToInt32(dt2.Rows[i]["RecordStatusID"]);

                        ReportDataSource rds = new ReportDataSource("GSQDetails", dt2);
                        ReportViewerGSQ.LocalReport.DataSources.Add(rds);
                        ReportViewerGSQ.LocalReport.ReportPath = "Report/LivinInOut/GSQDetailsPrint.rdlc";
                        //ReportViewerGSQ.LocalReport.SetParameters(param);
                        ReportViewerGSQ.LocalReport.Refresh();
                        ReportViewerGSQ.DataBind();

                    }
                }
            }
        }
    }
}