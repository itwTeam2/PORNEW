using ReportData.BAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ReportData.DAL
{
    public class DALCommanQuery:DALBase
    {
        string SNO = "";        
        public DataSet NextId_()
        {
            DataSet ds = new DataSet();

            try
            {
                string SQL = "SELECT IDENT_CURRENT('FixedAllowanceDetail')";

                ds = Detail(SQL);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds;
        }
        public int NextId()
        {
            int Id = 0;
            DataTable dt = new DataTable();
            try
            {
                string SQL = "select IDENT_CURRENT('FixedAllowanceDetail')+1 as NextId";
                dt = Select(SQL);
                foreach (DataRow row in dt.Rows)
                {
                   Id = Convert.ToInt32(row["NextId"]);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Id;
        }
        public int NextLeaveHeaderNextId()
        {
            int Id = 0;
            DataTable dt = new DataTable();
            try
            {
                string SQL = "select IDENT_CURRENT('LeaveHeader') as NextId";
                dt = Select(SQL);
                foreach (DataRow row in dt.Rows)
                {
                    Id = Convert.ToInt32(row["NextId"]);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Id+1;
        }
        public DataSet ThisLeaveCount(string ServiceNo, int ThisYear)
        {
            DataSet ds = new DataSet();

            try
            {
                string sql = " SELECT sum(LeaveDetail.AnnualLeave)AnnualLeave, sum(LeaveDetail.PrivilegeLeave) PrivilegeLeave, DATEPART(year, LH.FromDate) FromDate, DATEPART(year, LH.ToDate) "
                            + " FROM LeaveHeader LH INNER JOIN LeaveDetail ON LH.LHID = LeaveDetail.LeaveHeaderId where  LH.Sno = '" + ServiceNo + "' and DATEPART(year, LH.FromDate)in('" + ThisYear + "') and  LH.Active !=0"
                            + " group by DATEPART(year, LH.FromDate) , DATEPART(year, LH.ToDate)";

                //string sql = " select MAX(LH.PrivilegeLeave)as PrivilegeLeave,MIN(LH.AnnualLeave)as AnnualLeave,DATEPART(year,LH.FromDate) as D,DATEPART(year,LH.ToDate) as Todate from  LeaveHeader LH INNER JOIN LeaveDetail LD ON LH.LHID = LD.LeaveHeaderId "
                //           + " where LH.Sno = '" + ServiceNo + "' and LH.LeaveCategoryId = 1 and DATEPART(year,LH.FromDate)in('" + MinYearFrom + "','" + MidYearFrom + "','" + YearFrom + "') " // and LH.Active !=0
                //           + " group by DATEPART(year,LH.FromDate) , DATEPART(year,LH.ToDate)  ";

                ds = Detail(sql);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds;
        }
        public DataSet LeaveCount(string ServiceNo, int MinYearFrom, int MidYearFrom)
        {
            DataSet ds = new DataSet();

            try
            {
                string sql = " SELECT sum(LeaveDetail.AnnualLeave)AnnualLeave, sum(LeaveDetail.PrivilegeLeave) PrivilegeLeave, DATEPART(year, LH.FromDate) FromDate, DATEPART(year, LH.ToDate) "
                            + " FROM LeaveHeader LH INNER JOIN LeaveDetail ON LH.LHID = LeaveDetail.LeaveHeaderId where  LH.Sno = '" + ServiceNo + "' and DATEPART(year, LH.FromDate)in('" + MinYearFrom + "','" + MidYearFrom + "') "
                            + " group by DATEPART(year, LH.FromDate) , DATEPART(year, LH.ToDate)";

                //string sql = " select MAX(LH.PrivilegeLeave)as PrivilegeLeave,MIN(LH.AnnualLeave)as AnnualLeave,DATEPART(year,LH.FromDate) as D,DATEPART(year,LH.ToDate) as Todate from  LeaveHeader LH INNER JOIN LeaveDetail LD ON LH.LHID = LD.LeaveHeaderId "
                //           + " where LH.Sno = '" + ServiceNo + "' and LH.LeaveCategoryId = 1 and DATEPART(year,LH.FromDate)in('" + MinYearFrom + "','" + MidYearFrom + "','" + YearFrom + "') " // and LH.Active !=0
                //           + " group by DATEPART(year,LH.FromDate) , DATEPART(year,LH.ToDate)  ";

                ds = Detail(sql);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds;
        }
        public DataSet FixedAllowanceReport(string EffectiveDateFrom, string EffectiveDateTo, string EstablishmentId)
        {
            DataSet ds = new DataSet();
            try
            {
                string sql = "SELECT * FROM Vw_FixedAllowanceDetail";

                if ((EffectiveDateFrom != "" && EffectiveDateFrom != null) || (EffectiveDateTo != "" && EffectiveDateTo != null))
                {
                    DateTime? FromDate_ = Convert.ToDateTime(EffectiveDateFrom);
                    DateTime? ToDate_ = Convert.ToDateTime(EffectiveDateTo);
                    sql += " where EstablishmentId='" + EstablishmentId + "' and CurrentStatus=4 and (EffectiveDate >= '" + FromDate_ + "' and EffectiveDate < '" + ToDate_ + "')";

                    //FixedAllowanceDetails = FixedAllowanceDetails.Where(x => x.EstablishmentId == UserInfo.LocationId & x.CurrentStatus == 4 && (x.EffectiveDate >= FromDate_ && x.EffectiveDate < ToDate_));
                }
                else
                {
                    sql += " where EstablishmentId='" + EstablishmentId + "' and CurrentStatus=4";
                    //FixedAllowanceDetails = FixedAllowanceDetails.Where(x => x.EstablishmentId == UserInfo.LocationId & x.CurrentStatus == 4);
                }                
                ds = Detail(sql);
            }
            catch (Exception ex)
            {                
                throw ex;
            }
            return ds;
        }
        public DataSet FAEligibleList(string EstablishmentId,string ServiceNo)
        {
            DataSet ds = new DataSet();
            try
            {
                
                string sql = "SELECT VPD.ServiceNo,VPD.Rank,VPD.Name,A.AllowanceName,FAEL.EffectiveDate,FAEL.EndDate FROM Vw_PersonalDetail VPD,FixedAllowanceEligibleList FAEL,Allowance A " +
                             " WHERE VPD.SNo = FAEL.SNo and FAEL.AllowanceId = A.ALWID";

                if (ServiceNo != null || ServiceNo != "")
                {
                    sql += " AND EstablishmentId='" + EstablishmentId + "' and CurrentStatus=4 AND VPD.ServiceNo = '" + ServiceNo + "'";
                    //FixedAllowanceDetails = FixedAllowanceDetails.Where(x => x.EstablishmentId == UserInfo.LocationId & x.CurrentStatus == 4 && (x.EffectiveDate >= FromDate_ && x.EffectiveDate < ToDate_));
                }
                else
                {
                    sql += "  AND EstablishmentId='" + EstablishmentId + "' and CurrentStatus=4";
                    //FixedAllowanceDetails = FixedAllowanceDetails.Where(x => x.EstablishmentId == UserInfo.LocationId & x.CurrentStatus == 4);
                }
                ds = Detail(sql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds;
        }
        public DataSet RejectList(string EffectiveDateFrom, string EffectiveDateTo, string EstablishmentId)
        {
            DataSet ds = new DataSet();
            try
            {
                string sql = " select Vw.ServiceNo,Vw.Rank,Vw.Name,FAD.AllowanceId,FAD.EffectiveDate,FAD.EndDate,FAD.CampAuthority,FAFS.Comment from FixedAllowanceDetail FAD Inner join FixAllowanceDetailsFlowStatus FAFS ON "
                             + " FAD.FADID = FAFS.FADID inner join Vw_PersonalDetail Vw on FAD.Sno = Vw.SNo where FAFS.CreatedDate in(select max(F.CreatedDate) from  FixAllowanceDetailsFlowStatus F "
                             + " where  F.FADID = FAD.FADID and F.CreatedDate between'" + EffectiveDateFrom + "' and '" + EffectiveDateTo + "' and f.RecordStatusId='3000') and FAD.EstablishmentId='" + EstablishmentId + "'";

                ds = Detail(sql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds;
        }
        public DataSet AnnualLeaveCount(string ServiceNo, int MinYearFrom, int MidYearFrom,int YearFrom)
        {
            DataSet ds = new DataSet();
           
             try
            {
                string sql = " select MAX(LH.PrivilegeLeave)as PrivilegeLeave,MIN(LH.AnnualLeave)as AnnualLeave,DATEPART(year,LH.FromDate) as D,DATEPART(year,LH.ToDate) as Todate from  LeaveHeader LH INNER JOIN LeaveDetail LD ON LH.LHID = LD.LeaveHeaderId "
                           + " where LH.Sno = '" + ServiceNo + "' and LH.LeaveCategoryId = 1 and DATEPART(year,LH.FromDate)in('" + MinYearFrom + "','" + MidYearFrom + "','" + YearFrom + "') " // and LH.Active !=0
                           + " group by DATEPART(year,LH.FromDate) , DATEPART(year,LH.ToDate)  ";
                
                ds = Detail(sql);
               
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds;
        }
        public DataSet AnnualCasualLeave(string ServiceNo, int MinYearFrom, int MidYearFrom, int YearFrom)
        {
            DataSet ds = new DataSet();

            try
            {
                string sql = " select SUM(LD.PrivilegeLeave)as PrivilegeLeave,SUM(LD.AnnualLeave)as AnnualLeave,DATEPART(year,LH.FromDate) as D,DATEPART(year,LH.ToDate) as Todate from  LeaveHeader LH INNER JOIN LeaveDetail LD ON LH.LHID = LD.LeaveHeaderId "
                           + " where LH.Sno = '" + ServiceNo + "' and LH.LeaveCategoryId = 1 and DATEPART(year,LH.FromDate)in('" + MinYearFrom + "','" + MidYearFrom + "','" + YearFrom + "') "
                           + " group by DATEPART(year,LH.FromDate) , DATEPART(year,LH.ToDate)  ";

                ds = Detail(sql);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds;
        }
        public DataSet LeaveDeffarent(string ServiceNo, int MinYearFrom, int MidYearFrom)
        {
            DataSet ds = new DataSet();
            try
            {
                string sql = "select LH.Sno,DATEPART(year,LH.FromDate)as FromDate , DATEPART(year,LH.ToDate) as Todate,LD.PrivilegeLeave,DATEDIFF(day,LH.FromDate,concat(DATEPART(year,LH.FromDate),'-', DATEPART(month,LH.FromDate),'-',DATEPART(DAY,LH.FromDate))),"
                           + "DATEDIFF(day,LH.FromDate,concat(DATEPART(YYYY,LH.FromDate),'-'+'12'+'-'+'31')) AS FromYear,DATEDIFF(day,LH.ToDate,concat(DATEPART(YYYY,LH.ToDate),'-'+'01'+'-'+'01')) AS ToYear "
                           + "from  LeaveHeader LH INNER JOIN LeaveDetail LD ON LH.LHID = LD.LeaveHeaderId where LH.Sno = '" + ServiceNo + "' and LH.LeaveCategoryId = 1 and  LH.Active!=0 and DATEPART(year,LH.FromDate)in('" + MinYearFrom + "','" + MidYearFrom + "')";

                ds = Detail(sql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds;
        }       
        public DataSet AnnualLeavePlane(int MinYearFrom, int MidYearFrom, int fromyear)
        {
            DataSet ds = new DataSet();           
            try
            {
                string sql = "select AnnualLeavePlan.Year,LeaveCategory.LeaveCategoryName,AnnualLeavePlan.TotalLeaveDays from AnnualLeavePlan INNER JOIN LeaveCategory "
                           + " ON AnnualLeavePlan.LeaveCategoryID = LeaveCategory.LCID where  AnnualLeavePlan.Year  in('" + MinYearFrom + "','" + MidYearFrom + "','" + fromyear + "') and AnnualLeavePlan.LeaveCategoryID in(9)";

                ds = Detail(sql);               
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds;
        }
        public DataSet RejectLeave(string Sno)
        {
            DataSet ds = new DataSet();
            try
            {
                string sql = " select LeaveHeader.LHID,LeaveHeader.PrivilegeLeave,LeaveHeader.AnnualLeave,LeaveDetail.AnnualLeave AS AL,LeaveDetail.PrivilegeLeave as PL from LeaveHeader "
                             + " INNER JOIN LeaveDetail ON LeaveHeader.LHID = LeaveDetail.LeaveHeaderId  "
                             + " where  LeaveHeader.LHID in (select max(LH.LHID) from  LeaveHeader as LH where  LH.Sno = '" + Sno + "' and LH.Active!=0) ";

                ds = Detail(sql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds;
        }
        public DataTable LoadLeaveRejectList(int RoleId)
        {
            DataTable dt = new DataTable();
            try
            {
                string SQL = " SELECT * " +
                              " FROM Vw_Leave VL " +
                              " WHERE VL.CurrentStatus = '" + RoleId + "' and VL.Active = 1 and VL.RecordStatusId =3000 ";
                dt = Select(SQL);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return dt;
        }
        public int AnnualLeaveplaneCasualCheck(int fromyear)
        {
            DataSet ds = new DataSet();
            int Casual=0;
            try
            {
                string sql = "select AnnualLeavePlan.Year,LeaveCategory.LeaveCategoryName,AnnualLeavePlan.TotalLeaveDays from AnnualLeavePlan INNER JOIN LeaveCategory "
                           + " ON AnnualLeavePlan.LeaveCategoryID = LeaveCategory.LCID where  AnnualLeavePlan.Year  in('" + fromyear + "') and AnnualLeavePlan.LeaveCategoryID in(7)";

                ds = Detail(sql);
                DataTable dt = ds.Tables[0];
                foreach (DataRow item in dt.Rows)
                {
                    Casual = Convert.ToInt32(item["TotalLeaveDays"].ToString());                    
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Casual;
        }
        public int HistryCheck(string Sno)
        {
            DataSet ds = new DataSet();
            int HistryCheck = 0;
            try
            {
                string sql = "select * from LeaveHeader where LeaveHeader.FromDate = '2018-10-01' and LeaveHeader.Sno='"+Sno+"'";

                ds = Detail(sql);
                DataTable dt = ds.Tables[0];
                HistryCheck = dt.Rows.Count;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return HistryCheck;
        }
        public int HistryCheck2019(string Sno)
        {
            DataSet ds = new DataSet();
            int HistryCheck2019 = 0;
            try
            {
                string sql = "select * from LeaveHeader where LeaveHeader.FromDate = '2019-10-01' and LeaveHeader.Sno='" + Sno + "'";

                ds = Detail(sql);
                DataTable dt = ds.Tables[0];
                HistryCheck2019 = dt.Rows.Count;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return HistryCheck2019;
        }
        public int CasualLeaveCount(string ServiceNo, int fromyear)
        {
            DataSet ds = new DataSet();
            int Casual = 0;
            try
            {
                string sql = " SELECT sum(LeaveDetail.CasualLeave)as Counts "    
                           + " FROM LeaveHeader INNER JOIN "
                           + " LeaveDetail ON LeaveHeader.LHID = LeaveDetail.LeaveHeaderId "
                           + " where LeaveHeader.Sno='" + ServiceNo + "' and DATEPART(year,LeaveHeader.FromDate)='" + fromyear + "' and LeaveHeader.Active !=0";

                ds = Detail(sql);
                DataTable dt = ds.Tables[0];
                foreach (DataRow item in dt.Rows)
                {
                    string c = item["Counts"].ToString();

                    if (c=="")
                    {
                        
                    }
                    else
                    {
                        Casual = Convert.ToInt32(item["Counts"].ToString());
                    }

                 
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Casual;
        }
        public int ReEngagementLeave(string Sno)
        {
            DataSet ds = new DataSet();
            int ReEngagementLeave = 0;
            try
            {
                string sql = "select SUM(LeaveDetail.ReEngagementLeave) as REL from LeaveHeader INNER JOIN LeaveDetail ON LeaveHeader.LHID = LeaveDetail.LeaveHeaderId "
                            + " where LeaveHeader.Sno = '" + Sno + "' and LeaveHeader.Active!=0 AND LeaveHeader.FromDate not in('2019-10-01','2018-10-01') ";

                ds = Detail(sql);
                DataTable dt = ds.Tables[0];
                foreach (DataRow item in dt.Rows)
                {
                    if (item["REL"].ToString()=="")
                    {
                        ReEngagementLeave = 0;
                    }
                    else
                    {
                        ReEngagementLeave = Convert.ToInt32(item["REL"].ToString());
                    }
                 
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ReEngagementLeave;
        }
        public int ReEngagementLeaveHistry(string Sno)
        {
            DataSet ds = new DataSet();
            int ReEngagementLeave = 0;
            try
            {
                string sql = "select max(LeaveDetail.ReEngagementLeave) as REL "
                    + " from LeaveHeader INNER JOIN LeaveDetail ON LeaveHeader.LHID = LeaveDetail.LeaveHeaderId  "
                    + " where LeaveHeader.Sno = '" + Sno + "' and LeaveHeader.Active!=0 And LeaveHeader.FromDate in('2019-10-01','2018-10-01')";

                ds = Detail(sql);
                DataTable dt = ds.Tables[0];
                foreach (DataRow item in dt.Rows)
                {
                    if (item["REL"].ToString() == "")
                    {
                        ReEngagementLeave = 0;
                    }
                    else
                    {
                        ReEngagementLeave = Convert.ToInt32(item["REL"].ToString());
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ReEngagementLeave;
        }
        public bool SQL_UpdateServicePersonnelProfile(string LivingStatusName, string MacAddress,int? UID, long? Sno)
        {
            ///Created By   : Flt Lt WAKY Wickramasinghe
            ///Created Date :2021.12.02
            ///Des: When account one certified the record update the P3hrms servicePersonnelProfile table  which related to Living IN Out status

            bool status = false; ;
            string ModifiedUser = Convert.ToString(UID);
            string SNO = Convert.ToString(Sno);
            DateTime ModifiedDate = DateTime.Now;
            SqlTransaction trans = null;
            try
            {
                using (var con = DALConnectionManager.openHrmis())
                {
                    con.Open();
                    trans = con.BeginTransaction();
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE ServicePersonnelProfile
                                      SET LivingIn_Out = @LivingStatusName,
                                      ModifiedMachine = @MacAddress,
                                      ModifiedBy = @ModifiedUser,
                                      ModifiedDate = @ModifiedDate 
                                      WHERE SNo= @Sno";
                        cmd.Transaction = trans;
                        cmd.Parameters.AddWithValue("@LivingStatusName", LivingStatusName);
                        cmd.Parameters.AddWithValue("@MacAddress", MacAddress);
                        cmd.Parameters.AddWithValue("@ModifiedUser", ModifiedUser);
                        cmd.Parameters.AddWithValue("@ModifiedDate", ModifiedDate);
                        cmd.Parameters.AddWithValue("@Sno", Sno);
                        cmd.ExecuteNonQuery();
                        trans.Commit();

                        status = true;
                    }
                }
                
                return status;
            }
            catch (Exception ex)
            {

                throw ex;
            }          
        }
        public DataTable getNokDetails(int? tableID, int detailsCollectCategory)
        {
            ///Created By   : Flt Lt WAKY Wickramasinghe
            ///Created Date :2021.12.08
            ///Des:Get Nok details which relevant to Living In Out header ID. this query used for two task. 
            ///    Case one get consider the Civil Status ID, Case two consider the Living Inout Id.case three consider the GSQ ID.

            DataTable dt = new DataTable();
            string SQL = "";
            try
            {
                switch (detailsCollectCategory)
                {
                    case (int)BAL.Enum.NOKSelectCategory.civilStatus:
                         SQL = " SELECT nch.NOKCHID,nch.Sno,nch.Location,nch.NOKStatus,nch.CivilStatusHeaderID,nch.ServiceTypeId,nch.WFDate,nch.RefNo,nch.Authority,nch.CreatedDate, " +
                             " ncd.NOKCDID,ncd.NOKAddress,ncd.NOKName,ncd.NOKChangeTo,ncd.District,ncd.GSDivision,ncd.NearestTown,ncd.PoliceStation,ncd.PostOffice,ncd.Authority,ncd.Remarks,ncd.CreatedDate,ncd.Active" +
                             " FROM NOKChangeHeader nch " +
                             " INNER JOIN NOKChangeDetails ncd " +
                             " ON nch.NOKCHID = ncd.NOKChangeHeadrerID " +
                             " WHERE  nch.CivilStatusHeaderID = '" + tableID + "' and nch.Active = 1 ";
                        break;
                    case (int)BAL.Enum.NOKSelectCategory.livinInOut:
                         SQL = " SELECT nch.NOKCHID,nch.Sno,nch.Location,nch.NOKStatus,nch.InOutHeaderID,nch.ServiceTypeId,nch.WFDate,nch.RefNo,nch.Authority,nch.CreatedDate, " +
                             " ncd.NOKCDID,ncd.NOKAddress,ncd.NOKName,ncd.NOKChangeTo,ncd.Province,ncd.District,ncd.GSDivision,ncd.NearestTown,ncd.PoliceStation,ncd.PostOffice,ncd.Authority,ncd.Remarks,ncd.CreatedDate,ncd.Active" +
                             " FROM NOKChangeHeader nch " +
                             " INNER JOIN NOKChangeDetails ncd " +
                             " ON nch.NOKCHID = ncd.NOKChangeHeadrerID " +
                             " WHERE  nch.InOutHeaderID = '" + tableID + "' and nch.Active = 1 ";
                        break;
                    case (int)BAL.Enum.NOKSelectCategory.gsqAllocateVacant:
                        SQL = " SELECT nch.NOKCHID,nch.Sno,nch.Location,nch.NOKStatus,nch.InOutHeaderID,nch.ServiceTypeId,nch.WFDate,nch.RefNo,nch.Authority,nch.CreatedDate, " +
                              " ncd.NOKCDID,ncd.NOKAddress,ncd.NOKName,ncd.NOKChangeTo,ncd.Province,ncd.District,ncd.GSDivision,ncd.NearestTown,ncd.PoliceStation,ncd.PostOffice,ncd.Authority,ncd.Remarks,ncd.CreatedDate,ncd.Active" +
                              " FROM NOKChangeHeader nch " +
                              " INNER JOIN NOKChangeDetails ncd " +
                              " ON nch.NOKCHID = ncd.NOKChangeHeadrerID " +
                              " WHERE  nch.GSQHeaderID = '" + tableID + "' and nch.Active = 1 ";
                        break;
                    case (int)BAL.Enum.NOKSelectCategory.NokChange:
                        SQL = " SELECT nch.NOKCHID,nch.Sno,nch.Location,nch.NOKStatus,nch.ServiceTypeId,nch.WFDate,nch.RefNo,nch.Authority,nch.CreatedDate, " +
                              " ncd.NOKCDID,ncd.NOKAddress,ncd.NOKName,ncd.NOKChangeTo,ncd.Province,ncd.District,ncd.GSDivision,ncd.NearestTown,ncd.PoliceStation,ncd.PostOffice,ncd.Remarks,ncd.CreatedDate,ncd.Active" +
                              " FROM NOKChangeHeader nch " +
                              " INNER JOIN NOKChangeDetails ncd " +
                              " ON nch.NOKCHID = ncd.NOKChangeHeadrerID " +
                              " WHERE  nch.NOKCHID = '" + tableID + "' and nch.Active = 1 ";
                        break;
                    default:
                        break;
                }
                

                      dt = Select(SQL);
            }
            catch (Exception ex )
            { 


                throw ex;
            }
            return dt;
        }
        public bool UpdatePreviousNokTypeId(long? Sno, int? UID, string MacAddress)
        {
            ///Created By   : Flt Lt WAKY Wickramasinghe
            ///Created Date :2021.12.09
            ///Des: Update the 
            /// 
            bool status = false;
            DateTime ModifiedDate = DateTime.Now;
            SqlTransaction trans = null;
            string ModifiedUser = Convert.ToString(UID);

            string SSNo = Sno.ToString();
            try
            {
                using (var con = DALConnectionManager.openHrmis())
                {
                    int NOKType = 2;
                    con.Open();
                    trans = con.BeginTransaction();
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE NOK_Change_Details
                                      SET NOKType = @NOKType,
                                      ModifiedUser = @ModifiedUser,
                                      ModifiedMachine = @MacAddress,
                                      ModifiedDate = @ModifiedDate 
                                      WHERE SNo= @Sno AND NOKType = 1";
                        cmd.Transaction = trans;
                        cmd.Parameters.AddWithValue("@NOKType", NOKType);
                        cmd.Parameters.AddWithValue("@MacAddress", MacAddress);
                        cmd.Parameters.AddWithValue("@ModifiedUser", ModifiedUser);
                        cmd.Parameters.AddWithValue("@ModifiedDate", ModifiedDate);
                        cmd.Parameters.AddWithValue("@Sno", SSNo);
                        cmd.ExecuteNonQuery();
                        trans.Commit();

                        status = true;
                    }
                }               
               
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return status;
        }
        public DataTable getCivilStatusDetails(int? CivilStatusHeaderID, int? CivilStatusCat)
        {
            ///Created By   : Flt Lt WAKY Wickramasinghe
            ///Created Date :2021.12.13
            ///Des: Get civil Status details from POR Db

            DataTable dt1 = new DataTable();            
            try
            {
                //switch (CivilStatusCat)
                //{
                //    ///Married
                //    case (int)ReportData.BAL.Enum.CivilStatusCategory.Marriage:
                //        SQL = "SELECT CSH.CSHID,CSH.Sno,CSH.SubCategoryId,CSH.Location,csh.RefNo,csh.Authority,CSH.CreatedDate, " +
                //              "CSH.Active,CSMD.CSMDID,CSMD.MarriageDate,CSMD.MarriageCertificateNo,CSMD.RegistarOfficeLocation,CSMD.CreatedDate " +
                //              "FROM CivilStatusHeader CSH " +
                //              "INNER JOIN CivilStatusMarriedDetails CSMD " +
                //              "ON CSH.CSHID =  CSMD.CivilStatusHeaderID " +
                //              "WHERE CSH.CSHID = '" + CivilStatusHeaderID + "' AND CSH.Active = 1 ";

                //        break;
                //    /// Divorce
                //    case (int)ReportData.BAL.Enum.CivilStatusCategory.Divorce:
                //        SQL = "SELECT CSH.CSHID,CSH.Sno,CSH.SubCategoryId,CSH.Location,csh.RefNo,csh.Authority,CSH.CreatedDate,CSH.Active,CSDD.CSDDID,CSDD.DivorceDtae, " +
                //             "CSDD.Place,CSDD.CourseCaseNo,CSDD.CreatedDate " +
                //             "FROM CivilStatusHeader CSH " +
                //             "INNER JOIN CivilStatusDivorceDetails CSDD " +
                //             "ON CSH.CSHID =  CSDD.CivilStatusHeaderID " +
                //             "WHERE CSH.CSHID = '" + CivilStatusHeaderID + "' AND CSH.Active = 1 ";

                //        break;
                //    // Widow
                //    case (int)ReportData.BAL.Enum.CivilStatusCategory.Widow:
                //        SQL = "SELECT CSH.CSHID,CSH.Sno,CSH.SubCategoryId,CSH.Location,csh.RefNo,csh.Authority,CSH.CreatedDate,CSH.Active,CSWD.CSWDID,CSWD.DeathCertificateNoOfSpouse, " +
                //         "CSWD.DateofDecease,CSWD.CreatedDate,CSWD.Active " +
                //         "FROM CivilStatusHeader CSH " +
                //         "INNER JOIN CivilStatusWidowDetails CSWD " +
                //         "ON CSH.CSHID =  CSWD.CivilStatusHeaderID " +
                //         "WHERE CSH.CSHID = '" + CivilStatusHeaderID + "' AND CSH.Active = 1 ";

                //        break;
                //    // Null&Void
                //    case (int)ReportData.BAL.Enum.CivilStatusCategory.NullVoid:

                //        SQL = "SELECT CSH.CSHID,CSH.Sno,CSH.SubCategoryId,CSH.Location,csh.RefNo,csh.Authority,CSH.CreatedDate,CSH.Active,CSDD.CSDDID,CSDD.DivorceDtae, " +
                //              "CSDD.Place,CSDD.CourseCaseNo,CSDD.CreatedDate " +
                //              "FROM CivilStatusHeader CSH " +
                //              "INNER JOIN CivilStatusDivorceDetails CSDD " +
                //              "ON CSH.CSHID =  CSDD.CivilStatusHeaderID " +
                //              "WHERE CSH.CSHID = '" + CivilStatusHeaderID + "' AND CSH.Active = 1 ";

                //        break;
                //    default:
                //        break;
                //}
                //dt = Select(SQL);

                using (var con = DALConnectionManager.openPOR())
                {
                    using (var cmd = con.CreateCommand())
                    {
                        switch (CivilStatusCat)
                        {
                            case (int)ReportData.BAL.Enum.CivilStatusCategory.Marriage:

                                cmd.CommandText = @"SELECT CSH.CSHID,CSH.Sno,CSH.SubCategoryId,CSH.Location,csh.RefNo,csh.Authority,CSH.CreatedDate,CSH.Active,CSMD.CSMDID,CSMD.MarriageDate,
                                                           CSMD.MarriageCertificateNo,CSMD.RegistarOfficeLocation,CSMD.CreatedDate 
                                                    FROM CivilStatusHeader CSH 
                                                    INNER JOIN CivilStatusMarriedDetails CSMD ON CSH.CSHID = CSMD.CivilStatusHeaderID 
                                                    WHERE CSH.CSHID = @CivilStatusHeaderID AND CSH.Active = 1";

                                cmd.Parameters.AddWithValue("@CivilStatusHeaderID", CivilStatusHeaderID);
                                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                                adapter.Fill(dt1);
                                break;

                            case (int)ReportData.BAL.Enum.CivilStatusCategory.Divorce:

                                cmd.CommandText = @"SELECT CSH.CSHID,CSH.Sno,CSH.SubCategoryId,CSH.Location,csh.RefNo,csh.Authority,CSH.CreatedDate,CSH.Active,CSDD.CSDDID,CSDD.DivorceDtae,
                                                   CSDD.Place,CSDD.CourseCaseNo,CSDD.CreatedDate
                                                   FROM CivilStatusHeader CSH INNER JOIN CivilStatusDivorceDetails CSDD ON CSH.CSHID =  CSDD.CivilStatusHeaderID
                                                   WHERE CSH.CSHID = @CivilStatusHeaderID  AND CSH.Active = 1";


                                cmd.Parameters.AddWithValue("@CivilStatusHeaderID", CivilStatusHeaderID);
                                SqlDataAdapter adapter2 = new SqlDataAdapter(cmd);
                                adapter2.Fill(dt1);
                                break;

                            case (int)ReportData.BAL.Enum.CivilStatusCategory.Widow:

                                cmd.CommandText = @"SELECT CSH.CSHID,CSH.Sno,CSH.SubCategoryId,CSH.Location,csh.RefNo,csh.Authority,CSH.CreatedDate,CSH.Active,CSMD.CSWDID,CSMD.DeathCertificateNoOfSpouse,
                                                   CSMD.DateofDecease,CSMD.CreatedDate,CSMD.Active
                                                   FROM CivilStatusHeader CSH INNER JOIN CivilStatusWidowDetails CSMD ON CSH.CSHID =  CSMD.CivilStatusHeaderID
                                                   WHERE CSH.CSHID = @CivilStatusHeaderID AND CSH.Active = 1";


                                cmd.Parameters.AddWithValue("@CivilStatusHeaderID", CivilStatusHeaderID);
                                SqlDataAdapter adapter3 = new SqlDataAdapter(cmd);
                                adapter3.Fill(dt1);
                                break;

                            case (int)ReportData.BAL.Enum.CivilStatusCategory.NullVoid:

                                cmd.CommandText = @"SELECTCSH.CSHID,CSH.Sno,CSH.SubCategoryId,CSH.Location,csh.RefNo,csh.Authority,CSH.CreatedDate,CSH.Active,
                                                   CSDD.CSDDID,CSDD.DivorceDtae,CSDD.Place,CSDD.CourseCaseNo,CSDD.CreatedDate
                                                   FROM CivilStatusHeader CSH INNER JOIN CivilStatusDivorceDetails CSDD ON CSH.CSHID =  CSDD.CivilStatusHeaderID
                                                   WHERE CSH.CSHID = @CivilStatusHeaderID AND CSH.Active = 1";


                                cmd.Parameters.AddWithValue("@CivilStatusHeaderID", CivilStatusHeaderID);
                                SqlDataAdapter adapter4 = new SqlDataAdapter(cmd);
                                adapter4.Fill(dt1);
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return dt1;
        }      
        public bool UpdateMarriageControlDetaild(long? Sno, int? UID, string MacAddress, BALNOK_Change_Details objNOK_Change_Details,DataTable dt, int? CivilStatusCat)
        {
            ///Created By   : Flt Lt WAKY Wickramasinghe
            ///Created Date :2021.12.09
            ///Des: Update the 
            /// 
            bool status = false;
            SqlTransaction trans = null;

            DateTime ModifiedDate = DateTime.Now;
            DateTime MarriedDate = DateTime.Now;
            DateTime DivorceDate = new DateTime();
            DateTime DateOfDeath = new DateTime();
            DataTable dt1 = new DataTable();
            DataTable dt2 = new DataTable();
            
            string ModifiedUser = Convert.ToString(UID);           
            string MarriedCertificateID = "";            
            string MarriedPlace = "";
            string AuthRef = "";
            string MarriagePOR = "";
            string MarriageID = "";                   
            string DivorcePOR = "";              
            string DeathPOR = "";
            int MarriedStatus = 0;
            string ConSNo = Convert.ToString(Sno);

            try
            {
                
                using (var con = DALConnectionManager.openHrmis())
                {
                    int NOKType = 2;
                    con.Open();
                    trans = con.BeginTransaction();
                    using (var cmd = con.CreateCommand())
                    {
                        #region NOK related Area
                        cmd.CommandText = @"UPDATE NOK_Change_Details
                                      SET NOKType = @NOKType,
                                      ModifiedUser = @ModifiedUser,
                                      ModifiedMachine = @MacAddress,
                                      ModifiedDate = @ModifiedDate 
                                      WHERE SNo= @Sno AND NOKType = 1";
                        cmd.Transaction = trans;
                        cmd.Parameters.AddWithValue("@NOKType", NOKType);
                        cmd.Parameters.AddWithValue("@MacAddress", MacAddress);
                        cmd.Parameters.AddWithValue("@ModifiedUser", ModifiedUser);
                        cmd.Parameters.AddWithValue("@ModifiedDate", ModifiedDate);
                        cmd.Parameters.AddWithValue("@Sno", ConSNo);
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = @"INSERT INTO NOK_Change_Details (NOKID,SNo,NOKType,Relationship,NOKName,NOKAddress,District,GramaseDiv,NearPoliceSta,NearTown,
                                            NearPostOff,WEFDate,AuthRefNo,PORRefNo,LivingStatus,Status,CreatedUser,CreatedDate,CreatedMachine) VALUES (@NOKID,@Sno,@NOKType2,
                                            @Relationship,@NOKName,@NOKAddress,@District,@GramaseDiv,@NearPoliceSta,@NearTown,@NearPostOff,@WEFDate,@AuthRefNo,@PORRefNo,
                                            @LivingStatus,@Status,@CreatedUser,@CreatedDate,@CreatedMachine)";

                        cmd.Transaction = trans;
                        cmd.Parameters.AddWithValue("@NOKID", objNOK_Change_Details.NOKID);
                        //cmd.Parameters.AddWithValue("@SNo", objNOK_Change_Details.SNo);
                        cmd.Parameters.AddWithValue("@NOKType2", 1);
                        cmd.Parameters.AddWithValue("@Relationship", objNOK_Change_Details.Relationship);
                        cmd.Parameters.AddWithValue("@NOKName", objNOK_Change_Details.NOKName);
                        cmd.Parameters.AddWithValue("@NOKAddress", objNOK_Change_Details.NOKAddress);
                        cmd.Parameters.AddWithValue("@District", objNOK_Change_Details.District);
                        cmd.Parameters.AddWithValue("@GramaseDiv", objNOK_Change_Details.GramaseDiv);
                        cmd.Parameters.AddWithValue("@NearPoliceSta", objNOK_Change_Details.NearPoliceSta);
                        cmd.Parameters.AddWithValue("@NearTown", objNOK_Change_Details.NearTown);
                        cmd.Parameters.AddWithValue("@NearPostOff", objNOK_Change_Details.NearPostOff);
                        cmd.Parameters.AddWithValue("@WEFDate", objNOK_Change_Details.WEFDate);
                        cmd.Parameters.AddWithValue("@AuthRefNo", objNOK_Change_Details.AuthRefNo);
                        cmd.Parameters.AddWithValue("@PORRefNo", objNOK_Change_Details.PORRefNo);
                        cmd.Parameters.AddWithValue("@LivingStatus", 1);
                        cmd.Parameters.AddWithValue("@Status", objNOK_Change_Details.Status);
                        cmd.Parameters.AddWithValue("@CreatedUser", objNOK_Change_Details.CreatedUser);
                        cmd.Parameters.AddWithValue("@CreatedDate", objNOK_Change_Details.CreatedDate);
                        cmd.Parameters.AddWithValue("@CreatedMachine", objNOK_Change_Details.CreatedMachine);
                        cmd.ExecuteNonQuery();
                        #endregion

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                        switch (CivilStatusCat)
                        {
                            case (int)ReportData.BAL.Enum.CivilStatusCategory.Marriage:

                                #region Marriage
                                MarriedStatus = 1;

                                //// Assign Marriage details to variables
                                foreach (DataRow row in dt.Rows)
                                {
                                    MarriedCertificateID = row["MarriageCertificateNo"].ToString();
                                    MarriedDate = Convert.ToDateTime(row["MarriageDate"]);
                                    MarriedPlace = row["RegistarOfficeLocation"].ToString();
                                    AuthRef = row["Authority"].ToString();
                                    MarriagePOR = row["RefNo"].ToString();
                                    SNO = row["Sno"].ToString();
                                }

                                cmd.CommandText = @"SELECT MarriageID FROM Marriage WHERE SNo = @Sno AND MarriedStatus = 0 AND Status = 1 ";
                                cmd.Transaction = trans;
                                //cmd.Parameters.AddWithValue("@SNO", SNO);
                                adapter.Fill(dt2);

                                if (dt2.Rows[0]["MarriageID"].ToString() != "")
                                {
                                    MarriageID = dt2.Rows[0]["MarriageID"].ToString();

                                    /// Update the Marriage table following coloum
                                    cmd.CommandText = @"UPDATE Marriage
                                      SET MarriedCertificateID = @MarriedCertificateID,
                                      MarriedDate = @MarriedDate,
                                      MarriedPlace = @MarriedPlace,
                                      MarriedStatus = @MarriedStatus,
                                      AuthRef = @AuthRef,
                                      MarriagePOR = @MarriagePOR,
                                      Status = @StatusM,
                                      MarriedCertificateUpdateStatus = @MarriedCertificateUpdateStatus,
                                      ModifiedUser = @ModifiedUser,
                                      ModifiedMachine = @ModifiedMachine,
                                      ModifiedDate = @ModifiedDate
                                      WHERE MarriageID = @MarriageID";

                                    cmd.Transaction = trans;
                                    cmd.Parameters.AddWithValue("@MarriedCertificateID", MarriedCertificateID);
                                    cmd.Parameters.AddWithValue("@MarriedDate", MarriedDate);
                                    cmd.Parameters.AddWithValue("@MarriedPlace", MarriedPlace);
                                    cmd.Parameters.AddWithValue("@MarriedStatus", MarriedStatus);
                                    cmd.Parameters.AddWithValue("@AuthRef", AuthRef);
                                    cmd.Parameters.AddWithValue("@MarriagePOR", MarriagePOR);
                                    cmd.Parameters.AddWithValue("@StatusM", 1);
                                    cmd.Parameters.AddWithValue("@MarriedCertificateUpdateStatus", 10);
                                    cmd.Parameters.AddWithValue("@MarriageID", MarriageID);
                                    //cmd.Parameters.AddWithValue("@ModifiedUser", ModifiedUser);
                                    cmd.Parameters.AddWithValue("@ModifiedMachine", MacAddress);
                                   // cmd.Parameters.AddWithValue("@ModifiedDate", ModifiedDate);
                                    cmd.ExecuteNonQuery();

                                }

                                #endregion

                                break;

                            case (int)ReportData.BAL.Enum.CivilStatusCategory.Divorce:

                                #region Divorce
                                MarriedStatus = 2;

                                //// Assign Divorce details to variables
                                foreach (DataRow row in dt.Rows)
                                {
                                    DivorceDate = Convert.ToDateTime(row["DivorceDtae"]);
                                    DivorcePOR = row["RefNo"].ToString();
                                    SNO = row["Sno"].ToString();
                                }

                                cmd.CommandText = @"SELECT MarriageID FROM Marriage WHERE SNo = @Sno AND MarriedStatus = 1 AND Status = 1 ";
                                cmd.Transaction = trans;
                                //cmd.Parameters.AddWithValue("@SNO", SNO);
                                adapter.Fill(dt2);

                                if (dt2.Rows[0]["MarriageID"].ToString() != "")
                                {
                                    MarriageID = dt2.Rows[0]["MarriageID"].ToString();

                                    cmd.CommandText = @"UPDATE Marriage
                                      SET MarriedStatus = @MarriedStatus,
                                      DivorceDate = @DivorceDate,
                                      DivorcePOR = @DivorcePOR,
                                      Status = @StatusD,
                                      ModifiedUser = @ModifiedUser,
                                      ModifiedMachine = @MacAddress,
                                      ModifiedDate = @ModifiedDate
                                      WHERE MarriageID = @MarriageID ";

                                    cmd.Transaction = trans;
                                    cmd.Parameters.AddWithValue("@MarriedStatus", MarriedStatus);
                                    cmd.Parameters.AddWithValue("@DivorceDate", DivorceDate);
                                    cmd.Parameters.AddWithValue("@DivorcePOR", DivorcePOR);
                                    cmd.Parameters.AddWithValue("@StatusD", 1);
                                    cmd.Parameters.AddWithValue("@MarriageID", MarriageID);
                                    //cmd.Parameters.AddWithValue("@ModifiedUser", ModifiedUser);
                                    cmd.Parameters.AddWithValue("@ModifiedMachine", MacAddress);
                                    //cmd.Parameters.AddWithValue("@ModifiedDate", ModifiedDate);
                                    cmd.ExecuteNonQuery();
                                   
                                }

                                #endregion

                                break;

                            case (int)ReportData.BAL.Enum.CivilStatusCategory.Widow:

                                #region Widow
                                MarriedStatus = 3;

                                //// Assign Widow details to variables

                                foreach (DataRow row in dt.Rows)
                                {
                                    DateOfDeath = Convert.ToDateTime(row["DateofDecease"]);
                                    DeathPOR = row["RefNo"].ToString();
                                    SNO = row["Sno"].ToString();
                                }

                                cmd.CommandText = @"SELECT MarriageID FROM Marriage WHERE SNo = @Sno AND MarriedStatus = 1 AND Status = 1 ";
                                cmd.Transaction = trans;
                                //cmd.Parameters.AddWithValue("@SNO", SNO);
                                adapter.Fill(dt2);

                                if (dt2.Rows[0]["MarriageID"].ToString() != "")
                                {
                                    MarriageID = dt2.Rows[0]["MarriageID"].ToString();

                                    cmd.CommandText = @"UPDATE Marriage
                                      SET MarriedStatus = @MarriedStatus,
                                      DateOfDeath = @DateOfDeath,
                                      DeathPOR = @DeathPOR,
                                      Status = @StatusW,
                                      ModifiedUser = @ModifiedUser,
                                      ModifiedMachine = @MacAddress,
                                      ModifiedDate = @ModifiedDate
                                      WHERE MarriageID = @MarriageID ";

                                    cmd.Transaction = trans;
                                    cmd.Parameters.AddWithValue("@MarriedStatus", MarriedStatus);
                                    cmd.Parameters.AddWithValue("@DateOfDeath", DateOfDeath);
                                    cmd.Parameters.AddWithValue("@DeathPOR", DeathPOR);
                                    cmd.Parameters.AddWithValue("@StatusW", 1);
                                    cmd.Parameters.AddWithValue("@MarriageID", MarriageID);
                                    //cmd.Parameters.AddWithValue("@ModifiedUser", ModifiedUser);
                                    cmd.Parameters.AddWithValue("@ModifiedMachine", MacAddress);
                                    //cmd.Parameters.AddWithValue("@ModifiedDate", ModifiedDate);
                                    cmd.ExecuteNonQuery();

                                  }

                                #endregion

                                break;
                            case (int)ReportData.BAL.Enum.CivilStatusCategory.NullVoid:

                                #region NullVoid
                                MarriedStatus = 4;

                                //// Assign NullVoid details to variables

                                foreach (DataRow row in dt.Rows)
                                {
                                    DateOfDeath = Convert.ToDateTime(row["DateofDecease"]);
                                    DeathPOR = row["RefNo"].ToString();
                                    SNO = row["Sno"].ToString();
                                }

                                cmd.CommandText = @"SELECT MarriageID FROM Marriage WHERE SNo = @Sno AND MarriedStatus = 1 AND Status = 1 ";
                                cmd.Transaction = trans;
                                //cmd.Parameters.AddWithValue("@SNO", SNO);
                                adapter.Fill(dt2);

                                if (dt2.Rows[0]["MarriageID"].ToString() != "")
                                {
                                    MarriageID = dt2.Rows[0]["MarriageID"].ToString();

                                    cmd.CommandText = @"UPDATE Marriage
                                      SET MarriedStatus = @MarriedStatus,
                                      DivorceDate = @DivorceDate,
                                      DivorcePOR = @DivorcePOR,
                                      Status = @StatusN,
                                      ModifiedUser = @ModifiedUser,
                                      ModifiedMachine = @MacAddress,
                                      ModifiedDate = @ModifiedDate
                                      WHERE MarriageID = @MarriageID ";

                                    cmd.Transaction = trans;
                                    cmd.Parameters.AddWithValue("@MarriedStatus", MarriedStatus);
                                    cmd.Parameters.AddWithValue("@DivorceDate", DivorceDate);
                                    cmd.Parameters.AddWithValue("@DivorcePOR", DivorcePOR);
                                    cmd.Parameters.AddWithValue("@StatusN", 1);
                                    cmd.Parameters.AddWithValue("@MarriageID", MarriageID);
                                    //cmd.Parameters.AddWithValue("@ModifiedUser", ModifiedUser);
                                    cmd.Parameters.AddWithValue("@ModifiedMachine", MacAddress);
                                    //cmd.Parameters.AddWithValue("@ModifiedDate", ModifiedDate);
                                    cmd.ExecuteNonQuery();
                                   
                                }

                                #endregion

                                break;
                        }

                        cmd.CommandText = @"UPDATE ServicePersonnelProfile
                                      SET Marriage_Status = @Marriage_Status,
                                      ModifiedMachine = @MacAddress,
                                      ModifiedDate =@ModifiedDate 
                                      WHERE SNo= @Sno";
                        cmd.Transaction = trans;
                        cmd.Parameters.AddWithValue("@Marriage_Status", MarriedStatus);
                        //cmd.Parameters.AddWithValue("@MacAddress", MacAddress);
                        //cmd.Parameters.AddWithValue("@ModifiedDate", ModifiedDate);
                        //cmd.Parameters.AddWithValue("@Sno", SNO);
                        cmd.ExecuteNonQuery();


                        trans.Commit();
                        status = true;

                    }
                }

            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw ex;
            }

            return status;
        }
        public bool UpdateServicePersonalProfileMarriageStatus(string Sno, string Marriage_Status,string MacAddress) {

            ///Created By   : Flt Lt WAKY Wickramasinghe
            ///Created Date :2021.12.17
            ///Des: Update the HRMIS ServicePersonal profile Marriage_Status column.

            bool status = false;
            DateTime ModifiedDate = DateTime.Now;
            SqlTransaction trans = null;
            try
            {
                //string ModifiedUser = Convert.ToString(UID);
                //string SNO = Convert.ToString(Sno);

                using (var con = DALConnectionManager.openHrmis())
                {
                    con.Open();
                    trans = con.BeginTransaction();

                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE ServicePersonnelProfile
                                      SET Marriage_Status = @Marriage_Status,
                                      ModifiedMachine = @MacAddress,
                                      ModifiedDate =@ModifiedDate 
                                      WHERE SNo= @Sno";
                        cmd.Transaction = trans;
                        cmd.Parameters.AddWithValue("@Marriage_Status", Marriage_Status);
                        cmd.Parameters.AddWithValue("@MacAddress", MacAddress);
                        cmd.Parameters.AddWithValue("@ModifiedDate", ModifiedDate);
                        cmd.Parameters.AddWithValue("@Sno", Sno);
                        cmd.ExecuteNonQuery();
                        trans.Commit();

                        status = true;
                    }
                }                               

                return status;
            }
            catch (Exception)
            {

                throw;
            }
        }        
        public long GenerateKey(string table, string field)
        {
            ///Created BY   : FLT LT WAKY Wickramasinghe
            ///Created Date : 2021/12/08
            /// Description : This function is to generate NOK id for insert NOk details

            DataTable dt = new DataTable();
            long keyInt = 0;
            try
            {
                string sqlQuery = "SELECT MAX(CONVERT(bigint,SUBSTRING(" + field + ", CHARINDEX('/', " + field + ") + 1, LEN(" + field + ")))) AS keyInt FROM " + table;

                dt = SelectHrmis(sqlQuery);

                if (dt.Rows[0]["keyInt"].ToString().Trim() != "")
                    keyInt = long.Parse(dt.Rows[0]["keyInt"].ToString()) + 1;
                else
                    keyInt = 1;


                return keyInt;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public DataTable CallSP(long Sno)
        {
            ///Created BY   : FLT LT WAKY Wickramasinghe
            ///Created Date : 2021/08/26
            /// Description : Call CivilFlowStatus Sp , when seraching the details

            DataTable dt = new DataTable();
            try
            {
                SqlConnection Connection = DALConnectionManager.open();
                SqlCommand command = Connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "CivilFlowStatus";
                command.Parameters.AddWithValue("@Sno", Sno);
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
        public DataTable CallLivingINOutSP(long Sno)
        {
              ///Created BY   : FLT LT WAKY Wickramasinghe
             ///Created Date  : 2021/08/26
            /// Description   : Call LivingInOutStatus Sp

            DataTable dt = new DataTable();
            try
            {
                SqlConnection Connection = DALConnectionManager.open();
                SqlCommand command = Connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "LivingInOutStatus";
                command.Parameters.AddWithValue("@Sno", Sno);
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
        public DataTable CallGSQSP(long Sno)
        {
            ///Created BY   : FLT LT WAKY Wickramasinghe
            ///Created Date  : 2022/03/16
            /// Description   : Call LivingInOutStatus Sp

            DataTable dt = new DataTable();
            try
            {
                SqlConnection Connection = DALConnectionManager.open();
                SqlCommand command = Connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "GSQFlowStatus";
                command.Parameters.AddWithValue("@Sno", Sno);
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
        public DataTable OICPROCESS(long Sno)
        {
            ///Created BY   : 
            ///Created Date  : 
            /// Description   : 

            DataTable dt = new DataTable();
            try
            {
                SqlConnection Connection = DALConnectionManager.open();
                SqlCommand command = Connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "OICPROCES";
                command.Parameters.AddWithValue("@Sno", Sno);
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
        public DataTable CallF121SP(long Sno)
        {
            ///Created BY   : 
            ///Created Date  : 
            /// Description   : 

            DataTable dt = new DataTable();
            try
            {
                SqlConnection Connection = DALConnectionManager.open();
                SqlCommand command = Connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "F121";
                command.Parameters.AddWithValue("@Sno", Sno);
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
        public DataTable CallGSQRejectSP()
        {
            ///Created BY     : FLT LT WAKY Wickramasinghe
            ///Created Date   : 2022/08/10
            /// Description   : Call GSQReject Sp

            DataTable dt = new DataTable();
            try
            {
                SqlConnection Connection = DALConnectionManager.open();
                SqlCommand command = Connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "GSQRejectList";
                //command.Parameters.AddWithValue("@Sno", Sno);
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
        public DataTable CallNOKSP(long Sno)
        {
            ///Created BY   : FLT LT WAKY Wickramasinghe
            ///Created Date  : 2021/08/26
            /// Description   : Call LivingInOutStatus Sp

            DataTable dt = new DataTable();
            try
            {
                SqlConnection Connection = DALConnectionManager.open();
                SqlCommand command = Connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "NOKFlowStatus";
                command.Parameters.AddWithValue("@Sno", Sno);
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
        public DataTable CallNOKRejectSP()
        {
            ///Created BY   : FLT LT WAKY Wickramasinghe
            ///Created Date  : 2023/03/14
            /// Description   : Call Reject Sp

            DataTable dt = new DataTable();
            try
            {
                SqlConnection Connection = DALConnectionManager.open();
                SqlCommand command = Connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "NOKRejectList";
                //command.Parameters.AddWithValue("@Sno", Sno);
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
        public DataTable CallPersonalContatSP(long Sno)
        {
            ///Created BY   : Sqn ldr WAKY Wickramasinghe
            ///Created Date  : 2023/05/23
            /// Description   : Call Personal Contact Sp

            DataTable dt = new DataTable();
            try
            {
                SqlConnection Connection = DALConnectionManager.open();
                SqlCommand command = Connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PersonContactFlowStatus";
                command.Parameters.AddWithValue("@Sno", Sno);
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


        public DataTable CallDutySP(long Sno)
        {
            ///Created BY   : Sqn ldr priyantha
            ///Created Date  : 2023/05/23
            /// Description   : secondary duty

            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection Connection = DALConnectionManager.open())
                {
                    SqlCommand command = Connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "SecondaryDutyFlowStatus";
                    command.Parameters.AddWithValue("@Sno", Sno);
                    //command.Parameters.AddWithValue("@PsnConHeaderID");
                    command.CommandTimeout = 1000;
                    SqlDataAdapter adp = new SqlDataAdapter(command);

                    adp.Fill(dt);
                }


                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataTable CallDutyRejectSP()
        {
            ///Created BY   : FLT LT WAKY Wickramasinghe
            ///Created Date  : 2023/03/14
            /// Description   : Call Reject Sp

            DataTable dt = new DataTable();
            try
            {
                SqlConnection Connection = DALConnectionManager.open();
                SqlCommand command = Connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "SecondaryDutyRejectFlowStatus";
               // command.Parameters.AddWithValue("@Sno", Sno);
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




        public DataTable CallChildDetailsSP(long Sno,int PsnConHeaderID)
        {
            ///Created BY   : Sqn ldr WAKY Wickramasinghe
            ///Created Date  : 2023/05/23
            /// Description   : Call Child Details Sp

            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection Connection = DALConnectionManager.open())
                {
                    SqlCommand command = Connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "PersonChildFlowStatus";
                    command.Parameters.AddWithValue("@Sno", Sno);
                    command.Parameters.AddWithValue("@PsnConHeaderID", PsnConHeaderID);
                    command.CommandTimeout = 1000;
                    SqlDataAdapter adp = new SqlDataAdapter(command);

                    adp.Fill(dt);
                }            

                
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable CallPsnContactRejectSP()
        {
            ///Created BY   : FLT LT WAKY Wickramasinghe
            ///Created Date  : 2023/03/14
            /// Description   : Call Reject Sp

            DataTable dt = new DataTable();
            try
            {
                SqlConnection Connection = DALConnectionManager.open();
                SqlCommand command = Connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PersonConRejectList";
                //command.Parameters.AddWithValue("@Sno", Sno);
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
        public DataTable CallPsnChildRejectSP()
        {
            ///Created BY   : FLT LT WAKY Wickramasinghe
            ///Created Date  : 2023/03/14
            /// Description   : Call Reject Sp

            DataTable dt = new DataTable();
            try
            {
                SqlConnection Connection = DALConnectionManager.open();
                SqlCommand command = Connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PersonChildRejectList";
                //command.Parameters.AddWithValue("@Sno", Sno);
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
        public DataTable getCivilStatusSearchDetails(string FromDate, string ToDate, string SearchLoc,int CSCID,int recordType)
        {
            ///Created By   : Flt Lt WAKY Wickramasinghe
            ///Created Date :2022-08-03
            ///Des: Searching Option for history record in Civil Status

            DataTable dt = new DataTable();
            string SQL = "";
            try
            {
                if (FromDate != "" && ToDate != "" && SearchLoc != "" && CSCID != 0 && recordType != 0)
                {
                    SQL = "SELECT urm.ServiceNo,urm.Rank,urm.Name,urm.CategoryName,urm.Authority,urm.Location,urm.CreatedDate,urm.RoleName,urm.CivilStatusHeaderID " +
                          "FROM Vw_CurrentUserRoleMarried urm " +
                          "WHERE urm.Location = '" + SearchLoc + "' AND urm.SubCategoryId = '" + CSCID + "' AND urm.HeaderActive = 1 AND urm.RecordStatusID = '" + recordType + "' AND urm.CreatedDate BETWEEN CONVERT(DATETIME,'" + FromDate + "',102) AND CONVERT(DATETIME,'" + ToDate + "',102) Order By urm.CreatedDate asc ";

                }
                else if (FromDate != "" && ToDate != "" && CSCID != 0 && recordType != 0)
                {
                    SQL = "SELECT urm.ServiceNo,urm.Rank,urm.Name,urm.CategoryName,urm.Authority,urm.Location,urm.CreatedDate,urm.RoleName,urm.CivilStatusHeaderID " +
                          "FROM Vw_CurrentUserRoleMarried urm " +
                          "WHERE urm.SubCategoryId = '" + CSCID + "' AND urm.HeaderActive = 1 urm.RecordStatusID = '" + recordType + "' AND urm.CreatedDate BETWEEN CONVERT(DATETIME,'" + FromDate + "',102) AND CONVERT(DATETIME,'" + ToDate + "',102) Order By urm.CreatedDate asc ";


                }
                else if (FromDate != "" && SearchLoc != "" && CSCID != 0 && recordType != 0)
                {
                    SQL = "SELECT urm.ServiceNo,urm.Rank,urm.Name,urm.CategoryName,urm.Authority,urm.Location,urm.CreatedDate,urm.RoleName,urm.CivilStatusHeaderID " +
                          "FROM Vw_CurrentUserRoleMarried urm " +
                          "WHERE urm.Location = '" + SearchLoc + "' AND urm.HeaderActive = 1 AND urm.SubCategoryId = '" + CSCID + "' urm.RecordStatusID = '" + recordType + "' AND CONVERT(date,urm.CreatedDate, 102) = CONVERT(DATETIME,'" + FromDate + "',102) Order By urm.CreatedDate asc ";

                }
                else if (ToDate != "" && SearchLoc != "" && CSCID != 0 && recordType != 0)
                {
                    SQL = "SELECT urm.ServiceNo,urm.Rank,urm.Name,urm.CategoryName,urm.Authority,urm.Location,urm.CreatedDate,urm.RoleName,urm.CivilStatusHeaderID " +
                          "FROM Vw_CurrentUserRoleMarried urm " +
                          "WHERE urm.Location = '" + SearchLoc + "' AND urm.HeaderActive = 1 urm.RecordStatusID = '" + recordType + "' AND urm.SubCategoryId = '" + CSCID + "' AND CONVERT(date,urm.CreatedDate, 102) = CONVERT(DATETIME,'" + ToDate + "',102) Order By urm.CreatedDate asc ";

                }

                dt = Select(SQL);

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return dt;
        }
        public DataTable getLivingStatusSearchDetails(string FromDate, string ToDate, string SearchLoc, int LSID, int recordType)
        {
            ///Created By   : Flt Lt WAKY Wickramasinghe
            ///Created Date :2022-08-03
            ///Des: Searching Option for history record in Civil Status

            DataTable dt = new DataTable();
            string SQL = "";
            try
            {
                if (FromDate != "" && ToDate != "" && SearchLoc != "" && LSID != 0 && recordType != 0)
                {
                    SQL = "SELECT cul.ServiceNo,cul.Rank,cul.Name,cul.LivingStatusShortName,cul.Authority,cul.Location,cul.CreatedDate,cul.RoleName,cul.RSID,cul.InOut_ID,cul.Active " +
                          "FROM Vw_CurrentUserLiving cul " +
                          "WHERE cul.Location = '" + SearchLoc + "' AND cul.LSID = '" + LSID + "' AND cul.Active = 1 AND cul.RSID = '" + recordType + "' AND cul.CreatedDate BETWEEN CONVERT(DATETIME,'" + FromDate + "',102) AND CONVERT(DATETIME,'" + ToDate + "',102) Order By cul.CreatedDate asc ";

                }
                else if (FromDate != "" && ToDate != "" && LSID != 0 && recordType != 0)
                {
                    SQL = "SELECT cul.ServiceNo,cul.Rank,cul.Name,cul.LivingStatusShortName,cul.Authority,cul.Location,cul.CreatedDate,cul.RoleName,cul.RSID,cul.InOut_ID,cul.Active " +
                          "FROM Vw_CurrentUserLiving cul " +
                          "WHERE cul.Location = '" + SearchLoc + "' AND cul.LSID = '" + LSID + "' AND cul.Active = 1 AND cul.RSID = '" + recordType + "' AND cul.CreatedDate BETWEEN CONVERT(DATETIME,'" + FromDate + "',102) AND CONVERT(DATETIME,'" + ToDate + "',102) Order By cul.CreatedDate asc ";

                }
                else if (FromDate != "" && SearchLoc != "" && LSID != 0 && recordType != 0)
                {
                    SQL = "SELECT cul.ServiceNo,cul.Rank,cul.Name,cul.LivingStatusShortName,cul.Authority,cul.Location,cul.CreatedDate,cul.RoleName,cul.RSID,cul.InOut_ID,cul.Active " +
                          "FROM Vw_CurrentUserLiving cul " +
                          "WHERE cul.Location = '" + SearchLoc + "' AND cul.Active = 1 AND cul.LSID = '" + LSID + "' AND cul.RSID = '" + recordType + "' AND CONVERT(date,cul.CreatedDate, 102) = CONVERT(DATETIME,'" + FromDate + "',102) Order By cul.CreatedDate asc ";

                }
                else if (ToDate != "" && SearchLoc != "" && LSID != 0 && recordType != 0)
                {
                    SQL = "SELECT cul.ServiceNo,cul.Rank,cul.Name,cul.LivingStatusShortName,cul.Authority,cul.Location,cul.CreatedDate,cul.RoleName,cul.RSID,cul.InOut_ID,cul.Active " +
                          "FROM Vw_CurrentUserLiving cul " +
                          "WHERE cul.Location = '" + SearchLoc + "' AND cul.Active = 1 AND cul.RSID = '" + recordType + "' AND cul.LSID = '" + LSID + "' AND CONVERT(date,cul.CreatedDate, 102) = CONVERT(DATETIME,'" + ToDate + "',102) Order By cul.CreatedDate asc ";

                }

                dt = Select(SQL);

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return dt;
        }
        public DataTable getGSQStatusSearchDetails(string FromDate, string ToDate, string SearchLoc, int LSID, int recordType)
        {
            ///Created By   : Flt Lt WAKY Wickramasinghe
            ///Created Date :2022-09-01
            ///Des: Searching Option for history record in GSQ Allocation/vacent

            DataTable dt = new DataTable();           
            try
            {
                SqlConnection Connection = DALConnectionManager.open();
                SqlCommand command = Connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "GSQAdvancedSearch";
                command.Parameters.AddWithValue("@SearchLoc", SearchLoc);
                command.Parameters.AddWithValue("@GSQStatus", LSID);
                command.Parameters.AddWithValue("@recordType",recordType);
                command.Parameters.AddWithValue("@FromDate",FromDate);
                command.Parameters.AddWithValue("@ToDate",ToDate);
                command.CommandTimeout = 1000;
                SqlDataAdapter adp = new SqlDataAdapter(command);

                adp.Fill(dt);              

            }
            catch (Exception ex)
            {

                throw ex;
            }

            return dt;
        }
        public DataTable getPsrSearchDetails(string FromDate, string ToDate, string SearchLoc, int MSCID, int recordType,int RoleId)
        {
            ///Created By   : Flt Lt WAKY Wickramasinghe
            ///Created Date :2022-08-03
            ///Des: Searching Option for history record in Civil Status

            DataTable dt = new DataTable();
            
            try
            {
                SqlConnection Connection = DALConnectionManager.open();
                SqlCommand command = Connection.CreateCommand();
                switch (MSCID)
                {
                    case (int)ReportData.BAL.Enum.PORMasterSubCategory.DetailOfChildBirth:

                        
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "PersonChildAdvancedSearch";
                        command.Parameters.AddWithValue("@SearchLoc", SearchLoc);
                        command.Parameters.AddWithValue("@MSCID", MSCID);
                        command.Parameters.AddWithValue("@recordType", recordType);
                        command.Parameters.AddWithValue("@FromDate", FromDate);
                        command.Parameters.AddWithValue("@ToDate", ToDate);
                        command.CommandTimeout = 1000;
                        SqlDataAdapter adp = new SqlDataAdapter(command);
                        adp.Fill(dt);

                        break;
                    default:

                        //SqlConnection Connection = DALConnectionManager.open();
                        //SqlCommand command = Connection.CreateCommand();
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "PersonConAdvancedSearch";
                        command.Parameters.AddWithValue("@SearchLoc", SearchLoc);
                        command.Parameters.AddWithValue("@MSCID", MSCID);
                        command.Parameters.AddWithValue("@recordType", recordType);
                        command.Parameters.AddWithValue("@FromDate", FromDate);
                        command.Parameters.AddWithValue("@ToDate", ToDate);
                        command.CommandTimeout = 1000;
                        SqlDataAdapter adp2 = new SqlDataAdapter(command);
                        adp2.Fill(dt);

                        break;
                }         

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return dt;
        }
        public bool UpdateP3PsnConDetails(string MacAddress, int? UID, long? Sno, int? MasSubCatID, string PsnContactInfo)
        {
            ///Created By   : Sqn Ldr WAKY Wickramasinghe
            ///Created Date :2023.07.11
            ///Des: SOp3/VolCo certified, update the P3HRMIS Enlistment Table
            /// 

            bool isExcuted = false;
            SqlTransaction trans = null;
            DataTable dt1 = new DataTable();
            //string createSource = "PORSys";

            try
            {
                string ModifiedUser = Convert.ToString(UID);
                DateTime ModifiedDate = DateTime.Now;
                string ConSno = Sno.ToString();
                using (var con = DALConnectionManager.openHrmis())
                {
                    con.Open();
                    trans = con.BeginTransaction();

                    using (var cmd = con.CreateCommand())
                    {
                        switch (MasSubCatID)
                        {

                            case (int)BAL.Enum.PORMasterSubCategory.MobileNo:

                                cmd.CommandText = @"UPDATE Enlishment
                                      SET Mobile = @Mobile,
                                      ModifiedUser = @ModifiedUser,
                                      ModifiedDate = @ModifiedDate                                     
                                      WHERE SNo =  @SNo ";
                                cmd.Transaction = trans;
                                cmd.Parameters.AddWithValue("@Mobile", PsnContactInfo);

                                break;
                            case (int)BAL.Enum.PORMasterSubCategory.ResidentialTeleNo:

                                cmd.CommandText = @"UPDATE Enlishment
                                      SET LandPhone = @LandPhone,
                                      ModifiedUser = @ModifiedUser,
                                      ModifiedDate = @ModifiedDate                                     
                                      WHERE SNo =  @SNo ";
                                cmd.Transaction = trans;
                                cmd.Parameters.AddWithValue("@LandPhone", PsnContactInfo);

                                break;
                            default:
                                cmd.CommandText = @"UPDATE Enlishment
                                      SET Email = @Email,
                                      ModifiedUser = @ModifiedUser,
                                      ModifiedDate = @ModifiedDate                                     
                                      WHERE SNo =  @SNo ";
                                cmd.Transaction = trans;
                                cmd.Parameters.AddWithValue("@Email", PsnContactInfo);
                                break;
                        }

                        cmd.Parameters.AddWithValue("@ModifiedUser", ModifiedUser);
                        cmd.Parameters.AddWithValue("@ModifiedDate", ModifiedDate);
                        cmd.Parameters.AddWithValue("@SNo", ConSno);
                        cmd.ExecuteNonQuery();

                        trans.Commit();
                        isExcuted = true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return isExcuted;
        }
        public bool UpdateP3PsnChildDetails(string MacAddress, int? UID, BAL_PsnContactHeader objPsnDetails)
        {
            ///Created By   : Sqn Ldr WAKY Wickramasinghe
            ///Created Date :2023.07.07
            ///Des: SoP2 and VolCo certified, update the HRMSChild Details

            bool isExcuted = false;
            SqlTransaction trans = null;
            DataTable dt1 = new DataTable();
            string ModifiedUser = Convert.ToString(UID);
            DateTime ModifiedDate = DateTime.Now;
            int rowcount = 0;
            string CreateSource = "POR System";
            string ChildId = "";
            string ExsistchildID = "";
            //string NewNumber = "";
            string field = "ChildID";
            try
            {
                using (var con = DALConnectionManager.openHrmis())
                {
                    con.Open();
                    trans = con.BeginTransaction();

                    using (var cmd = con.CreateCommand())
                    {

                        switch (objPsnDetails.SCID)
                        {
                            case (int)BAL.Enum.PorSubCategory.Live:

                                #region GetChildNextId

                                cmd.CommandText = @"SELECT MAX(CONVERT(bigint,SUBSTRING(" + field + ", CHARINDEX('/', " + field + ") + 1, LEN(" + field + ")))) AS keyInt FROM Children_Details WHERE ISNUMERIC(SUBSTRING(" + field + ", CHARINDEX('/', " + field + ") + 1, LEN(" + field + "))) = 1";
                                //cmd.CommandText = @"SELECT MAX(" + field + ") AS keyInt FROM  Children_Details GROUP BY CreatedDate ORDER BY CreatedDate DESC";

                                cmd.Transaction = trans;
                                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                                adapter.Fill(dt1);

                                if (dt1.Rows[0]["keyInt"].ToString() != "")
                                    rowcount = int.Parse(dt1.Rows[0]["keyInt"].ToString()) + 1;

                                else
                                    rowcount = 1;

                                ChildId = objPsnDetails.SNO + "/" + rowcount.ToString();
                                #endregion

                                #region Insert Child details

                                cmd.CommandText = @"INSERT INTO Children_Details (SNo,ChildID,ChildSex,ChildName,BirthCertificateNo,ChildBirthday,ChildBirthPlace,ChildDistrict,LivingStatus,PORRefNo,PORDate,
                                                    CreatedUser,CreatedDate,CreatedMachine,Status) VALUES (@SNo,@ChildID,@ChildSex,@ChildName,@BirthCertificateNo,@ChildBirthday,@ChildBirthPlace,@ChildDistrict,@LivingStatus,@PORRefNo,
                                                    @PORDate,@CreatedUser,@CreatedDate,@CreatedMachine,@Status)";

                                cmd.Transaction = trans;                                
                                cmd.Parameters.AddWithValue("@SNO", objPsnDetails.SNO);
                                cmd.Parameters.AddWithValue("@ChildID", ChildId);
                                cmd.Parameters.AddWithValue("@ChildSex", objPsnDetails.Gender);
                                cmd.Parameters.AddWithValue("@ChildName", objPsnDetails.ChildFullName);
                                cmd.Parameters.AddWithValue("@BirthCertificateNo", objPsnDetails.BirthCertificateNo);
                                cmd.Parameters.AddWithValue("@ChildBirthday", objPsnDetails.DateOfBirth);
                                cmd.Parameters.AddWithValue("@ChildBirthPlace", objPsnDetails.BirthPlace);
                                cmd.Parameters.AddWithValue("@ChildDistrict", objPsnDetails.District);
                                cmd.Parameters.AddWithValue("@LivingStatus", 1);
                                cmd.Parameters.AddWithValue("@PORRefNo", objPsnDetails.RefNo);
                                cmd.Parameters.AddWithValue("@PORDate", objPsnDetails.CreatedDate);
                                cmd.Parameters.AddWithValue("@CreatedUser", ModifiedUser);
                                cmd.Parameters.AddWithValue("@CreatedDate", ModifiedDate);
                                cmd.Parameters.AddWithValue("@CreatedMachine", MacAddress);
                                cmd.Parameters.AddWithValue("@Status", 1);                                                       

                                cmd.ExecuteNonQuery();

                                #endregion

                                break;
                            case (int)BAL.Enum.PorSubCategory.Death:

                                #region Get Live Child ChildernID

                                cmd.CommandText = @"SELECT ChildID as ChildID FROM Children_Details WHERE SNO = @SNO AND ChildName like CONCAT('%',@ChildName,'%')";
                                cmd.Transaction = trans;
                                cmd.Parameters.AddWithValue("@SNO", objPsnDetails.SNO);
                                cmd.Parameters.AddWithValue("@ChildName", objPsnDetails.ChildFullName);
                                SqlDataAdapter adapter2 = new SqlDataAdapter(cmd);
                                adapter2.Fill(dt1);

                                if (dt1.Rows[0]["ChildID"].ToString() != "")
                                    ExsistchildID = dt1.Rows[0]["ChildID"].ToString();

                                else
                                    ExsistchildID = "";

                                if (ExsistchildID != "")
                                {
                                    /// Update Live Status Child into Dead Status
                                    cmd.CommandText = @"UPDATE Children_Details
                                      SET DeathDate = @DeathDate,
                                      DeathCertificateNo = @DeathCertificateNo,
                                      LivingStatus = @LivingStatus,
                                      DeathPORRefNo = @DeathPORRefNo,
                                      DeathPORDate = @DeathPORDate,   
                                      ModifiedUser = @ModifiedUser,   
                                      ModifiedDate = @ModifiedDate,
                                      ModifiedMachine = @ModifiedMachine                             
                                      WHERE ChildID =  @ExsistchildID ";
                                    cmd.Transaction = trans;
                                    cmd.Parameters.AddWithValue("@DeathDate", objPsnDetails.DateOfDeath);
                                    cmd.Parameters.AddWithValue("@DeathCertificateNo", objPsnDetails.DeathCertificateNo);
                                    cmd.Parameters.AddWithValue("@LivingStatus", 2);
                                    cmd.Parameters.AddWithValue("@DeathPORRefNo", objPsnDetails.RefNo);
                                    cmd.Parameters.AddWithValue("@DeathPORDate", objPsnDetails.CreatedDate);
                                    cmd.Parameters.AddWithValue("@ModifiedUser", ModifiedUser);
                                    cmd.Parameters.AddWithValue("@ModifiedDate", ModifiedDate);
                                    cmd.Parameters.AddWithValue("@ModifiedMachine", MacAddress);
                                    cmd.Parameters.AddWithValue("@ExsistchildID", ExsistchildID);
                                    cmd.ExecuteNonQuery();
                                }

                                #endregion

                                break;
                        }

                        trans.Commit();
                        isExcuted = true;
                        return isExcuted;
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
                //trans.Rollback();
            }

        }

        
    }
}