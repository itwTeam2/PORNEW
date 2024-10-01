using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;


/// <summary>
/// Summary description for DALConnectionManager
/// </summary>
public class DALConnectionManager
{
    public static SqlConnection open() 
    {
        SqlConnection ConnectionManager = new SqlConnection();
        try
        {
            //ConnectionManager.ConnectionString = "Data Source=135.22.210.105\\;Initial Catalog=POR;User ID=poruser;password=password";
            //ConnectionManager.ConnectionString = "Data Source=135.22.67.113\\;Initial Catalog=POR;User ID=poruser;password=password";
            ConnectionManager.ConnectionString = "Data Source=135.22.67.113\\;Initial Catalog=POR;User ID=poruser;password=password";
            ConnectionManager.Open();              
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return ConnectionManager;
    }
    public static void Close(SqlConnection Connection) 
    {
        try
        {
            if (Connection.State == ConnectionState.Open)
            {
                Connection.Close();
            }
        }
        catch (Exception)
        {            
            throw;
        }
    }

    public static SqlConnection openHrmis()
    {
        SqlConnection ConnectionManager = new SqlConnection();
        try
        {
            ConnectionManager.ConnectionString = "Data Source=135.22.210.105\\;Initial Catalog=P3HRMS;User ID=hrmsuser;password=hrms123";
            //ConnectionManager.ConnectionString = "Data Source=135.22.67.71\\;Initial Catalog=P3HRMS;User ID=p3hrmsuser;password=p3hrmsuser@123";
            //ConnectionManager.Open();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return ConnectionManager;
    }
    public static void CloseHrim(SqlConnection Connection)
    {
        try
        {
            if (Connection.State == ConnectionState.Open)
            {
                Connection.Close();
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    public static SqlConnection openP2Hrmis() {

        SqlConnection ConnectionManagerP2 = new SqlConnection();
        try
        {
            ConnectionManagerP2.ConnectionString = "Data Source=135.22.210.105\\;Initial Catalog=HRMS;User ID=hrmsuser;password=hrms123";
            //ConnectionManagerP2.ConnectionString = "Data Source=135.22.67.71\\;Initial Catalog=HRMS;User ID=hrmsuser;password=hrms123";
            //ConnectionManagerP2.Open();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return ConnectionManagerP2;
    }

    public static void CloseP2Hrims(SqlConnection Connection)
    {
        try
        {
            if (Connection.State == ConnectionState.Open)
            {
                Connection.Close();
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    public static SqlConnection openPOR()
    {
        SqlConnection ConnectionManager = new SqlConnection();
        try
        {
            //ConnectionManager.ConnectionString = "Data Source=135.22.210.105\\;Initial Catalog=POR;User ID=poruser;password=password";
             ConnectionManager.ConnectionString = "Data Source=135.22.67.113\\;Initial Catalog=POR;User ID=poruser;password=password";
            //ConnectionManager.Open();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return ConnectionManager;
    }

}
