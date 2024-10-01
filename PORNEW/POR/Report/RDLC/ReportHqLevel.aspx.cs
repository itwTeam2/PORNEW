using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace POR.Report.RDLC
{
    public partial class ReportHqLevel : System.Web.UI.Page
    {
        SqlCommand oSqlCommand;
        public string sqlQuery;
        //string fromDate;
        //string toDate;

        string fromDate;
        string toDate;
        string location;

        DataTable dt = new DataTable();
        DataTable dt1 = new DataTable();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                //fromDate = (string)Session["fromDate"];
                //toDate = (string)Session["toDate"];

                fromDate = Session["fromDate"].ToString();
                toDate = Session["toDate"].ToString();

                location = Session["location"].ToString();
                int PageLoad = (int)Session["PageLoad"];

                DataTable odsvoltxndata6 = new DataTable();
                oSqlCommand = new SqlCommand();

                if (PageLoad == 1)
                {
                    fromDate = null;
                    toDate = null;

                    string Query = "select * from Vw_F121 AS Vw"
                     + " where CAST(Vw.ChargeDate AS DATE) BETWEEN '" + fromDate + "' AND '" + toDate + "' ";

                    DataTable dt = Select(Query);

                    ReportDataSource rds = new ReportDataSource("ReportHqLevel", dt);
                    ReportViewerReportHqLevel.LocalReport.DataSources.Add(rds);
                    ReportViewerReportHqLevel.LocalReport.Refresh();
                    ReportViewerReportHqLevel.DataBind();
                }

                // to get all location data For Afhq Level
                else if (location == "ALL")
                {

                    DataTable dt = CallSP1(fromDate, toDate);


                    ReportDataSource rds = new ReportDataSource("ReportHqLevel", dt);
                    ReportViewerReportHqLevel.LocalReport.DataSources.Add(rds);
                    ReportViewerReportHqLevel.LocalReport.Refresh();
                    ReportViewerReportHqLevel.DataBind();
                }
                else
                {


                    //DataTable dt = Select(Query);
                    DataTable dt = CallSP(fromDate, toDate, location);
                    //dt1 = dt.AsEnumerable().Where(x => x.Field<int>("FMSID") == 241).CopyToDataTable();
                    ReportDataSource rds = new ReportDataSource("ReportHqLevel", dt);
                    ReportViewerReportHqLevel.LocalReport.DataSources.Add(rds);
                    ReportViewerReportHqLevel.LocalReport.Refresh();
                    ReportViewerReportHqLevel.DataBind();
                }


            }
        }
        public DataTable Select(string sql)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlConnection Connection = DALConnectionManager.open();
                SqlCommand command = new SqlCommand(sql, Connection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(dt);
                DALConnectionManager.Close(Connection);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }
        public DataTable CallSP(string fromDate, string toDate, string location)
        {
            ///Created BY   :
            ///Created Date : 
            /// Description : 

            DataTable dt = new DataTable();
            try
            {

                SqlConnection Connection = DALConnectionManager.open();
                SqlCommand command = Connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "F121SP";
                command.Parameters.AddWithValue("@fromDate", fromDate);
                command.Parameters.AddWithValue("@toDate", toDate);
                command.Parameters.AddWithValue("@location", location);
                command.CommandTimeout = 1000;
                SqlDataAdapter adp = new SqlDataAdapter(command);

                adp.Fill(dt);

                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }



        public DataTable CallSP1(string fromDate, string toDate)
        {
            ///Created BY   :
            ///Created Date : 
            /// Description : 

            DataTable dt = new DataTable();
            try
            {

                SqlConnection Connection = DALConnectionManager.open();
                SqlCommand command = Connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "F121SP1";
                command.Parameters.AddWithValue("@fromDate", fromDate);
                command.Parameters.AddWithValue("@toDate", toDate);
                //command.Parameters.AddWithValue("@location", location);
                command.CommandTimeout = 1000;
                SqlDataAdapter adp = new SqlDataAdapter(command);

                adp.Fill(dt);

                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }


    }
}