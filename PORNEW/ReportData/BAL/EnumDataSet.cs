using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReportData.BAL.Enum
{
    public enum PORMasterSubCategory
    {
        MobileNo = 60,
        ResidentialTeleNo = 61,
        EMailAddress = 62,
        DetailOfChildBirth = 63
    }

    public enum CivilStatusCategory
    {
        Marriage = 300,
        Divorce = 301,
        Widow = 302,
        NullVoid = 311,
    }

    public enum NOKSelectCategory
    {
        civilStatus = 1,
        livinInOut = 2,
        gsqAllocateVacant = 3,
        NokChange = 4,
    }

    public enum PorSubCategory
    {
        Live = 50,
        Death = 51
    }

}