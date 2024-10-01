using Microsoft.Reporting.WebForms;
using ReportData.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace POR.Report.Contact
{
    public partial class frmChildDetails : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                DataTable dt5 = new DataTable();
                DataTable dt6 = new DataTable();
                DALCommanQuery objDALCommanQuery = new DALCommanQuery();
                long PCHID = Convert.ToInt32(Session["PCHID"]);

                try
                {
                    dt5 = objDALCommanQuery.CallChildDetailsSP(0,0);
                    dt6 = dt5.AsEnumerable().Where(x => x.Field<int>("Active") == 1 && x.Field<int>("PCHID") == PCHID).CopyToDataTable();

                }
                catch (Exception ex)
                {

                    throw ex;
                }
                ReportDataSource rds = new ReportDataSource("ChildDetails", dt6);
                rptChildDetails.LocalReport.DataSources.Add(rds);
                rptChildDetails.LocalReport.Refresh();
                rptChildDetails.DataBind();

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
  
    }
}