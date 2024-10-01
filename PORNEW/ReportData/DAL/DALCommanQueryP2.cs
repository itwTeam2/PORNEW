using ReportData.BAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ReportData.DAL
{
    public class DALCommanQueryP2 : DALBase
    {
        public int NokNextRecordId(string Location,int NOKStatus, int CreatedYear)
        {
            ///1001,1002,1003,1004 means officers service Category
            int Id = 0;
            DataTable dt = new DataTable();
            try
            {                
                using (var con = DALConnectionManager.open())
                {
                    //con.Open();
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = @"SELECT Count (NCH.RecordCount) AS RecordCount
                                            FROM NOKChangeHeader NCH
                                            WHERE NCH.Location = @Location AND NCH.NOKStatus = @NOKStatus AND YEAR(NCH.CreatedDate) = @CreatedYear AND 
                                           (NCH.ServiceTypeId = 1001 OR NCH.ServiceTypeId = 1002 OR NCH.ServiceTypeId = 1003 OR NCH.ServiceTypeId = 1004)";

                        cmd.Parameters.AddWithValue("@Location", Location);
                        cmd.Parameters.AddWithValue("@NOKStatus", NOKStatus);
                        cmd.Parameters.AddWithValue("@CreatedYear", CreatedYear);
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            Id = Convert.ToInt32(row["RecordCount"]);
                        }
                    }                  
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Id;


        }

        public int LivingInOutNextRecordId(string Location, int CreatedYear)
        {
            ///1001,1002,1003,1004 means officers service Category
            int Id = 0;
            DataTable dt = new DataTable();

            try
            {
                using (var con = DALConnectionManager.open())
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = @"SELECT Count (LIH.RecordCount) AS RecordCount
                                            FROM LivingInOutHeader LIH
                                            WHERE LIH.Location = @Location AND YEAR(LIH.CreatedDate) = @CreatedYear AND 
                                           (LIH.ServiceTypeId = 1001 OR LIH.ServiceTypeId = 1002 OR LIH.ServiceTypeId = 1003 OR LIH.ServiceTypeId = 1004)";

                        cmd.Parameters.AddWithValue("@Location", Location);
                        cmd.Parameters.AddWithValue("@CreatedYear", CreatedYear);
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            Id = Convert.ToInt32(row["RecordCount"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return Id;
        }

        public int GSQNextRecordId(string Location, int CreatedYear)
        {
            ///1001,1002,1003,1004 means officers service Category
            int Id = 0;
            DataTable dt = new DataTable();

            try
            {
                using (var con = DALConnectionManager.open())
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = @"SELECT COUNT(GH.RecordCount) AS RecordCount
                                            FROM GSQHeader GH
                                            WHERE GH.Location = = @Location AND AND YEAR(GH.CreatedDate) = @CreatedYear AND 
                                           (GH.ServiceTypeId = 1001 OR GH.ServiceTypeId = 1002 OR GH.ServiceTypeId = 1003 OR GH.ServiceTypeId = 1004)";

                        cmd.Parameters.AddWithValue("@Location", Location);
                        cmd.Parameters.AddWithValue("@CreatedYear", CreatedYear);
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            Id = Convert.ToInt32(row["RecordCount"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return Id;
        }

        public bool SQL_UpdateServicePersonnelProfileP2(string LivingStatusName, string MacAddress, int? UID, long? Sno)
        {
            ///Created By   : Flt Lt WAKY Wickramasinghe
            ///Created Date :2023.03.10
            ///Des: When account one certified the record update the P3hrms servicePersonnelProfile table  which related to Living IN Out status

            bool status = false;
            DateTime ModifiedDate = DateTime.Now;
            SqlTransaction trans = null;
            try
            {
                string ModifiedUser = Convert.ToString(UID);                

                using (var con = DALConnectionManager.openP2Hrmis())
                {
                    con.Open();
                    trans = con.BeginTransaction();
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE ServicePersonnelProfile
                                      SET LivingIn_Out = @LivingStatusName,
                                      ModifiedMachine = @MacAddress,
                                      ModifiedBy = @ModifiedUser,
                                      ModifiedDate =@ModifiedDate 
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
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw ex;
            }
            return status;
        }

        public bool HRMSdataEntering(string LivingStatusName, string MacAddress, int? UID, BALNOK_Change_Details objNOK_Change_Details, int RelationshipID,int key)
        {
            ///Created By   : Flt Lt WAKY Wickramasinghe
            ///Created Date :2023.04.06
            ///Des: All hrms related data entering and updating section done in this section / This include the P2 living In/Out 1234

            bool isExcuted = false;
            SqlTransaction trans = null;
            DataTable dt1 = new DataTable();
            long squenceOrder = 0;
            int aliveDeceased = 1;           
            int status = 1;
            string createSource = "PORSys";

            try
            {
                string ModifiedUser = Convert.ToString(UID);
                DateTime ModifiedDate = DateTime.Now;

                using (var con = DALConnectionManager.openP2Hrmis())
                {
                    con.Open();
                    trans = con.BeginTransaction();

                    using (var cmd = con.CreateCommand())
                    {
                        #region Update the ServicePersonnelProfile
                        
                        /// Update the ServicePersonnelProfile table LivingIn_Out coloum and other fields
                        cmd.CommandText = @"UPDATE ServicePersonnelProfile
                                      SET LivingIn_Out = @LivingStatusName,
                                      ModifiedMachine = @MacAddress,
                                      ModifiedBy = @ModifiedUser,
                                      ModifiedDate =@ModifiedDate 
                                      WHERE SNo= @Sno";
                        cmd.Transaction = trans;
                        cmd.Parameters.AddWithValue("@LivingStatusName", LivingStatusName);
                        cmd.Parameters.AddWithValue("@MacAddress", MacAddress);
                        cmd.Parameters.AddWithValue("@ModifiedUser", ModifiedUser);
                        cmd.Parameters.AddWithValue("@ModifiedDate", ModifiedDate);
                        cmd.Parameters.AddWithValue("@Sno", objNOK_Change_Details.SNo);
                        //cmd.CommandTimeout = 0;
                        cmd.ExecuteNonQuery();
                        
                        //int rowAffected = cmd.ExecuteNonQuery();

                        ///// Write if else condition in one line
                        //isExcuted = rowAffected != 0 ? isExcuted = true : isExcuted = false;

                        #endregion

                        #region Get the SequenceOrder of the F373_NOK_Detail

                        //// Get the Max SequenceOrder of the NOK details
                        cmd.CommandText = @"SELECT MAX (SequenceOrder) As SequenceOrder  FROM F373_NOK_Detail WHERE SvcID = @SvcID ";
                        cmd.Parameters.AddWithValue("@SvcID", objNOK_Change_Details.SNo);
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(dt1);

                        if (dt1.Rows[0]["SequenceOrder"].ToString() != "")
                            squenceOrder = long.Parse(dt1.Rows[0]["SequenceOrder"].ToString()) + 1;
                        else
                            squenceOrder = 1;


                        #endregion

                        #region Enter New NOK details to F373_NOK_Detail
                        
                        ///Enter New NOK details to F373_NOK_Detail

                        cmd.CommandText = @"INSERT INTO F373_NOK_Detail (SvcID,NOKID,NOKTypeID,NOKFullName,NOKAddress,SequenceOrder,Authority,EffectiveDate,AliveDeceased,Status,
                                            CreatedUser,CreatedDate,CreatedMachine,CreateSource) VALUES (@SvcID2,@NOKID,@NOKTypeID,@NOKFullName,@NOKAddress,@SequenceOrder,@Authority,@EffectiveDate,@AliveDeceased,@Status,@CreateUser,@CreateDate,@CreatedMachine,@CreateSource)";

                        cmd.Transaction = trans;
                        cmd.Parameters.AddWithValue("@SvcID2", objNOK_Change_Details.SNo);
                        cmd.Parameters.AddWithValue("@NOKID", objNOK_Change_Details.NOKID);
                        cmd.Parameters.AddWithValue("@NOKTypeID", objNOK_Change_Details.Relationship);
                        cmd.Parameters.AddWithValue("@NOKFullName", objNOK_Change_Details.NOKName);
                        cmd.Parameters.AddWithValue("@NOKAddress", objNOK_Change_Details.NOKAddress);
                        cmd.Parameters.AddWithValue("@SequenceOrder", squenceOrder);
                        cmd.Parameters.AddWithValue("@Authority", objNOK_Change_Details.AuthRefNo);
                        cmd.Parameters.AddWithValue("@EffectiveDate", objNOK_Change_Details.WEFDate);
                        cmd.Parameters.AddWithValue("@AliveDeceased", aliveDeceased);
                        cmd.Parameters.AddWithValue("@Status", status);                        
                        cmd.Parameters.AddWithValue("@CreateUser", objNOK_Change_Details.CreatedUser);
                        cmd.Parameters.AddWithValue("@CreateDate", objNOK_Change_Details.CreatedDate);
                        cmd.Parameters.AddWithValue("@CreatedMachine", objNOK_Change_Details.CreatedMachine);
                        cmd.Parameters.AddWithValue("@CreateSource", createSource);
                        cmd.ExecuteNonQuery();

                        #endregion

                        #region Update the PRIMARYKEY_SEQ_HANDLER Table

                        /// Update the ServicePersonnelProfile table LivingIn_Out coloum and other fields
                        cmd.CommandText = @"UPDATE PRIMARYKEY_SEQ_HANDLER
                                      SET Previous = @Previous,
                                      CurrentUsed = @CurrentUsed,
                                      NextUse = @NextUse
                                      WHERE TableName = 'F373_ContactDetail' 
                                      AND PrimaryKeyField = 'NOKID'  AND JointField = 'Nok' ";
                        cmd.Transaction = trans;
                        cmd.Parameters.AddWithValue("@Previous", key -1);
                        cmd.Parameters.AddWithValue("@CurrentUsed", key);
                        cmd.Parameters.AddWithValue("@NextUse", key + 1);
                        cmd.ExecuteNonQuery();                        

                        #endregion

                        #region Update the F373_Personal_Infor NOK related coloums 
                        // This function Update the all NOK related infrmation (ex : )

                        cmd.CommandText = @"UPDATE F373_Personal_Infor
                                      SET GSDivision = @GSDivision,
                                      GSDivision_2 = @GSDivision_2,
                                      District = @District,
                                      Province = @Province,
                                      NearestTown = @NearestTown,
                                      NearestPoliceStaion = @NearestPoliceStaion,
                                      NearestPostOffice = @NearestPostOffice,
                                      ModifiedUser = @ModifiedUser2,
                                      ModifiedDate = @ModifiedDate2
                                      WHERE SvCID =  @SvcID ";
                        cmd.Transaction = trans;
                        cmd.Parameters.AddWithValue("@GSDivision", objNOK_Change_Details.GramaseDiv);
                        cmd.Parameters.AddWithValue("@GSDivision_2", objNOK_Change_Details.GramaseDivName);
                        cmd.Parameters.AddWithValue("@District", objNOK_Change_Details.District);
                        cmd.Parameters.AddWithValue("@Province", objNOK_Change_Details.PresentprovinceId);
                        cmd.Parameters.AddWithValue("@NearestTown", objNOK_Change_Details.NearTown);
                        cmd.Parameters.AddWithValue("@NearestPoliceStaion", objNOK_Change_Details.NearPoliceSta);
                        cmd.Parameters.AddWithValue("@NearestPostOffice", objNOK_Change_Details.P2NearPostOff);
                        cmd.Parameters.AddWithValue("@ModifiedUser2", ModifiedUser);
                        cmd.Parameters.AddWithValue("@ModifiedDate2", ModifiedDate);
                        cmd.ExecuteNonQuery();

                        ///// This Section Update the F373_ContactDetail, In this Table Update the present Address related details

                        cmd.CommandText = @"UPDATE F373_ContactDetail
                                      SET CAddressLine1 = @CAddressLine1,
                                      CAddressLine3 = @PORRef,
                                      PModifiedUser = @ModifiedUser,
                                      PModifiedDate = @ModifiedDate                                     
                                      WHERE SvcIDNOKID =  @SvcID ";
                        cmd.Transaction = trans;
                        cmd.Parameters.AddWithValue("@CAddressLine1", objNOK_Change_Details.NOKAddress);
                        cmd.Parameters.AddWithValue("@PORRef", objNOK_Change_Details.PORRefNo);                       
                        cmd.ExecuteNonQuery();

                        #endregion

                        trans.Commit();
                        isExcuted = true;
                    }
                    return isExcuted;
                }
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw ex;
            }

        }

        public bool HRMSdataEntering(string MacAddress, int? UID, BALNOK_Change_Details objNOK_Change_Details, int RelationshipID, int key)
        {
            ///Created By   : Flt Lt WAKY Wickramasinghe
            ///Created Date :2023.04.06
            ///Des: All hrms related data entering and updating section done in this section/ this method consider in the GSQ hrms update 

            bool isExcuted = false;
            SqlTransaction trans = null;
            DataTable dt1 = new DataTable();
            long squenceOrder = 0;
            int aliveDeceased = 1;
            int? NOKTypeID;
            int status = 1;
            string createSource = "PORSys";

            try
            {
                string ModifiedUser = Convert.ToString(UID);
                DateTime ModifiedDate = DateTime.Now;

                using (var con = DALConnectionManager.openP2Hrmis())
                {
                    con.Open();
                    trans = con.BeginTransaction();

                    using (var cmd = con.CreateCommand())
                    {

                        #region Get the SequenceOrder of the F373_NOK_Detail

                        //// Get the Max SequenceOrder of the NOK details
                        cmd.CommandText = @"SELECT MAX (SequenceOrder) As SequenceOrder,NOKTypeID FROM F373_NOK_Detail WHERE SvcID = @SvcID GROUP BY SequenceOrder,NOKTypeID ORDER BY SequenceOrder DESC ";
                        cmd.Transaction = trans;
                        cmd.Parameters.AddWithValue("@SvcID", objNOK_Change_Details.SNo);
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(dt1);

                        if (dt1.Rows[0]["SequenceOrder"].ToString() != "")
                            squenceOrder = long.Parse(dt1.Rows[0]["SequenceOrder"].ToString()) + 1;
                        else
                            squenceOrder = 1;


                        #endregion

                        if (dt1.Rows[0]["NOKTypeID"].ToString() != "")
                        {                          

                            NOKTypeID = Convert.ToInt32(dt1.Rows[0]["NOKTypeID"]);

                            if (NOKTypeID != objNOK_Change_Details.Relationship)
                            {
                                #region Enter New NOK details to F373_NOK_Detail
                                ///Enter New NOK details to F373_NOK_Detail // this section is consider not to enter same NOK details in NOK table

                                cmd.CommandText = @"INSERT INTO F373_NOK_Detail (SvcID,NOKID,NOKTypeID,NOKFullName,NOKAddress,SequenceOrder,Authority,EffectiveDate,AliveDeceased,Status,CreatedUser,CreatedDate,
                                                 CreatedMachine,CreateSource) VALUES (@SvcID2,@NOKID,@NOKTypeID,@NOKFullName,@NOKAddress,@SequenceOrder,@Authority,@EffectiveDate,@AliveDeceased,@Status,@CreateUser,@CreateDate,@CreatedMachine,@CreateSource)";

                                //@"INSERT INTO F373_NOK_Detail (SvcID,NOKID,NOKTypeID,NOKFullName,NOKAddress,SequenceOrder,Authority,EffectiveDate,AliveDeceased,Status,CreateSource,
                                //            CreatedUser,CreatedDate,CreatedMachine) VALUES (@SvcID2,@NOKID,@NOKTypeID,@NOKFullName,@NOKAddress,@SequenceOrder,@Authority,@EffectiveDate,@AliveDeceased,@Status,@CreateSource,@CreateUser,@CreateDate,@CreatedMachine)";

                                cmd.Transaction = trans;
                                cmd.Parameters.AddWithValue("@SvcID2", objNOK_Change_Details.SNo);
                                cmd.Parameters.AddWithValue("@NOKID", objNOK_Change_Details.NOKID);
                                cmd.Parameters.AddWithValue("@NOKTypeID", objNOK_Change_Details.Relationship);
                                cmd.Parameters.AddWithValue("@NOKFullName", objNOK_Change_Details.NOKName);
                                cmd.Parameters.AddWithValue("@NOKAddress", objNOK_Change_Details.NOKAddress);
                                cmd.Parameters.AddWithValue("@SequenceOrder", squenceOrder);
                                cmd.Parameters.AddWithValue("@Authority", objNOK_Change_Details.AuthRefNo);
                                cmd.Parameters.AddWithValue("@EffectiveDate", objNOK_Change_Details.WEFDate);
                                cmd.Parameters.AddWithValue("@AliveDeceased", aliveDeceased);
                                cmd.Parameters.AddWithValue("@Status", status);
                                cmd.Parameters.AddWithValue("@CreateUser", objNOK_Change_Details.CreatedUser);
                                cmd.Parameters.AddWithValue("@CreateDate", objNOK_Change_Details.CreatedDate);
                                cmd.Parameters.AddWithValue("@CreatedMachine", objNOK_Change_Details.CreatedMachine);
                                cmd.Parameters.AddWithValue("@CreateSource", createSource);

                                cmd.ExecuteNonQuery();

                                // This function Update the all NOK related infrmation (ex : )

                                cmd.CommandText = @"UPDATE F373_Personal_Infor
                                      SET GSDivision = @GSDivision,
                                      GSDivision_2 = @GSDivision_2,
                                      District = @District,
                                      Province = @Province,
                                      NearestTown = @NearestTown,
                                      NearestPoliceStaion = @NearestPoliceStaion,
                                      NearestPostOffice = @NearestPostOffice,
                                      ModifiedUser = @ModifiedUser,
                                      ModifiedDate = @ModifiedDate
                                      WHERE SvCID =  @SvcID ";
                                cmd.Transaction = trans;
                                cmd.Parameters.AddWithValue("@GSDivision", objNOK_Change_Details.GramaseDiv);
                                cmd.Parameters.AddWithValue("@GSDivision_2", objNOK_Change_Details.GramaseDivName);
                                cmd.Parameters.AddWithValue("@District", objNOK_Change_Details.District);
                                cmd.Parameters.AddWithValue("@Province", objNOK_Change_Details.PresentprovinceId);
                                cmd.Parameters.AddWithValue("@NearestTown", objNOK_Change_Details.NearTown);
                                cmd.Parameters.AddWithValue("@NearestPoliceStaion", objNOK_Change_Details.NearPoliceSta);
                                cmd.Parameters.AddWithValue("@NearestPostOffice", objNOK_Change_Details.P2NearPostOff);
                                cmd.Parameters.AddWithValue("@ModifiedUser", ModifiedUser);
                                cmd.Parameters.AddWithValue("@ModifiedDate", ModifiedDate);
                                cmd.ExecuteNonQuery();

                                /// Update the F373_ContactDetail Present Address and POR Ref No
                                cmd.CommandText = @"UPDATE F373_ContactDetail
                                      SET CAddressLine1 = @CAddressLine1,
                                      CAddressLine3 = @PORRef,
                                      PModifiedUser = @ModifiedUser2,
                                      PModifiedDate = @ModifiedDate2                                     
                                      WHERE SvcIDNOKID =  @SvcID ";
                                cmd.Transaction = trans;
                                cmd.Parameters.AddWithValue("@CAddressLine1", objNOK_Change_Details.NOKAddress);
                                cmd.Parameters.AddWithValue("@PORRef", objNOK_Change_Details.PORRefNo);
                                cmd.Parameters.AddWithValue("@ModifiedUser2", ModifiedUser);
                                cmd.Parameters.AddWithValue("@ModifiedDate2", ModifiedDate);
                                cmd.ExecuteNonQuery();


                                /// Update the PRIMARYKEY_SEQ_HANDLER Table
                                cmd.CommandText = @"UPDATE PRIMARYKEY_SEQ_HANDLER
                                      SET Previous = @Previous,
                                      CurrentUsed = @CurrentUsed,
                                      NextUse = @NextUse
                                      WHERE TableName = 'F373_ContactDetail' 
                                      AND PrimaryKeyField = 'NOKID'  AND JointField = 'Nok' ";
                                cmd.Transaction = trans;
                                cmd.Parameters.AddWithValue("@Previous", key - 1);
                                cmd.Parameters.AddWithValue("@CurrentUsed", key);
                                cmd.Parameters.AddWithValue("@NextUse", key + 1);
                                cmd.ExecuteNonQuery();

                                #endregion
                            }

                            else
                            {
                                #region Update the F373_Personal_Infor NOK related coloums 
                                // This function Update the all NOK related infrmation (ex : )

                                cmd.CommandText = @"UPDATE F373_Personal_Infor
                                      SET GSDivision = @GSDivision3,
                                      GSDivision_2 = @GSDivision_3,
                                      District = @District3,
                                      Province = @Province3,
                                      NearestTown = @NearestTown3,
                                      NearestPoliceStaion = @NearestPoliceStaion3,
                                      NearestPostOffice = @NearestPostOffice3,
                                      ModifiedUser = @ModifiedUser3,
                                      ModifiedDate = @ModifiedDate3
                                      WHERE SvCID =  @SvcID ";

                                cmd.Transaction = trans;
                                cmd.Parameters.AddWithValue("@GSDivision3", objNOK_Change_Details.GramaseDiv);
                                cmd.Parameters.AddWithValue("@GSDivision_3", objNOK_Change_Details.GramaseDivName);
                                cmd.Parameters.AddWithValue("@District3", objNOK_Change_Details.District);
                                cmd.Parameters.AddWithValue("@Province3", objNOK_Change_Details.PresentprovinceId);
                                cmd.Parameters.AddWithValue("@NearestTown3", objNOK_Change_Details.NearTown);
                                cmd.Parameters.AddWithValue("@NearestPoliceStaion3", objNOK_Change_Details.NearPoliceSta);
                                cmd.Parameters.AddWithValue("@NearestPostOffice3", objNOK_Change_Details.P2NearPostOff);
                                cmd.Parameters.AddWithValue("@ModifiedUser3", ModifiedUser);
                                cmd.Parameters.AddWithValue("@ModifiedDate3", ModifiedDate);
                                cmd.ExecuteNonQuery();

                                /// Update the F373_ContactDetail Present Address and POR Ref No
                                cmd.CommandText = @"UPDATE F373_ContactDetail
                                      SET CAddressLine1 = @CAddressLine1,
                                      CAddressLine3 = @PORRef4,
                                      PModifiedUser = @ModifiedUser4,
                                      PModifiedDate = @ModifiedDate4                                     
                                      WHERE SvcIDNOKID =  @SvcID ";
                                cmd.Transaction = trans;
                                cmd.Parameters.AddWithValue("@CAddressLine1", objNOK_Change_Details.NOKAddress);
                                cmd.Parameters.AddWithValue("@PORRef4", objNOK_Change_Details.PORRefNo);
                                cmd.Parameters.AddWithValue("@ModifiedUser4", ModifiedUser);
                                cmd.Parameters.AddWithValue("@ModifiedDate4", ModifiedDate);
                                cmd.ExecuteNonQuery();

                                #endregion
                            }
                            

                            trans.Commit();
                            isExcuted = true;
                        }
                        
                    }
                    return isExcuted;
                }
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw ex;
            }
        }

        public int GenerateKey()
        {
            ///Created BY   : FLT LT WAKY Wickramasinghe
            ///Created Date : 2023/04/04
            /// Description : This function is to generate NOK id for insert NOk details

            DataTable dt = new DataTable();
            int keyInt = 0;
            try
            {
               
                using (var con = DALConnectionManager.openP2Hrmis())
                {
                    con.Open();

                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = @"SELECT NextUse FROM PRIMARYKEY_SEQ_HANDLER WHERE TableName='F373_ContactDetail' 
                                            AND PrimaryKeyField='NOKID' AND JointField='Nok'";
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(dt);

                    }
                }                

                if (dt.Rows[0]["NextUse"].ToString().Trim() != "")
                    keyInt = int.Parse(dt.Rows[0]["NextUse"].ToString());
                else
                    keyInt = 1;


                return keyInt;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }       

        public bool UpdateP2PsnConDetails(string MacAddress, int? UID, long? Sno,int? MasSubCatID,string PsnContactInfo)
        {
            ///Created By   : Sqn Ldr WAKY Wickramasinghe
            ///Created Date :2023.07.05
            ///Des: SoP2 and SOp3/VolCo certified, update the HRMS
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
                using (var con = DALConnectionManager.openP2Hrmis())
                {
                    con.Open();
                    trans = con.BeginTransaction();

                    using (var cmd = con.CreateCommand())
                    {
                        switch (MasSubCatID)
                        {
                            
                            case (int)BAL.Enum.PORMasterSubCategory.MobileNo:
                                
                                cmd.CommandText = @"UPDATE F373_ContactDetail
                                      SET CMobileNo1 = @CMobileNo1,
                                      PModifiedUser = @ModifiedUser,
                                      PModifiedDate = @ModifiedDate                                     
                                      WHERE SvcIDNOKID =  @SvcID ";
                                cmd.Transaction = trans;
                                cmd.Parameters.AddWithValue("@CMobileNo1", PsnContactInfo);                        

                                break;
                            case (int)BAL.Enum.PORMasterSubCategory.ResidentialTeleNo:

                                cmd.CommandText = @"UPDATE F373_ContactDetail
                                      SET CHomeNo = @CHomeNo,
                                      PModifiedUser = @ModifiedUser,
                                      PModifiedDate = @ModifiedDate                                     
                                      WHERE SvcIDNOKID =  @SvcID ";
                                cmd.Transaction = trans;
                                cmd.Parameters.AddWithValue("@CHomeNo", PsnContactInfo);                      

                                break;
                            default:
                                cmd.CommandText = @"UPDATE F373_ContactDetail
                                      SET CEmailAddress = @CEmailAddress,
                                      PModifiedUser = @ModifiedUser,
                                      PModifiedDate = @ModifiedDate                                     
                                      WHERE SvcIDNOKID =  @SvcID ";
                                cmd.Transaction = trans;
                                cmd.Parameters.AddWithValue("@CEmailAddress", PsnContactInfo);
                                break;                                
                        }

                        cmd.Parameters.AddWithValue("@ModifiedUser", ModifiedUser);
                        cmd.Parameters.AddWithValue("@ModifiedDate", ModifiedDate);
                        cmd.Parameters.AddWithValue("@SvcID", ConSno);
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

        public bool UpdateP2PsnChildDetails(string MacAddress, int? UID, BAL_PsnContactHeader objPsnDetails)
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

            try
            {
                using (var con = DALConnectionManager.openP2Hrmis())
                {
                    con.Open();
                    trans = con.BeginTransaction();

                    using (var cmd = con.CreateCommand())
                    {                      

                        switch (objPsnDetails.SCID)
                        {
                            case (int)BAL.Enum.PorSubCategory.Live:

                                #region GetChildNextId

                                cmd.CommandText = @"SELECT COUNT(ChildrenId) as NofRow from Children ";
                                cmd.Transaction = trans;
                                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                                adapter.Fill(dt1);

                                if (dt1.Rows[0]["NofRow"].ToString() != "")
                                    rowcount = int.Parse(dt1.Rows[0]["NofRow"].ToString()) + 1;

                                else
                                    rowcount = 1;

                                ChildId = objPsnDetails.ServiceNo + "-" + rowcount.ToString();
                                #endregion

                                #region Insert Child details

                                cmd.CommandText = @"INSERT INTO Children (ChildrenId,ServiceNo,MarrigeId,ChildrenName,DateofBirth,Gender,LocationID,SVCType,AutorityRef,BirthDayCertificate,SchoolAttendence,Status,
                                                    CreatedUser, CreatedDate,CreatedMachine,LiveStatus,CreateSource) VALUES (@ChildrenId,@ServiceNo,@MarrigeId,@ChildrenName,@DateofBirth,@Gender,@LocationID,@SVCType,@AutorityRef,@BirthDayCertificate,@SchoolAttendence,
                                                    @Status,@CreatedUser,@CreatedDate,@CreatedMachine,@LiveStatus,@CreateSource)";

                                cmd.Transaction = trans;
                                cmd.Parameters.AddWithValue("@ChildrenId", ChildId);
                                cmd.Parameters.AddWithValue("@ServiceNo", objPsnDetails.SNO);
                                cmd.Parameters.AddWithValue("@MarrigeId", "Nill");
                                cmd.Parameters.AddWithValue("@ChildrenName", objPsnDetails.ChildFullName);
                                cmd.Parameters.AddWithValue("@DateofBirth", objPsnDetails.DateOfBirth);
                                cmd.Parameters.AddWithValue("@Gender", objPsnDetails.Gender);
                                cmd.Parameters.AddWithValue("@LocationID", objPsnDetails.Location);
                                cmd.Parameters.AddWithValue("@SVCType", objPsnDetails.ServiceType);
                                cmd.Parameters.AddWithValue("@AutorityRef", objPsnDetails.RefNo);
                                cmd.Parameters.AddWithValue("@BirthDayCertificate", objPsnDetails.BirthCertificateNo);
                                cmd.Parameters.AddWithValue("@SchoolAttendence", 0);
                                cmd.Parameters.AddWithValue("@Status", 454);                                 
                                cmd.Parameters.AddWithValue("@CreatedUser", ModifiedUser);
                                cmd.Parameters.AddWithValue("@CreatedDate", ModifiedDate);
                                cmd.Parameters.AddWithValue("@CreatedMachine", MacAddress);                                
                                cmd.Parameters.AddWithValue("@LiveStatus", 1);
                                cmd.Parameters.AddWithValue("@CreateSource", CreateSource);

                                cmd.ExecuteNonQuery();

                                #endregion

                                break;
                            case (int)BAL.Enum.PorSubCategory.Death:

                                #region Get Live Child ChildernID

                                cmd.CommandText = @"SELECT ChildrenId as ChildId FROM Children WHERE ServiceNo = @SNO AND ChildrenName like '%"+ objPsnDetails.ChildFullName + "%'";
                                cmd.Transaction = trans;
                                cmd.Parameters.AddWithValue("@SNO", objPsnDetails.SNO);
                                SqlDataAdapter adapter2 = new SqlDataAdapter(cmd);
                                adapter2.Fill(dt1);

                                if (dt1.Rows[0]["ChildId"].ToString() != "")
                                    ExsistchildID = dt1.Rows[0]["ChildId"].ToString();

                                else
                                    ExsistchildID = "";

                                if (ExsistchildID != "")
                                {
                                    /// Update Live Status Child into Dead Status
                                    cmd.CommandText = @"UPDATE Children
                                      SET LiveStatus = @LiveStatus,
                                      AutorityRef = @AutorityRef,
                                      ModifiedUser = @ModifiedUser2,
                                      ModifiedDate = @ModifiedDate2,
                                      ModifiedMachine = @ModifiedMachine                                    
                                      WHERE ChildrenId =  @ExsistchildID ";
                                    cmd.Transaction = trans;
                                    cmd.Parameters.AddWithValue("@LiveStatus", 0);
                                    cmd.Parameters.AddWithValue("@AutorityRef", objPsnDetails.RefNo);
                                    cmd.Parameters.AddWithValue("@ModifiedUser2", ModifiedUser);
                                    cmd.Parameters.AddWithValue("@ModifiedDate2", ModifiedDate);
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