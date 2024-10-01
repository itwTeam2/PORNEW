///Create By: Flt lt Wickramasinghe
///Create Date: 30/01/2022  
///Description: NOKChange controller related java script funcition include here



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
                $('#GSDivision').html(items);
                $.each(FromDistrict, function (i, FromDistrict) {
                    $("#GSDivision").append('<option value="' + FromDistrict.Value + '">' +
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
                $('#PoliceStation').html(items);
                $.each(FromPoliceStation, function (i, FromPoliceStation) {
                    $("#PoliceStation").append('<option value="' + FromPoliceStation.Value + '">' +
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
                $('#NearestTown').html(items);
                $.each(FromTown, function (i, FromTown) {
                    $("#NearestTown").append('<option value="' + FromTown.Value + '">' +
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
                $('#PostOffice').html(items);
                $.each(FromTown, function (i, FromTown) {
                    $("#PostOffice").append('<option value="' + FromTown.Value + '">' +
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
