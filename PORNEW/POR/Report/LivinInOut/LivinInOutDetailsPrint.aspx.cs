using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace POR.Report.LivinInOut
{
    public partial class LivinInOutDetailsPrint : System.Web.UI.Page
    {

        ///Created BY   : FLT LT WAKY Wickramasinghe
        ///Created Date : 2022/09/05
        /// Description : Load RDLC Report to get Living IN Out as print out

        public string sqlQuery;
        POR.Models.LivingInOutHeader objLivingIN = new Models.LivingInOutHeader();
        POR.Models.LivingInOut._LivingInOut obj_LivingInOut = new Models.LivingInOut._LivingInOut();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Session["LIOHID"] != null)
                {
                    int LIOHID = (int)Session["LIOHID"];
                    int LivingSubCat;
                    DataTable dt = new DataTable();
                    DataTable dt2 = new DataTable();
                    int NOKChange = 0;

                    ReportData.DAL.DALCommanQuery objDALCommanQuery = new ReportData.DAL.DALCommanQuery();
                    dt = objDALCommanQuery.CallLivingINOutSP(0);
                    dt2 = dt.AsEnumerable().Where(x => x.Field<int>("LIOActive") == 1 && x.Field<int>("LIOHID") == LIOHID).CopyToDataTable();
                    //ReportParameter[] param = new ReportParameter[4];

                    for (int i = 0; i < dt2.Rows.Count; i++)
                    {
                        LivingSubCat = Convert.ToInt32(dt2.Rows[i]["LivingStatusID"].ToString());
                        NOKChange = Convert.ToInt32(dt2.Rows[i]["IsNOKChange"]);


                        if (LivingSubCat == (int)POR.Enum.LivinInOutCategories.BLIN || LivingSubCat == (int)POR.Enum.LivinInOutCategories.MLIN || LivingSubCat == (int)POR.Enum.LivinInOutCategories.MLOut)
                        {
                            ReportParameter[] param = new ReportParameter[2];

                            objLivingIN.FromDate = Convert.ToDateTime(dt2.Rows[i]["FromDate"]);
                            param[0] = new ReportParameter("LivingHeading", "With Effect Date : ", false);
                            param[1] = new ReportParameter("LivingValues", " " + objLivingIN.FromDate + "", false);

                            if (LivingSubCat == (int)POR.Enum.LivinInOutCategories.MLIN || LivingSubCat == (int)POR.Enum.LivinInOutCategories.MLOut)
                            {

                                if (NOKChange == 1)
                                {
                                    ReportParameter[] param1 = new ReportParameter[4];

                                    objLivingIN.FromDate = Convert.ToDateTime(dt2.Rows[i]["FromDate"]);
                                    param1[0] = new ReportParameter("LivingHeading", "With Effect Date : ", false);
                                    param1[1] = new ReportParameter("LivingValues", " " + objLivingIN.FromDate + "", false);

                                    obj_LivingInOut.NOKName = dt2.Rows[i]["NOKName"].ToString();
                                    obj_LivingInOut.NOKChangeTo = dt2.Rows[i]["NOKChangeTo"].ToString();
                                    obj_LivingInOut.NOKaddress = dt2.Rows[i]["NOKAddress"].ToString();
                                    obj_LivingInOut.District1 = dt2.Rows[i]["District"].ToString();
                                    obj_LivingInOut.GSName = dt2.Rows[i]["GSDivision"].ToString();
                                    obj_LivingInOut.NearestTown = dt2.Rows[i]["NearestTown"].ToString();
                                    obj_LivingInOut.PoliceStation1 = dt2.Rows[i]["PoliceStation"].ToString();
                                    obj_LivingInOut.PostOfficeName = dt2.Rows[i]["PostOffice"].ToString();
                                    obj_LivingInOut.NOKWEFDate = Convert.ToDateTime(dt2.Rows[i]["WFDate"]);

                                    param1[2] = new ReportParameter("NOKHeading", "NOK Name : || NOK Change To : || NOK Address : ||District :|| GS Division : || Nearest Town : || Police Station : || Post Office : || With Effect Date : ||", false);
                                    param1[3] = new ReportParameter("NOKValues", " " + obj_LivingInOut.NOKName + " || " + obj_LivingInOut.NOKChangeTo + " || " + obj_LivingInOut.NOKaddress + " || " + obj_LivingInOut.District1 + " || " + obj_LivingInOut.GSName + " || " + obj_LivingInOut.NearestTown + " || " + obj_LivingInOut.PoliceStation1 + " || " + obj_LivingInOut.PostOfficeName + " || " + obj_LivingInOut.NOKWEFDate + "", false);


                                    ReportDataSource rds1 = new ReportDataSource("LivingInOut", dt2);
                                    ReportViewerLivinInOut.LocalReport.DataSources.Add(rds1);
                                    ReportViewerLivinInOut.LocalReport.ReportPath = "Report/LivinInOut/LivinInOutDetailsPrint.rdlc";
                                    ReportViewerLivinInOut.LocalReport.SetParameters(param1);
                                    ReportViewerLivinInOut.LocalReport.Refresh();
                                    ReportViewerLivinInOut.DataBind();
                                }
                                else
                                {

                                }

                                ReportDataSource rds = new ReportDataSource("LivingInOut", dt2);
                                ReportViewerLivinInOut.LocalReport.DataSources.Add(rds);
                                ReportViewerLivinInOut.LocalReport.ReportPath = "Report/LivinInOut/LivinInOutDetailsPrint.rdlc";
                                ReportViewerLivinInOut.LocalReport.SetParameters(param);
                                ReportViewerLivinInOut.LocalReport.Refresh();
                                ReportViewerLivinInOut.DataBind();

                            }

                        }
                        else
                        {
                            ReportParameter[] param = new ReportParameter[2];

                            objLivingIN.FromDate = Convert.ToDateTime(dt2.Rows[i]["FromDate"]);
                            objLivingIN.ToDate = Convert.ToDateTime(dt2.Rows[i]["ToDate"]);

                            param[0] = new ReportParameter("LivingHeading", "From Date : ||To Date : ", false);
                            param[1] = new ReportParameter("LivingValues", " " + objLivingIN.FromDate + "||" + objLivingIN.ToDate + "", false);

                            ReportDataSource rds = new ReportDataSource("LivingInOut", dt2);
                            ReportViewerLivinInOut.LocalReport.DataSources.Add(rds);
                            ReportViewerLivinInOut.LocalReport.ReportPath = "Report/LivinInOut/LivinInOutDetailsPrint.rdlc";
                            ReportViewerLivinInOut.LocalReport.SetParameters(param);
                            ReportViewerLivinInOut.LocalReport.Refresh();
                            ReportViewerLivinInOut.DataBind();
                        }


                    }
                }
                else
                {

                }
            }
        }
    }
}



