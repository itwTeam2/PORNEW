//Date Picker for all Textbox
$(function () {
    $('.datepicker').datepicker();
});
//Msg Shade Out for all Textbox
$(function () {
    $("#Msg").delay(5000).fadeOut("slow");
});

//Use in OccuranceMaster Page
//Select Sub POR Category Relevant to the POR main Category
$(function () {
    //DDL for Select

    $("#PorCategoryMainId").change(function () {
        //DDL Automaticaly Bind
        $("#PorCategorySubId").empty();
        $.ajax({
            type: 'POST',
            url: '../PorOccurrenceMaster/PorCategorySub', dataType: 'json', data: { id: $("#PorCategoryMainId").val() }, success: function (PorCategorySub) {
                var items = '<option>SELECT</option>';
                $('#PorCategorySubId').html(items);
                $.each(PorCategorySub, function (i, PorCategorySub) {
                    $("#PorCategorySubId").append('<option value="' + PorCategorySub.Value + '">' +
                       PorCategorySub.Text + '</option>');
                });
            },
            error: function (ex) {
                alert('Failed to retrieve Make.' + ex);
            }
        });
        return false;
    })
});
//Get Personal Information using ServiceNo from P3HRMS Database
//Use View ServicePersonP3
$(function () {
    $("#SNo").change(function () {
        $.ajax({
            url: '../PorOccurrenceMaster/PersonalInfo',
            type: 'POST',
            dataType: 'json',
            data: { id: $("#SNo").val() },
            success: function (data) {
                $("#Info").text(data.UserName);
                ////$("#ServicePersonImg").append(data.ProfilePicture);
                $("#PersonNumber").text(data.ServiceNo);
                $("#PersonRank").text(data.Rank);
                $("#PersonName").text(data.Name);

            },
            error: function () {
                alert("in error function");
            }
        });
    });
});
/// Get UserInfo Using ServieNo from POR database
$(function () {
    $("#ServiceNo1").change(function () {
        $.ajax({
            url: '../User/UserInfo', type: 'POST', dataType: 'json', data: { id: $("#ServiceNo").val() },
            success: function (data) {
                $("#UserName").text(data.UserName);
                $("#LocationId").text(data.LocationId);
            },
            error: function () {
                alert("in error function2");
            }
        });
    });
});
//Posting Occurence
//Use in PorMovementOccurrence Page
//Select From Formation Relevant to the Establishment
$(function () {
    //DDL for Select    
    $("#FromEstablishmentId").change(function () {
        //DDL Automaticaly Bind
        $("#FromFormationId").empty();
        $.ajax({
            type: 'POST',
            url: '../DDL/FromEstablishment', dataType: 'json', data: { id: $("#FromEstablishmentId").val() }, success: function (FromEstablishment) {
                var items = '<option>SELECT</option>';
                $('#FromFormationId').html(items);
                $.each(FromEstablishment, function (i, FromEstablishment) {
                    $("#FromFormationId").append('<option value="' + FromEstablishment.Value + '">' +
                       FromEstablishment.Text + '</option>');
                });
            },
            error: function (ex) {
                alert('Failed to retrieve Make.' + ex);
            }
        });
        return false;
    })
});

//Get Allowance Category from Allowance
//select DDL select
$(function () {    
    
    $("#AllowanceId").change(function () {
        //DDL Automaticaly Bind
        $("#AllowanceCategoryID").empty();
        $.ajax({
            type: 'POST',
            url: '../DDL/AllowanceCategory', dataType: 'json', data: { id: $("#AllowanceId").val() }, success: function (AllowanceCategory) {
                var items = '<option>SELECT</option>';
                $('#AllowanceCategoryID').html(items);

                $.each(AllowanceCategory, function (i, AllowanceCategory) {
                    $("#AllowanceCategoryID").append('<option value="' + AllowanceCategory.Value + '">' +
                       AllowanceCategory.Text + '</option>');                 
                });
            },
            error: function (ex) {
                alert('Failed to retrieve Make.' + ex);
            }
        });
        return false;
    })
});

//Get Allowance type from Allowance
//select DDL select
$(function ()
{
    $("#ATID").change(function () {

        var ab = $("#ATID").val();

        if ($("#ATID").val() == 2)
        {
            $("#divEndDate").show();
            $("#EndDate").show();
        }
        else {
            $("#divEndDate").hide();
            $("#EndDate").hide();
        }

        $.ajax({
            type: 'POST',
            dataType: 'json',
            url: '../DDL/AllowanceType',
            data: { id: $("#ATID").val() },
            success: function (AllowanceType) {
                var items = '<option>SELECT</option>';
                $('#AllowanceId').html(items);
                $.each(AllowanceType, function (i, AllowanceType) {
                    $("#AllowanceId").append('<option value="' + AllowanceType.Value + '">' +
                    AllowanceType.Text + '</option>');

                });
            },
            error: function (ex) {
                alert('Failed to retrieve Make.' + ex);
            }
        });
        return false;
    })
});

//DDL for Select
//Get Allowance Payment Type from Allowance Category
 
$(function () { 

    $("#AllowanceId").change(function () {
        //DDL Automaticaly Bind
        $("#AllowancePaymentTypeId").empty();
       
        $.ajax({
            type: 'POST',
            url: '../DDL/AllowancePaymentType', dataType: 'json', data: { id: $("#AllowanceId").val() }, success: function (AllowancePaymentType) {
                
                var items = '<option>SELECT</option>';
                $('#AllowancePaymentTypeId').html(items);  
                $.each(AllowancePaymentType, function (i, AllowancePaymentType) {
                    $("#AllowancePaymentTypeId").append('<option value="' + AllowancePaymentType.Value + '">' +
                       AllowancePaymentType.Text + '</option>');
                });
            },
            error: function (ex) {
                alert();
                alert('Failed to retrieve Make.' + ex);
            }
        });
        return false;
    })
});

//DDL for Select
//Use in PorMovementOccurrence Page
//Select To Formation Relevant to the Establishment
$(function () {
        
    $("#ToEstablishmentId").change(function () {
        //DDL Automaticaly Bind
        $("#ToFormationId").empty();
        $.ajax({
            type: 'POST',
            url: '../DDL/ToEstablishment', dataType: 'json', data: { id: $("#ToEstablishmentId").val() }, success: function (ToEstablishment) {
                var items = '<option>SELECT</option>';
                $('#ToFormationId').html(items);
                $.each(ToEstablishment, function (i, ToEstablishment) {
                    $("#ToFormationId").append('<option value="' + ToEstablishment.Value + '">' +
                       ToEstablishment.Text + '</option>');
                });
            },
            error: function (ex) {
                alert('Failed to retrieve Make.' + ex);
            }
        });
        return false;
    })
});
//DDL for Select   
//Attachment Occurence
//Use in PorMovementOccurrence  Page
//Select From Formation Relevant to the Establishment
$(function () {    
    $("#FromEstablishmentId_").change(function () {

        //DDL Automaticaly Bind
        $("#FromFormationId_").empty();
        $.ajax({
            type: 'POST',
            url: '../DDL/FromEstablishment', dataType: 'json', data: { id: $("#FromEstablishmentId_").val() }, success: function (FromEstablishment) {
                var items = '<option>SELECT</option>';
                $('#FromFormationId_').html(items);
                $.each(FromEstablishment, function (i, FromEstablishment) {
                    $("#FromFormationId_").append('<option value="' + FromEstablishment.Value + '">' +
                       FromEstablishment.Text + '</option>');
                });
            },
            error: function (ex) {
                alert('Failed to retrieve Make.' + ex);
            }
        });
        return false;
    })
});
//Use in PorMovementOccurrence Page
//Select To Formation Relevant to the Establishment
$(function () {
    //DDL for Select
    $("#ToEstablishmentId_").change(function () {
        //DDL Automaticaly Bind
        $("#ToFormationId_").empty();
        $.ajax({
            type: 'POST',
            url: '../DDL/ToEstablishment', dataType: 'json', data: { id: $("#ToEstablishmentId_").val() }, success: function (ToEstablishment) {
                var items = '<option>SELECT</option>';
                $('#ToFormationId_').html(items);
                $.each(ToEstablishment, function (i, ToEstablishment) {
                    $("#ToFormationId_").append('<option value="' + ToEstablishment.Value + '">' +
                       ToEstablishment.Text + '</option>');
                });
            },
            error: function (ex) {
                alert('Failed to retrieve Make.' + ex);
            }
        });
        return false;
    })
});
//Use in Fixed Allowance Details controller(Create view)
//Select Rank and Full name of Person
$(function () {
    $("#SvcNo").change(function () {
        
        $.ajax({
            url: '../FixedAllowanceDetail/GetServicePerson', type: 'POST', dataType: 'json', data: { id: $("#SvcNo").val(), ServiceCategoryId: $("#ServiceCategoryId").val() },
            success: function (data) {               
                var fullName = data.ServiceNo + ' - ' + data.Rank + ' ' + data.Name;           
                $("#SvcNo").val(fullName);
                $("#ServiceNo_").val(data.ServiceNo);                
            },
            error: function () {
                var ServiceCategoryId = $("#ServiceCategoryId").val();

                if (ServiceCategoryId == 1) {

                    alert("Service No Not Match with Database");
                }
                else {

                    alert("Service No Not Match with Database");
                }
                
            }
        });
    });
});

//Use in Fixed Allowance Details controller(Create view)
//Select Rank and Full name of Person for Leave Control
$(function () {
    $("#SvcNo_").change(function () {
        //alert("ada");
        $.ajax({
            url: '../DDL/GetServicePerson',
            type: 'POST', dataType: 'json',
            data: { id: $("#SvcNo_").val(), ServiceCategoryId: $("#LeaveHeader__ServiceCategoryId").val() },
            success: function (data) {
                var fullName = data.ServiceNo + ' - ' + data.Rank + ' ' + data.Name;
                $("#SvcNo_").val(fullName);       
                $("#LeaveHeader__ServiceNo_").val(data.ServiceNo);

            },
            error: function () {
                var ServiceCategoryId = $("#ServiceCategoryId").val();
                if (ServiceCategoryId == 1) {                    
                }
                else {
                   // alert("Service No Not Match with Database");
                }
            }
        });
    });
});

//Auto Genarate Authority No
//EstablishmentId/AllowanceShortName/Year/Month/AutoId
$(function () {
    $("#AllowanceId").change(function () {
        $.ajax({
            url: '../FixedAllowanceDetail/ReferenceNo',
            type: 'POST', dataType: 'json',
            data: { AllowanceId: $("#AllowanceId").val() },
            success: function (data) {               
                $("#CampAuthority").val(data.CampAuthority);
            },
            error: function () {
             alert("Service No Not Match with Database");                
            }
        });
    });
});

//Auto Genarate Authority No
//EstablishmentId/AllowanceShortName/Year/Month/AutoId
//show and Hide Patial View 
//Get Annual Leave and Privilege Leave Count
$(function () {
    $("#SvcNo_").change(function () {       
        $.ajax({
            url: '../Leave/ReferenceNo',
            type: 'POST',
            dataType: 'json',
            data: { LeaveCategoryId: $("#LeaveCategoryId").val(), SvcNo_: $("#SvcNo_").val() },
            success: function (data) {
               // $("#LeaveHeader__Authority").val(data.Authority);                
                $("#AccumulatedLeave").html(data.AccumulatedLeave);               
                $("#PrivilegeLeave").html(data.PrivilegeLeave);
                $("#CasualLeave").html(data.CasualLeave);
                $("#Rengagement").html(data.Reengagement);               
                if (data.PorStatus == 100)
                {                    
                    alert("This person don't have Leave and Already discharge");
                }
                else
                {
                   
                }
            },
            error: function () {
               // alert("Service No Not Match with Database");
            }
        });
    });
});

$(function () {
    $("#LeaveCategoryId").change(function () {
        $.ajax({
            url: '../Leave/ReferenceNo',
            type: 'POST',
            dataType: 'json',
            data: { LeaveCategoryId: $("#LeaveCategoryId").val() },
            success: function (data) {
               // $("#LeaveHeader__Authority").val(data.Authority);
                if (data.LeaveCategoryId == 1) {                    
                    $("#_LeaveDetail").show();
                    $("#CasualLeaveId").removeAttr("disabled");
                    $("#LeaveLeaveId").removeAttr("disabled");
                   // $("#Re_engagementId").removeAttr("disabled");
                }
                else if (data.LeaveCategoryId == 16) {

                    $("#_LeaveDetail").show();                    
                    $("#CasualLeaveId").attr("disabled", "disabled");
                    $("#LeaveLeaveId").attr("disabled", "disabled");
                   // $("#Re_engagementId").attr("disabled", "disabled");
                }
                else {
                    $("#_LeaveDetail").hide();
                }
            },
            error: function () {
               // alert("Service No Not Match with Database");
            }
        });
    });
});


//Date Calculation by Cpl Madusanka
$(function () {

    $("#LeaveHeader__ToDate").change(function () {
        var FormDate_ = $("#LeaveHeader__FromDate").val();
        var ToDate_ = $("#LeaveHeader__ToDate").val();  
        $.ajax({
            url: '../Leave/DateCalc',
            type: 'POST',
            dataType: 'json',
            data: { FromDate: FormDate_, ToDate: ToDate_ },
            success: function (data) {
                data.TotalDays;
                //alert(data.TotalDays);                  
                $("#LeaveHeader__TotalDays").val(data.TotalDays);
                //if (data.LeaveCategoryId == 1) {
                //    $("#_LeaveDetail").show();
                //}
                //else {
                //    $("#_LeaveDetail").hide();
                //}              

            },
            error: function () {
               // alert("Service No Not Match with Database");
            }
        });
    });
});


//Select Payment Type Acording to the Service Cast
$(function () {
    //DDL for Select

    $("#LeaveCategoryId").change(function () {
        //DDL Automaticaly Bind
        $("#LeaveHeader__PaymentTypeId").empty();
  
        $.ajax({
            type: 'POST',
            url: '../DDL/GetPaymentType', dataType: 'json', data: { LeaveCategoryId: $("#LeaveCategoryId").val() }, success: function (GetPaymentType) {
                var items = '';
                $('#LeaveHeader__PaymentTypeId').html(items);
                $.each(GetPaymentType, function (i , GetPaymentType) {
                    $("#LeaveHeader__PaymentTypeId").append('<option value="' + GetPaymentType.Value + '">' +
                       GetPaymentType.Text + '</option>');
                    
                });
            },
            error: function (ex) {
                alert('Failed to retrieve Make.' + ex);
            }
        });
        return false;
    })
});