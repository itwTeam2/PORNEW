$(document).ready(function () {
    ///Created BY   : Flt Lt Wickramasinghe
    ///Created Date : 2022/09/16
    ///Description : hide the GSQ allocate and Vacant date text Boc

    /// Drop down list in Operation Contribution
    $("#dvallocateDate").hide();
    $("#dvVacantDate").hide();
    $("#dvslafPersonName").hide(); 
    $("#dvloader").hide();
});

//// Load HRMS NOK Getails in to a Div. purpose is to know the details of the person NOK details
$("#ServiceNo").change(function () {
    $("#dvHRIMSNOKInfo").show();
   
});

////load details according to service number
$(function () {
    ///Created BY   : Flt Lt Wickramasinghe
    ///Created Date : 2022/09/16
    ///Description : load respective date include text area related to GSQ Status

    //250 == GSQ Allocation
    
    $("#StatusName").change(function () {
        var StatusName = $("#StatusName").val();
        
        if (StatusName == 250) {
            $("#dvallocateDate").show();
            $("#dvVacantDate").hide();
        }
        else {
            $("#dvVacantDate").show();
            $("#dvallocateDate").hide();
        }
    });
});

////load details according to service number
$(function () {
    $("#ServiceNo").change(function () {
        var ServiceNum = $("#ServiceNo").val();
        $('#Name').empty();
        $.ajax({
            //url: '@Url.Content("~/User/getfullName")',
            url: './getname',
            type: 'GET',
            dataType: 'json',
            data: { id: ServiceNum },
            success: function (data)
            {                
                if (data.Name == "Service number is not valid.") {
                    alert("Service number is not valid.");
                    $("#ServiceNo").val('');

                }
                else
                {                    
                    $("#ServiceNo").val(data.ServiceNo);
                    $("#FullName").val(data.Name);
                    $("#Rank").val(data.Rank);
                    $("#Trade").val(data.Branch);

                }
            },
            error: function () {
                alert("1in error function");                
            }
        });

/////////////////////////////////////////////////////////////////////
        $.ajax({
            //url: '@Url.Content("~/User/getfullName")',
            url: './HrmisNokDetails',
            type: 'POST',
            dataType: 'json',
            data: { id: $("#ServiceNo").val() },
            success: function (data) {
                $("#Name").append(data.NOKName);
                $("#NOKRelationship").append(data.RelationshipName);
                $("#NOKAddress").append(data.NOKAddress); 
                $("#NOKName").val(data.NOKName);
                $("#RelationshipName").val(data.RelationshipName);
            },
            error: function () {
                alert("error in function");
            }
        });
//////////////////////////////////////////////////////////////////////////////
        //$.ajax({
        //    url: './GetSLAFWorkingSPHusbandInfo',
        //    type: 'GET',
        //    dataType: 'json',
        //    data: { id: $("#ServiceNo").val() },
        //    success: function (data) {
               
        //        if (data.SpaouseName == "Spouse/Husband is not serving at SLAF") {

        //            $("#SpaouseName").val("Spouse/Husband is not serving at SLAF");
        //            $("#SpaouseWrkStatus").val(data.SpaouseWrkStatus);
        //        }
        //        else {
        //            $("#SpaouseName").val(data.SpaouseName);
        //            $("#SpaouseWrkStatus").val(data.SpaouseWrkStatus);
        //        }
               

               
        //    },
        //    error: function () {
        //        alert("1in error function");
        //    }
        //});
    });
});

//Select District  Relevant to the Province from 
$(function () {
    //DDL for Select
    $("#ProvinceId").change(function () {

        //DDL Automaticaly Bind
        $("#DistId").empty();
        $.ajax({
            type: 'POST',
            dataType: "json",
            url: './SelectDistrict',
            data: { id: $("#ProvinceId").val() },
            success: function (FromDistrict) {
                var items = '<option>SELECT</option>';
                $('#DistId').html(items);
                $.each(FromDistrict, function (i, FromDistrict) {
                    $("#DistId").append('<option value="' + FromDistrict.Value + '">' +
                       FromDistrict.Text + '</option>');
                    //$("#District").val(data.DESCRIPTION);
                });
            },
            error: function (ex) {
                alert('Failed to retrieve Make.' + ex);
            }
        });
        return false;
    })
});

//Select GS divition  Relevant to the Distric
$(function () {
    //DDL for Select
    $("#District").change(function () {
       
        //DDL Automaticaly Bind
        $("#GSName").empty();
        $.ajax({
            type: 'POST',
            dataType: "json",
            url: './FromDistrict',
            data: { id: $("#District").val() },
            success: function (FromDistrict) {
                var items = '<option>SELECT</option>';
                $('#GSName').html(items);
                $.each(FromDistrict, function (i, FromDistrict) {
                    $("#GSName").append('<option value="' + FromDistrict.Value + '">' +
                       FromDistrict.Text + '</option>');
                    //$("#District").val(data.DESCRIPTION);
                });
            },
            error: function (ex) {
                alert('Failed to retrieve Make.' + ex);
            }
        });
        return false;
    })
});

//Select Police Station  Relevant to the Distric
$(function () {
    //DDL for Select
    $("#District").change(function () {
        //DDL Automaticaly Bind
        $("#PoliceStation1").empty();
        $.ajax({
            type: 'POST',
            dataType: "json",
            url: './FromPoliceStation',
            //url: '/Married/FromPoliceStation',
            data: { id: $("#District").val() },
            success: function (FromPoliceStation) {
                var items = '<option>SELECT</option>';
                $('#PoliceStation1').html(items);
                $.each(FromPoliceStation, function (i, FromPoliceStation) {
                    $("#PoliceStation1").append('<option value="' + FromPoliceStation.Value + '">' +
                       FromPoliceStation.Text + '</option>');
                });
            },
            error: function (ex) {
                alert('Failed to retrieve Make.' + ex);
            }
        });
        return false;
    })
});

//Select Town Relevant to the Distric
$(function () {
    //DDL for Select
    $("#District").change(function () {
        //DDL Automaticaly Bind

        $("#Town1").empty();
        $.ajax({
            type: 'POST',
            dataType: "json",
            url: './FromTown',
            //url: '/Married/FromTown',
            data: { id: $("#District").val() },
            success: function (FromTown) {
                var items = '<option>SELECT</option>';
                $('#Town1').html(items);
                $.each(FromTown, function (i, FromTown) {
                    $("#Town1").append('<option value="' + FromTown.Value + '">' +
                       FromTown.Text + '</option>');
                });
            },
            error: function (ex) {
                alert('Failed to retrieve Make.' + ex);
            }
        });
        return false;
    })
});

//Select Post Office Relevant to the Distric
$(function () {
    //DDL for Select
    $("#District").change(function () {
        //DDL Automaticaly Bind

        $("#PostOfficeName").empty();
        $.ajax({
            type: 'POST',
            dataType: "json",
            url: './FromPostOffice',
            //url: '/Married/FromPostOffice',
            data: { id: $("#District").val() },
            success: function (FromTown) {
                var items = '<option>SELECT</option>';
                $('#PostOfficeName').html(items);
                $.each(FromTown, function (i, FromTown) {
                    $("#PostOfficeName").append('<option value="' + FromTown.Value + '">' +
                       FromTown.Text + '</option>');
                });
            },
            error: function (ex) {
                alert('Failed to retrieve Make.' + ex);
            }
        });
        return false;
    })
});

///Page Loader Section Functioned
$(function () {
    $("#btnCertify").click(function () {
        $("#dvloader").show();

    });
});

//////////// P2 Section
//Select Town Relevant to the Distric this selection done using P2 HRMS include town related table
$(function () {
    //DDL for Select
    $("#DistId").change(function () {
        //DDL Automaticaly Bind

        $("#Town1").empty();
        $.ajax({
            type: 'POST',
            dataType: "json",
            url: './FromTown',
            data: { id: $("#DistId").val() },
            success: function (FromTown) {
                var items = '<option>SELECT</option>';
                $('#Town1').html(items);
                $.each(FromTown, function (i, FromTown) {
                    $("#Town1").append('<option value="' + FromTown.Value + '">' +
                       FromTown.Text + '</option>');
                });
            },
            error: function (ex) {
                alert('Failed to retrieve Make.' + ex);
            }
        });
        return false;
    })
});

//Select Post Office Relevant to the Distric
$(function () {
    //DDL for Select
    $("#DistId").change(function () {
        //DDL Automaticaly Bind

        $("#PostOfficeName").empty();
        $.ajax({
            type: 'POST',
            dataType: "json",
            url: './FromPostOffice',
            data: { id: $("#DistId").val() },
            success: function (FromTown) {
                var items = '<option>SELECT</option>';
                $('#PostOfficeName').html(items);
                $.each(FromTown, function (i, FromTown) {
                    $("#PostOfficeName").append('<option value="' + FromTown.Value + '">' +
                       FromTown.Text + '</option>');
                });
            },
            error: function (ex) {
                alert('Failed to retrieve Make.' + ex);
            }
        });
        return false;
    })
});
