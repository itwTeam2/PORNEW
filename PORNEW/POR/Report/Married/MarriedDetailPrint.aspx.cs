using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace POR.Report.Married
{
    public partial class MarriedDetailPrint : System.Web.UI.Page
    {
        
        POR.Models.CivilStatusMarriedDetail objM = new Models.CivilStatusMarriedDetail();
        POR.Models.CivilStatusDivorceDetail objD = new Models.CivilStatusDivorceDetail();
        POR.Models.CivilStatusWidowDetail objW = new Models.CivilStatusWidowDetail();
        public string sqlQuery;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Session["CSHID"] != null)
                {
                    int CSHID = (int)Session["CSHID"];
                    int SubCateId;

                    DataTable dt = new DataTable();
                    DataTable dt2 = new DataTable();

                    ReportData.DAL.DALCommanQuery objDALCommanQuery = new ReportData.DAL.DALCommanQuery();
                    dt = objDALCommanQuery.CallSP(0);

                    dt2 = dt.AsEnumerable().Where(x => x.Field<int>("CSHActive") == 1 && x.Field<int>("CSHID") == CSHID).CopyToDataTable();

                    for (int i = 0; i < dt2.Rows.Count; i++)
                    {
                        SubCateId = Convert.ToInt32(dt2.Rows[i]["SubCategoryId"].ToString());
                        ReportParameter[] param = new ReportParameter[2];
                        switch (SubCateId)
                        {
                            case (int)POR.Enum.CivilStatusCategory.Marriage:
                                objM.MarriageDate = Convert.ToDateTime(dt2.Rows[i]["MarriageDate"]);
                                objM.RegistarOfficeLocation = dt2.Rows[i]["RegistarOfficeLocation"].ToString();
                                objM.MarriageCertificateNo = dt2.Rows[i]["MarriageCertificateNo"].ToString();

                                param[0] = new ReportParameter("DeathHeading", "Marriage Date : |Marriage Certificate No :|Registrar's' Office Location  : ", false);
                                param[1] = new ReportParameter("DeathValues", " " + objM.MarriageDate + "| " + objM.MarriageCertificateNo + "| " + objM.RegistarOfficeLocation + "", false);

                                break;
                            case (int)POR.Enum.CivilStatusCategory.Divorce:
                            case (int)POR.Enum.CivilStatusCategory.NullVoid:

                                objD.DivorceDtae = Convert.ToDateTime(dt2.Rows[i]["DivorceDtae"]);
                                objD.Place = dt2.Rows[i]["Place"].ToString();
                                objD.CourseCaseNo = dt2.Rows[i]["CourseCaseNo"].ToString();
                                objD.DateOfCase = Convert.ToDateTime(dt2.Rows[i]["DateOfCase"]);

                                param[0] = new ReportParameter("DeathHeading", "Divorce Date : |Divorce Place :|Court Case No  :|Date Of Case :", false);
                                param[1] = new ReportParameter("DeathValues", " " + objD.DivorceDtae + "| " + objD.Place + "| " + objD.CourseCaseNo + "|" + objD.DateOfCase + "", false);
                                break;
                            case (int)POR.Enum.CivilStatusCategory.Widow:

                                objW.DeathCertificateNoOfSpouse = dt2.Rows[i]["DeathCertificateNoOfSpouse"].ToString();
                                objW.DateofDecease = Convert.ToDateTime(dt2.Rows[i]["DateofDecease"]);

                                param[0] = new ReportParameter("DeathHeading", "Death Certificate No Of Spouse  : |Date of Decease : ", false);
                                param[1] = new ReportParameter("DeathValues", "" + objW.DeathCertificateNoOfSpouse + "| " + objW.DateofDecease + "", false);
                                break;
                            default:
                                break;
                        }

                        ReportDataSource rds = new ReportDataSource("MarriedDetails", dt2);
                        ReportViewerMarried.LocalReport.DataSources.Add(rds);
                        ReportViewerMarried.LocalReport.ReportPath = "Report/Married/MarriedDetailPrint.rdlc";
                        ReportViewerMarried.LocalReport.SetParameters(param);
                        ReportViewerMarried.LocalReport.Refresh();
                        ReportViewerMarried.DataBind();
                    }                
                }
                else
                {

                }
            }
        }
    }
    
}