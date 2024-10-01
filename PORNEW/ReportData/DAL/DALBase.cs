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
using System.Net.NetworkInformation;
using System.Collections.Generic;

/// <summary>
/// Summary description for DALBase
/// </summary>
public class DALBase
{   
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
    public DataSet Detail(string sql)
   {
       DataSet ds = new DataSet();
       try
       {
           SqlConnection Connection = DALConnectionManager.open();
           SqlCommand command = new SqlCommand(sql, Connection);
           SqlDataAdapter adapter = new SqlDataAdapter(command);
           adapter.Fill(ds);
           DALConnectionManager.Close(Connection);

       }
       catch (Exception ex)
       {
           throw ex;
       }
       return ds;
   }
    public DataSet GetSpInfo(string sql)
    {
        DataSet ds = new DataSet();
        try
        {
            SqlConnection Connection = DALConnectionManager.open();
            SqlCommand command = new SqlCommand(sql, Connection);
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(ds);
            DALConnectionManager.Close(Connection);

        }
        catch (Exception ex)
        {
            throw ex;
        }
        return ds;
    }
    public int Record(string sql)
   {
       int Id = 0;
       try
       {
           SqlConnection Connection = DALConnectionManager.open();
           SqlCommand command = new SqlCommand(sql, Connection);
           SqlDataAdapter adapter = new SqlDataAdapter(command);
           adapter.Equals(Id);
           DALConnectionManager.Close(Connection);
       }
       catch (Exception ex)
       {
           throw ex;
       }
       return Id;
   }
    public string GetMacAddress()
   {
       string macAddresses = string.Empty;

       try
       {
           foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
           {
               if (nic.OperationalStatus == OperationalStatus.Up)
               {
                   macAddresses += nic.GetPhysicalAddress().ToString();
                   break;
               }
           }
       }
       catch (Exception ex)
       {
           throw ex;
       }

       return macAddresses;
   }
    public bool Update(string sql)
    {
        bool status = false;
        //SqlTransaction trancation =  null;
        try
        {
            SqlConnection Connection = DALConnectionManager.openHrmis();
            SqlCommand command = new SqlCommand(sql, Connection);
            command.CommandTimeout = 0;
            command.CommandText = sql;           
            int rowAffected = command.ExecuteNonQuery();
           
            status = true;

            DALConnectionManager.CloseHrim(Connection);

        }
        catch (Exception ex)
        {
            status = false;         
            throw ex;
        }
        return status;
    }
    public bool Insert(string sql)
    {
        bool status = false;
        try
        {
            SqlConnection Connection = DALConnectionManager.openHrmis();
            SqlCommand command = new SqlCommand(sql, Connection);
            command.CommandTimeout = 0;
            command.CommandText = sql;
            int rowAffected = command.ExecuteNonQuery();

            status = true;

            DALConnectionManager.CloseHrim(Connection);
        }
        catch (Exception)
        {
            throw;
        }
        return status;
    }
    public DataTable SelectHrmis(string sql)
    {
        DataTable dt = new DataTable();
        try
        {
            SqlConnection Connection = DALConnectionManager.openHrmis();
            SqlCommand command = new SqlCommand(sql, Connection);
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(dt);
            DALConnectionManager.CloseHrim(Connection);

        }
        catch (Exception ex)
        {
            throw ex;
        }
        return dt;
    }
    public bool TransactionHandlle(string sql, string sql1)
    {
        bool status = false;
       
        try
        {
            SqlConnection Connection = DALConnectionManager.openHrmis();
            
            SqlCommand command = new SqlCommand(sql, Connection);
            command.CommandTimeout = 0;
            command.CommandText = sql;
            command.ExecuteNonQuery();            
           
            status = true;

            DALConnectionManager.CloseHrim(Connection);

        }
        catch (Exception ex)
        {
            
            status = false;
            throw ex;
        }
        return status;
    }
    

}
