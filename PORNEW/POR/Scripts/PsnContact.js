///Create By: Sqn ldr Wickramasinghe
///Create Date: 2023/04/28  
///Description: Persanalcontact controller related java script funcition include here

$(document).ready(function () {
    ///Created BY   : Sqn ldr Wickramasinghe
    ///Created Date : 2023/05/03
    ///Description : hide the differnt div 

    /// Drop down list in Operation Contribution
    $("#dvMobileNo").hide();
    $("#dvResidentialTelNo").hide();
    $("#dvEMail").hide();
    $("#dvChild").hide();
    $("#dvChildBirth").hide();
    $("#dvChildDeath").hide();
    $("#dvChildBirthDetailEnterForm").hide();
    $("#dvloader").hide();
});

////load details according to service number
$(function () {
    $("#ServiceNo").change(function () {

        //var ServiceNum = $("#ServiceNo").val()
        $.ajax({
            //url: '@Url.Content("~/User/getfullName")',
            url: './getname',
            type: 'POST',
            dataType: 'json',
            data: { id: $("#ServiceNo").val() },
            success: function (data) {

                if (data.ServiceNo == "Service number is not valid.") {
                    alert("Service number is not valid.");
                    $("#ServiceNo").val('');
                }
                else {

                    $("#ServiceNo").val(data.ServiceNo);
                    $("#FullName").val(data.Name);
                    $("#Rank").val(data.Rank);
                    $("#Trade").val(data.Branch);
                }
            },
            error: function (ex) {
                alert("in error function" + ex);
            }
        });
    });
});

/// Load diffent type of div related to the selection of por sub category
$(function () {
    $("#MasSubCatID").change(function () {
       
        var MasSubCatID = $("#MasSubCatID").val();       
        /// Case 60 : mean Mobile No , Case 61 : mean Residential Tele No , Case 62 : mean E-Mail Address ,  Case 63 : mean Detail Of Child Birth
       
        if (MasSubCatID == 60) {
            $("#dvMobileNo").show();

            $("#dvResidentialTelNo").hide();
            $("#dvEMail").hide();
            $("#dvChild").hide();
        }

        else if (MasSubCatID == 61) {
            $("#dvResidentialTelNo").show();

            $("#dvMobileNo").hide();
            $("#dvEMail").hide();
            $("#dvChild").hide();
        }

        else if (MasSubCatID == 62) {
            $("#dvEMail").show();

            $("#dvMobileNo").hide();
            $("#dvResidentialTelNo").hide();
            $("#dvChild").hide();
        }

        else if (MasSubCatID == 63) {
            $("#dvChild").show();

            $("#dvMobileNo").hide();
            $("#dvResidentialTelNo").hide();
            $("#dvEMail").hide();
        }       
       
    });
});

/// Load birth related details and death related text box according to the living Status of the chaildren 

$(function () {
    $("#SCID").change(function () {

        var livingStatus = $("#SCID").val();
        /// Living Status  = 50 live and 51 = Death

        if (livingStatus == 50) {
            $("#dvChildBirth").show();
            $("#dvChildDeath").hide();
        }
        else {
            $("#dvChildDeath").show();
            $("#dvChildBirth").hide();


           // load the live children name to drop down list
            $.ajax({
                //url: '@Url.Content("~/User/getfullName")',
                url: './getLiveChildrenName',
                type: 'POST',
                dataType: 'json',
                data: { id: $("#ServiceNo").val() },
                success: function (data) {
                    debugger;
                    var items = '<option>SELECT</option>';
                    $('#PCDID').html(items);
                    $.each(data, function (i, data) {
                        if (data.Text == "No Childern record to found") {
                            alert('No live Childern record to found');
                        }
                        else {
                            $("#PCDID").append('<option value="' + data.Value + '">' +
                          data.Text + '</option>');
                        }
                       
                    });
                },
                error: function (ex) {
                    alert('Failed to retrieve Make.' + ex);
                }
            });

        }

    });
});

///Page Loader Section Functioned
$(function () {
    $("#btnCertify").click(function () {
        $("#dvloader").show();

    });
});
