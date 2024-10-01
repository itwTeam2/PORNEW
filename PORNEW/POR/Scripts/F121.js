///Create By: Fg Off RGSD GAMAGE
///Create Date: 01/01/2022  
///Description: F121 controller related js function write in this js file.

$(document).ready(function () {
    $("#dvWitness").hide();
    $("#dvAddwitness").hide();
    $("#dvAdddoc").hide();
    $("#Sec").hide();
    $("#Punish").hide();
    $("#Punishment").hide();
    $("#dvPro").hide();

});

//load details according to service number
$(function () {
    $("#Snumber").change(function () {
        $.ajax({
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
                else {

                    $("#Snumber").val(data.ServiceNo);
                    $("#ServiceNo").val(data.SNo);
                    $("#Name").val(data.Name);
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

////load Charge Number if the person have charge Number
//$(function () {
//    $("#Snumber").change(function () {
//        $.ajax({
//            url: './getChargeNo',
//            type: 'POST',
//            dataType: 'json',
//            data: { id: $("#Snumber").val() },
//            success: function (data) {
//                if (data.ServiceNo == "Service number is not valid.") {
//                    alert("Service number is not valid."); 
//                }
//                else {
//                    $("#ChargeNo").val(data.ChargeNo);
//                }
//            },
//            error: function (ex) {
//                alert("in error function" + ex);
//            }
//        });
//    });
//});
///Create By   : Fg Off RGSD Gamage
///Create Date : 2021/01/10
///Description : load Witness details inserting dev with charge number
///

$(function () {
    $("#btnWitnesses").click(function () {
        $("#dvWitness").show();        
        $("#Punishment").hide();
        $("#dvAddwitness").show();
        $("#dvAdddoc").hide();
        $("#dvPro").hide();
        $("#Sec").show();
        
        debugger;

        var II_No = 0;
        var Def = 0;
        var Wit_No = 0;
        var x = 0;
        var dv = $("#dv02");

        var tbl_Html = '<table class="table table-responsive table-striped table-hover" style=width:60%"><tr>' +
         '<th>Service No / Name </th>' +
         '<th></th>';
        tbl_Html = tbl_Html + '</tr>';
        x++;
        II_No++;
        Def++;
        Wit_No++;

        tbl_Html = tbl_Html + '<tr>' +
        '<td >' + '<input type=text class="form-control" id="Wit_No' + x + '" value=" "/>' + ' </td>' +
        '<td>' + '<a href="#" id="btnAddWitness' + x + '" class="btn btn-success" onclick="ItemWitness(' + x + ')">Add Witnesses</a>' + '</td>' +
        '<td>' + '<a href="#" id="btnAddWitness' + x + '" class="btn btn-warning" onclick="ItemWitness2(' + x + ')">Delete Witnesses</a>' + '</td>';
        tbl_Html = tbl_Html + '</tr>'

        tbl_Html = tbl_Html + '</table>';

        dv.html(tbl_Html);


    });

});


/// 
///Create By   : Fg Off RGSD Gamage
///Create Date : 2021/01/10
///Description :This function show the added WitnessPerson details
///
function ItemWitness(x) {
    var WitnessPerson = $("#Wit_No" + x).val();
    $("#dvAddwitness").show();

    $.ajax({
        type: "GET",
        url: './GetNameFor',
        data: { id: WitnessPerson },
        dataType: "json",
        success: function (data) {
            if (data == "Please Enter the Witness Details") {
                alert("Please Enter the Witness Details..........!!!!");
            }
            else {
                var newRow = '<tr>' +
                             '<td style="padding-left:10px; height:30px">' + data + '</td>' +
                             '<td><button onclick="deleteRow(this)">Delete</button></td>' +

                             '</tr>';
                $('#tblItemList').after(newRow);
                $("#Tec_No" + x + '').val('');
            }

        },
        error: function () {
            alert("in error function");
        }
    });

    //var sItm = {};
    //sItm.ServiceNo = WitnessPerson;
}
function deleteRow(button) {
    $(button).closest('tr').remove();
}

// delete witness
///Create By   : Fg Off RGSD Gamage
///Create Date : 2021/01/10
///Description :This function show the added WitnessPerson details
///
function ItemWitness2(x) {
    var WitnessPerson = $("#Wit_No" + x).val();
    $("#dvAddwitness").show();

    $.ajax({
        type: "GET",
        url: './GetNameForDelete',
        data: { id: WitnessPerson },
        dataType: "json"
    });

    //var sItm = {};
    //sItm.ServiceNo = WitnessPerson;
}
//        



///Create By   : Fg Off RGSD Gamage
///Create Date : 2021/01/10
///Description : load Witness document details inserting dev with charge number
///

$(function () {
    $("#btnDoc").click(function () {
        $("#dvDoc").show();
        $("#dvWitness").hide();
        $("#dvAdddoc").show();        
        $("#Punishment").hide();        
        $("#dvPro").hide();
        $("#Sec").show();
        var II_No = 0;
        var Def = 0;
        var Wit_No = 0;
        var x = 0;
        var dv = $("#dv03");
        var tbl_Html_DOC = '<table class="table table-responsive table-striped table-hover" style=width:60%"><tr>' +

         '<th>Document Name</th>' +
         '<th></th>';
        tbl_Html_DOC = tbl_Html_DOC + '</tr>';

        x++;
        II_No++;
        Def++;
        Wit_No++;

        tbl_Html_DOC = tbl_Html_DOC + '<tr>' +

        '<td >' + '<input type=text class="form-control" id="Doc_No' + x + '" value=" "/>' + ' </td>' +
        '<td>' + '<a href="#" id="btnAddDoc' + x + '" class="btn btn-success" onclick="ItemPassDoc(' + x + ')">Add Documentary</a>' + '</td>' +
        '<td>' + '<a href="#" id="btnAddDoc' + x + '" class="btn btn-warning" onclick="ItemPassDoc2(' + x + ')">Delete Documentary</a>' + '</td>';

        tbl_Html_DOC = tbl_Html_DOC + '</tr>'
        tbl_Html_DOC = tbl_Html_DOC + '</table>';
        dv.html(tbl_Html_DOC);


    });
});
/// 
///Create By   : Fg Off RGSD Gamage
///Create Date : 2021/01/10
///Description :This function show the added Docuemnt  details
///

function ItemPassDoc(x) {
    var DocName = $("#Doc_No" + x).val();
    $("#dvAdddoc").show();
    $.ajax({
        type: "GET",
        url: './GetNameOfDocument',
        data: { id: DocName },
        dataType: "json",
        success: function (data) {
            if (data == "Please Enter the Document Details") {
                alert("Please Enter the Document Details...............!!!!!!!");
            }
            else {
                var newRow = '<tr>' +
                             '<td style="padding-left:10px; height:30px">' + data + '</td>' +
                             '<td><button onclick="deleteRow2(this)">Delete</button></td>' +
                             //'<td style="padding-left:10px; height:30px">' + data + '</td>' +
                             '</tr>';
                $('#tblItemListDoc').after(newRow);
                $("#Tec_No" + x + '').val('');
            }

        },
        error: function () {
            alert("in error function");
        }
    });

    //var sItm = {};
    //sItm.DocName = DocName;
}


$(function () {
    $("#btnPro").click(function () {
        $("#dvDoc").hide();
        $("#dvWitness").hide();
        $("#dvAdddoc").hide();        
        $("#Punishment").hide();
        $("#dvAdddoc").hide();
        $("#dvPro").show();
        $("#Sec").show();
        var II_No = 0;
        var Def = 0;
        var Wit_No = 0;
        var x = 0;
        var dv = $("#dv04");
        var tbl_Html_Pro = '<table class="table table-responsive table-striped table-hover" style=width:60%"><tr>' +

         '<th>Production Name</th>' +
         '<th></th>';
        tbl_Html_Pro = tbl_Html_Pro + '</tr>';

        x++;
        II_No++;
        Def++;
        Wit_No++;

        tbl_Html_Pro = tbl_Html_Pro + '<tr>' +

        '<td >' + '<input type=text class="form-control" id="Proc_No' + x + '" value=" "/>' + ' </td>' +
        '<td>' + '<a href="#" id="btnAddProc' + x + '" class="btn btn-success" onclick="ItemPassProc(' + x + ')">Add Production</a>' + '</td>' +
        '<td>' + '<a href="#" id="btnAddProc' + x + '" class="btn btn-warning" onclick="ItemPassProc2(' + x + ')">Delete Production</a>' + '</td>';

        tbl_Html_Pro = tbl_Html_Pro + '</tr>'
        tbl_Html_Pro = tbl_Html_Pro + '</table>';
        dv.html(tbl_Html_Pro);


    });
});
function ItemPassProc(x) {
    var DocName = $("#Proc_No" + x).val();
    $("#dvPro").show();
    $.ajax({
        type: "GET",
        url: './GetNameOfProc',
        data: { id: DocName },
        dataType: "json",
        success: function (data) {
            if (data == "Please Enter the Document Details") {
                alert("Please Enter the Document Details...............!!!!!!!");
            }
            else {
                var newRow = '<tr>' +
                             '<td style="padding-left:10px; height:30px">' + data + '</td>' +
                             '<td><button onclick="deleteRow2(this)">Delete</button></td>' +
                             //'<td style="padding-left:10px; height:30px">' + data + '</td>' +
                             '</tr>';
                $('#tblItemListDoc').after(newRow);
                $("#Tec_No" + x + '').val('');
            }

        },
        error: function () {
            alert("in error function");
        }
    });

    //var sItm = {};
    //sItm.DocName = DocName;
}


///Create By   : Fg Off RGSD Gamage
///Create Date : 2023/06/5
///Description :This function show the Delete Docuemnt  details
function deleteRow2(button) {
    $(button).closest('tr').remove();
}
function ItemPassProc2(x) {
    var DocName = $("#Proc_No" + x).val();
    $("#dvPro").show();
    $.ajax({
        type: "GET",
        url: './GetNameOfDocumentDelete',
        data: { id: DocName },
        dataType: "json"
    });

    //var sItm = {};
    //sItm.DocName = DocName;
}

/// 
///Create By   : Fg Off RGSD Gamage
///Create Date : 2022/02/04
///Description :This function to get all fiiled details and move the post method to save the data
///
$(function () {
    $("#btnSubmit").click(function () {
        debugger;
        $(function () {
            var url = $("#RedirectTo").val();
            var _objF252ChargeHeader = {};
            var objF252PunishmentHeader = {};
            var x = 0;
            var WitPerson = 0;
            var WitnessPerson = 0;
            var WitnessPerson = $("#Wit_No" + x + '').val();

            var DocDetail = 0;
            var DocumentDetail = 0;
            var DocumentDetail = $("#Doc_No" + x + '').val();
            var FMSID = null;
            var RSID = 1000;

            _objF252ChargeHeader.Snumber = $("#ServiceNo").val();
            _objF252ChargeHeader.ServiceNo = $("#Snumber").val();
            _objF252ChargeHeader.ChargeNo = $("#ChargeNo").val();
            _objF252ChargeHeader.ChargeNo2 = $("#ChargeNo2").val();
            _objF252ChargeHeader.ChargeDate = $("#ChargeDate").val();
            _objF252ChargeHeader.WitPerson = WitnessPerson;
            _objF252ChargeHeader.DocDetail = DocumentDetail;
            _objF252ChargeHeader.OffenceID = $("#OffenceID").val();
            _objF252ChargeHeader.OffenceWASO = $("#OffenceWASO").val();
            _objF252ChargeHeader.LocShortName = $("#LocShortName").val();
            _objF252ChargeHeader.OffenceDate = $("#OffenceDate").val();
            _objF252ChargeHeader.PunishmentID = $("#PunishmentID").val();
            _objF252ChargeHeader.PunishmentDescription = $("#PunishmentDescription").val();
            _objF252ChargeHeader.PunishDate = $("#PunishDate").val();
            _objF252ChargeHeader.Sec40 = $("#Sec40").val();
            _objF252ChargeHeader.OptCM = $("#OptCM").val();
            _objF252ChargeHeader.RecCM = $("#RecCM").val();
            _objF252ChargeHeader.Appointment = $("#Appointment").val();
            _objF252ChargeHeader.FMSID = FMSID;
            _objF252ChargeHeader.RSID = RSID;


            $.ajax({
                type: 'POST',
                dataType: "json",
                url: './Create',
                data: '{_objF252ChargeHeader: ' + JSON.stringify(_objF252ChargeHeader) + '}',
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    alert(data.Message);

                    location.href = url;
                },
                error: function () {
                    alert("Error while inserting data");
                }
            });

            return false;
        });

        return false;
    });
});




///Create By   : Fg Off RGSD Gamage
///Create Date : 2022/02/03
///Description : Show the Punishment details div 
///
$(function () {
    $("#btnPunishment").click(function () {
        $("#Punishment").show();
        //$("#dvDoc").hide();
        //$("#dvWitness").hide();
        //$("#dvAdddoc").hide();
        //$("#dvAddwitness").hide();
        //$("#Sec").hide();


    });

});

