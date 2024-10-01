/// 2021.02.10 Fg Off Gamage
////load details according to service number
$(function () {
    $("#Snumber").change(function () {       
        $.ajax({
            //url: '@Url.Content("~/User/getfullName")',
            url: './getname',
            type: 'POST',
            dataType: 'json',
            data: { id: $("#Snumber").val() },
            success: function (data) {
                if (data.ServiceNo == "Service number is not valid.") {
                    alert("Service number is not valid.");
                    $("#Snumber").val('');
                    $("#ServiceNo").val('');
                }
                else
                {
                    $("#Snumber").val(data.ServiceNo);
                    $("#FullName").val(data.Name);
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
            error: function () {
                alert("error in function");
            }
        });
    });
});

$(function () {
    $("#Snumber2").change(function () {
        $.ajax({
            //url: '@Url.Content("~/User/getfullName")',
            url: './getname',
            type: 'POST',
            dataType: 'json',
            data: { id: $("#Snumber2").val() },
            success: function (data) {
                if (data.ServiceNo == "Service number is not valid.") {
                    alert("Service number is not valid.");
                    $("#Snumber2").val('');
                    $("#ServiceNo2").val('');
                }
                else {
                    $("#Snumber2").val(data.ServiceNo);
                    $("#FullName2").val(data.Name);
                    $("#Rank2").val(data.Rank);
                    $("#Trade2").val(data.Branch);


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
            error: function () {
                alert("error iiiiiiin function");
            }
        });
    });
});

//Hide the div when loading the page NOK details change or not
$(document).ready(function () {
    $("#dvYes").hide();
    $("#dvNo").hide();
    //$("#dvHRIMSNOKInfo").hide();
    $("#dvDetails").hide();
    //$("#dvPORIncludeNOKDetails").hide();
    $("#dvDetails1").hide(); 
    $("#dvdatePeriod").hide();
    $("#dvWithEffctDate").hide();
    $("#warnningDv").hide();
    $("#dvNokShowArea").hide();
    $("#dvNormalSaveArea").hide();

    $("#dvloader").hide();

    $("#Snumber").change(function () {

        $("#dvHRIMSNOKInfo").show();
        $('#Name').empty();
        $("#NOKRelationship").empty();
        $("#NOKAddress").empty();

        $.ajax({
            //url: '@Url.Content("~/User/getfullName")',
            url: './HrmisNokDetails',
            type: 'POST',
            dataType: 'json',
            data: { id: $("#Snumber").val() },
            success: function (data) {
                $("#Name").append(data.NOKName);
                $("#NOKRelationship").append(data.RelationshipName);
                $("#NOKAddress").append(data.NOKAddress);
            },
            error: function () {
                alert("error in function");
            }
        });

        $("#dvPORIncludeNOKDetails").show();
        $("#PORName").empty();
        $("#PORNOKRelationship").empty();
        $("#PORNOKAddress").empty();

        $.ajax({
            //url: '@Url.Content("~/User/getfullName")',
            url: './PORNokDetails',
            type: 'POST',
            dataType: 'json',
            data: { id: $("#Snumber").val() },
            success: function (data) {               
                $.each(data, function (key, value) {
                    $("#PORName").append(value.NOKName);
                    $("#PORNOKRelationship").append(value.NOKChangeTo);
                    $("#PORNOKAddress").append(value.NOKAddress);
                })

            },
            error: function () {
                alert("error in function");
            }
        });
    });  

});

///Load Different Div related to clicking button
$(function () {
    $("#btnYes").click(function () {
        $("#dvYes").show();
        $("#dvNo").hide();
        $("#dvHRIMSNOKInfo").hide();
        $("#dvPORIncludeNOKDetails").hide();

        
        var LivingOutStatus = $("#LSID").val();
        //Check the Living In/Out status is Married Living -out, It's Code is 203
        if (LivingOutStatus == 203)
        {           
            $.ajax({
                //url: '@Url.Content("~/User/getfullName")',
                url: './getMarriageDetails',
                type: 'POST',
                dataType: 'json',
                data: { id: $("#Snumber").val() },
                success: function (data) {
                    if (data.SpouseName == "No records to found in HRMIS and Please Contact the Cmd P3 Section") {
                        $("#warnningDv").show();
                    }
                    else {
                        
                        $("#NOKName").val(data.SpouseName);
                    }

                },
                error: function (ex) {
                    alert("in error function" + ex);
                }
            });
        }     

    });
    $("#btnNo").click(function () {
        $("#dvNo").show();
        $("#dvYes").hide();
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

///Get the confirmation of Married or Not
$(function () {
    $("#LSID").change(function () {
       
        var lsid = $("#LSID").val();
        //201 = M/L/IN , 203 = M/L/OUT , 205 = M/R/OUT
        if (lsid == 201 || lsid == 203 || lsid == 205) {
            $.ajax({
                type: 'POST',
                dataType: "json",
                url: './GetMarriedDetails',
                data: { id: $("#Snumber").val() },
                success: function (data) {
                    if (data == 0) {
                        $("#warnningDv").show();
                        
                    }
                    else {
                        /// this if statement use to seclect which button need to show. 201 and 203 status show
                        /// the  Nok change button are.
                        if (lsid == 201 || lsid == 203)
                        {
                           
                            if (lsid == 203 || lsid == 201)
                            {

                                $("#dvNokShowArea").show();                                 
                                $("#dvWithEffctDate").show();

                                $("#dvdatePeriod").hide();
                                $("#dvNormalSaveArea").hide();
                            }
                            else
                            {
                                $("#dvNokShowArea").show();                                
                                $("#dvdatePeriod").show();

                                $("#dvWithEffctDate").hide();
                                $("#dvNormalSaveArea").hide();
                            }
                            
                        }
                        else
                        {
                            $("#dvNormalSaveArea").show();
                            $("#dvdatePeriod").show();

                            $("#dvNokShowArea").hide();
                            $("#warnningDv").hide();
                            $("#dvWithEffctDate").hide();
                        }
                       
                    }
                },
                error: function (ex) {
                    alert('Failed to retrieve Make.' + ex);
                }
            });
            return false;
        }
        else {

            if (lsid == 202 || lsid == 204)
            {
                $("#dvNormalSaveArea").show();
                $("#dvdatePeriod").show();

                $("#dvNokShowArea").hide();
                $("#warnningDv").hide();                 
                $("#dvWithEffctDate").hide();
            }
            else
            {
                $("#dvNokShowArea").hide();
                $("#warnningDv").hide();
                $("#dvdatePeriod").hide();
               
                $("#dvNormalSaveArea").show();                
                $("#dvWithEffctDate").show();
            }            
        }
    })
});

//////////////////////////////////////////////////////////////////////////////////////////secondary duty
$(function () {
    $("#SDSID").change(function () {

        var lsid = $("#SDSID").val();
        //201 = M/L/IN , 203 = M/L/OUT , 205 = M/R/OUT
        if (lsid == 1 || lsid == 2 ) {
            $.ajax({
                type: 'POST',
                dataType: "json",
                url: './GetSdutyDetails',
                data: { id: $("#Snumber").val() },
                success: function (data) {
                    if (data == 0) {
                        $("#warnningDv").show();

                    }
                    else {

                        if (lsid == 2 ) {
                            $("#dvNormalSaveArea").show();
                            $("#dvdatePeriod").show();

                            $("#dvNokShowArea").hide();
                            $("#warnningDv").hide();
                            $("#dvWithEffctDate").hide();
                        }
                        else {
                            $("#dvNokShowArea").hide();
                            $("#warnningDv").hide();
                            $("#dvdatePeriod").hide();

                            $("#dvNormalSaveArea").show();
                            $("#dvWithEffctDate").show();
                        }
                    }
                },
                error: function (ex) {
                    alert('Failed to retrieve Make.' + ex);
                }
            });
            return false;
        }
        else {

            if (lsid == 1 || lsid == 2) {
                $("#dvNormalSaveArea").show();
                $("#dvdatePeriod").show();

                $("#dvNokShowArea").hide();
                $("#warnningDv").hide();
                $("#dvWithEffctDate").hide();
            }
            else {
                $("#dvNokShowArea").hide();
                $("#warnningDv").hide();
                $("#dvdatePeriod").hide();

                $("#dvNormalSaveArea").show();
                $("#dvWithEffctDate").show();
            }
        }
    })
});
////////////////////////////////////////////////////////////////////////////////////////////





///Page Loader Section Functioned
$(function () {
    $("#btnCertify").click(function () {
        $("#dvloader").show();

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

