 using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POR.Enum
{
    public enum UserRole
    {
        AccountsClerk = 1,
        SNCO = 2,
        ACCOUNTSOFFICER = 3,
        OCPSOCA = 4,
        P3CLERK = 11,
        P2CLERK = 15,
        P2SNCO = 16,
        P3SNCO = 12,
        P2OIC = 17,
        P3OIC = 13,
        CMDP3 = 14,
        KOPNR = 5,
        SNCOSALARY = 6,
        WOSALARY = 7,
        ACCOUNTS01 = 8,
        CERTIFIED = 9,
        HRMSCLKP3VOL = 21,
        HRMSSNCO = 22,
        ASORSOVRP3VOL = 23,
        P1CLERK = 24,
        P1SNCO = 25,
        P1OIC = 26,
        HRMSCLKP2VOL = 28,
        HRMSP2SNCO = 29,
        ASORSOVRP2VOL = 30,
    }
    public enum RecordStatus
    {
        Insert = 1000,
        Forward = 2000,
        Reject = 3000,
        Delete = 4000,
        Cancel = 5000,
        Ceased = 6000,
    }
    public enum LivinInOutCategories
    {
        BLIN = 200,
        MLIN = 201,
        BLOut = 202,
        MLOut = 203,
        BROut = 204,
       MROut = 205,

    }
    public enum CivilStatusCategory
    {
        Marriage = 300,
        Divorce =301,
        Widow = 302,
        NullVoid = 311,
    }
    public enum ServiceType
    {
        RegOfficer = 1001,
        RegLadyOfficer = 1002,
        VolOfficer = 1003,
        VolLadyOfficer = 1004,
        RegAirmen = 1005,
        RegAirWomen = 1006,
        VolAirmen = 1007,
        VolAirWomen = 1008,
    }
    public enum PORFlowStatus
    {
        PROnly = 20,
        PRP3Combine = 21,
        P3Only = 22,
    }
    public enum GSQStatus
    {
        Allocate = 250,
        Vacant = 251
    }
    public enum NOKtatus
    {
        MarriedChange = 110,
        DivorceChange = 120,
        WidowChange = 130,
        LivingInOutChange = 140,
        ChangeNOKOnly = 150,
        GSQChange = 160,
    }
    public enum NOKSelectCategory
    {
        civilStatus = 1,
        livinInOut = 2,
        gsqAllocateVacanr= 3,
        NokChange = 4,
    }
    public enum PORMasterSubCategory
    {
        MobileNo = 60,
        ResidentialTeleNo = 61,
        EMailAddress = 62,
        DetailOfChildBirth = 63
    }

    public enum PorSubCategory
    {
        Live = 50,
        Death = 51
    }

}

