///Create By: Flt lt Wickramasinghe
///Create Date: 25/05/2021  
///Description: Marriage controller related js function write in this js file.

$(document).ready(function () {
    ///Created BY   : Flt Lt Wickramasinghe
    ///Created Date : 2021/05/25
    ///Description : hide the marriage,divorce,widow details enter text fields.

    /// Drop down list in Operation Contribution
    $("#dvMarriage").hide();
    $("#dvDivorce").hide();
    $("#dvWidow").hide();
    $("#LowerDewtailsDv").hide();
    $("#dvWarnning").hide();

    $("#dvloader").hide();
    
});

///Load respective text area according to por clerk selected sub category
$(function () {
    ///Created BY   : Flt Lt Wickramasinghe
    ///Created Date : 2021/05/25
    ///Description : Load respective text area according to por clerk selected sub category
    /// 300 = Marriage, 301 = Divorce, 302 = Widow ,311 = Null and Void
    $("#CSCID").change(function () {
        var CSCID = $("#CSCID").val();
        
        if (CSCID == 300) {
            $("#dvMarriage").show();
            $("#dvDivorce").hide();
            $("#dvWidow").hide();
            var asb = $("#Snumber").val();
           
            $.ajax({
                //url: '@Url.Content("~/User/getfullName")',
                url: './getMarriageDetails',
                type: 'POST',
                dataType: 'json',
                data: { id: $("#Snumber").val(), CivilCat: CSCID },
                success: function (data) {
                    if (data.SpouseName == "No records to found in HRMIS and Please Contact the Cmd P3 Section") {
                        $("#warnningDv").show();
                    }
                    else
                    {
                        $("#SpouseName").val(data.SpouseName);
                    }                   
                    
                },
                error: function (ex) {
                    alert("in error function" + ex);
                }
            });

        }

        else if (CSCID == 301) {
            $("#dvDivorce").show();
            $("#dvMarriage").hide();
            $("#dvWidow").hide();
            $("#SpouseName").val('');

            $.ajax({
                //url: '@Url.Content("~/User/getfullName")',
                url: './getMarriageDetails',
                type: 'POST',
                dataType: 'json',
                data: { id: $("#Snumber").val(), CivilCat: CSCID },
                success: function (data) {
                    if (data.SpouseName == "No records to found in HRMIS and Please Contact the Cmd P3 Section") {
                        $("#warnningDv").show();
                    }
                    else {
                        $("#SpouseName").val(data.SpouseName);
                    }

                },
                error: function (ex) {
                    alert("in error function" + ex);
                }
            });
        }

        else if (CSCID == 302) {
            $("#dvWidow").show();
            $("#dvDivorce").hide();
            $("#dvMarriage").hide();
            $("#SpouseName").val('');

            $.ajax({
                //url: '@Url.Content("~/User/getfullName")',
                url: './getMarriageDetails',
                type: 'POST',
                dataType: 'json',
                data: { id: $("#Snumber").val(), CivilCat: CSCID },
                success: function (data) {
                    if (data.SpouseName == "No records to found in HRMIS and Please Contact the Cmd P3 Section") {
                        $("#warnningDv").show();
                    }
                    else {
                        $("#SpouseName").val(data.SpouseName);
                    }

                },
                error: function (ex) {
                    alert("in error function" + ex);
                }
            });
        }

        else if (CSCID == 311) {

            $("#dvDivorce").show();
            $("#dvMarriage").hide();
            $("#dvWidow").hide();
            $("#SpouseName").val('');

            $.ajax({
                //url: '@Url.Content("~/User/getfullName")',
                url: './getMarriageDetails',
                type: 'POST',
                dataType: 'json',
                data: { id: $("#Snumber").val(), CivilCat: CSCID },
                success: function (data)
                {
                    if (data.SpouseName == "No records to found in HRMIS and Please Contact the Cmd P3 Section") {
                        $("#warnningDv").show();
                    }
                    else
                    {
                        $("#SpouseName").val(data.SpouseName);
                    }
                },
                error: function (ex) {
                    alert("in error function" + ex);
                }
            });
        }
        
        });
        return false;    
});

////load details according to service number
$(function () {
    $("#Snumber").change(function () {

        //var ServiceNum = $("#ServiceNo").val()
        $.ajax({
            //url: '@Url.Content("~/User/getfullName")',
            url: './getname',
            type: 'POST',
            dataType: 'json',
            data: { id: $("#Snumber").val()},
            success: function (data) {

                if (data.ServiceNo == "Service number is not valid.") {
                    alert("Service number is not valid.");
                    $("#Snumber").val('');
                    $("#ServiceNo").val('');
                }
                else {
                    
                    $("#Snumber").val(data.ServiceNo);
                    $("#ServiceNo").val(data.ServiceNo);
                    $("#Name").val(data.Name);
                    $("#Rank").val(data.Rank);
                    $("#Trade").val(data.Branch);

                    if (data.service_type == 1001 || data.service_type == 1002 || data.service_type == 1003 || data.service_type == 1004) {
                        if (data.Marriage_Status == 1) {
                            $("#Marriage_Status").val("Single");
                        }
                        else if (data.Marriage_Status == 2) {
                            $("#Marriage_Status").val("Married");

                        }
                        else if (data.Marriage_Status == 3) {
                            $("#Marriage_Status").val("Divorce");
                        }
                        else if (data.Marriage_Status == 4) {
                            $("#Marriage_Status").val("Widowed");
                        }

                    }
                    else if (data.service_type == 1005 || data.service_type == 1006 || data.service_type == 1007 || data.service_type == 1008) {
                        if (data.Marriage_Status == 0) {
                            $("#Marriage_Status").val("Single");
                        }
                        else if (data.Marriage_Status == 1) {
                            $("#Marriage_Status").val("Married");

                        }
                        else if (data.Marriage_Status == 2) {
                            $("#Marriage_Status").val("Divorce");
                        }
                        else if (data.Marriage_Status == 3) {
                            $("#Marriage_Status").val("Widowed");
                        }
                    }
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

///Get the confirmation wheather record have in HRMIS Marriage table
$(function () {
    $("#CSCID").change(function () {
        var CSCID = $("#CSCID").val();
        
        $.ajax({
            type: 'POST',
            dataType: "json",
            url: './getDetailsConfirmation',
            data: { id: $("#Snumber").val(), CivilCat: CSCID},
            success: function (data) {
               
                if (CSCID == 300) {
                    if (data == 0) {
                        $("#dvWarnning").show();
                        $("#dvMarriage").hide();
                        $("#dvDivorce").hide();
                        $("#dvWidow").hide();
                    }
                    else {
                        $("#dvMarriage").show();
                        $("#dvWarnning").hide();
                    }
                }
                else if (CSCID == 301) {
                    if (data == 0) {
                        $("#dvWarnning").show();
                        $("#dvMarriage").hide();
                        $("#dvDivorce").hide();
                        $("#dvWidow").hide();
                    }
                    else {
                        $("#dvDivorce").show();
                        $("#dvWarnning").hide();
                    }
                }
                else {
                    if (data == 0) {
                        $("#dvWarnning").show();
                        $("#dvMarriage").hide();
                        $("#dvDivorce").hide();
                        $("#dvWidow").hide();
                    }
                    else {
                        $("#dvWidow").show();
                        $("#dvWarnning").hide();
                    }
                }       

                
                
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

