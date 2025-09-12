function sel_text(obj, dest) {
    row = obj.parentNode.parentNode.rowIndex;
    var modelLength = obj.options[obj.selectedIndex].text;
    $("." + dest)[row - 1].value = modelLength;
    var selid = '/Common/CostFactorDetail/' + obj.value;
    $.getJSON(selid, function (data) {
        var items = [];
        $.each(data, function (key, val) {
            $(".CFMODE")[row - 1].value = val.CFMODE;
            $(".CFEXPR")[row - 1].value = val.CFEXPR;
            $(".CFTYPE")[row - 1].value = val.CFTYPE;
            $(".DORDRID")[row - 1].value = val.DORDRID;
            $(".DEDORDR")[row - 1].value = val.CFNATR;
        });
        total();
    });
}
function del_factor(obj) {
    row = obj.parentNode.parentNode.rowIndex;
    document.getElementById("CFACTOR").deleteRow(row);
    total();
    return false;

}
function total() {
    TRANNAMT = 0;
    TRANGAMT = 0;
    CF_ROW_TOTAL = 0;
    i = 0;
    TRANDGAMT = 0;
    TRANROAMT = 0;
    var total = 0; var gamt = 0.00;

    count = 0; 
    TRANSAMT = 0; TRANHAMT = 0; TRANEAMT = 0; TRANFAMT = 0; TRANPAMT = 0; TRANTCAMT = 0; TRANAAMT = 0;
    i = 0; //$("#TRANGAMT").val("0");
    // $(".TRANDID").each(function () {
    $(".STFDIDS").each(function () {
        ckb = $(this).is(':checked');

        if (ckb == true) {
            $(".boolSTFDIDS")[i].value = ckb;
            var TRANDSAMT = $(".TRANDSAMT")[i].value;
            var TRANDHAMT = $(".TRANDHAMT")[i].value;
            var TRANDFAMT = $(".TRANDFAMT")[i].value;
            var TRANDAAMT = $(".TRANDAAMT")[i].value;
            var TRANDEAMT = $(".TRANDEAMT")[i].value;
            var TRANDPAMT = $(".TRANDPAMT")[i].value;
            var TRANDIAMT = $(".TRANDIAMT")[i].value;
            var TRANDHALTAMT = $(".TRANDHALTAMT")[i].value;
            var TRANDTRNSPRTAMT = $(".TRANDTRNSPRTAMT")[i].value;
            if (isNaN(TRANDSAMT)) TRANDSAMT = "0"; if (isNaN(TRANDHAMT)) TRANDHAMT = "0"; if (isNaN(TRANDEAMT)) TRANDEAMT = "0"; if (isNaN(TRANDFAMT)) TRANDFAMT = "0"; if (isNaN(TRANDAAMT)) TRANDAAMT = "0";
            if (isNaN(TRANDPAMT)) TRANDPAMT = "0"; if (isNaN(TRANDIAMT)) TRANDIAMT = "0"; if (isNaN(TRANDHALTAMT)) TRANDHALTAMT = "0";
            if (isNaN(TRANDTRNSPRTAMT)) TRANDTRNSPRTAMT = "0";
            total = parseFloat(TRANDSAMT) + parseFloat(TRANDHAMT) + parseFloat(TRANDEAMT) + parseFloat(TRANDFAMT) + parseFloat(TRANDAAMT) + parseFloat(TRANDPAMT) + parseFloat(TRANDIAMT) + parseFloat(TRANDHALTAMT) + parseFloat(TRANDTRNSPRTAMT);

            $(".TRANDNAMT")[i].value = total.toFixed(2);
            if (isNaN(gamt)) gamt = "0";
            gamt = gamt + parseFloat($(".TRANDNAMT")[i].value);

            if (isNaN(TRANSAMT)) TRANSAMT = "0";
            TRANSAMT = TRANSAMT + eval($(".TRANDSAMT")[i].value);
            if (isNaN(TRANEAMT)) TRANEAMT = "0";
            TRANEAMT = TRANEAMT + eval($(".TRANDEAMT")[i].value);
            if (isNaN(TRANHAMT)) TRANHAMT = "0";
            TRANHAMT = TRANHAMT + eval($(".TRANDHAMT")[i].value);
            if (isNaN(TRANFAMT)) TRANFAMT = "0";
            TRANFAMT = TRANFAMT + eval($(".TRANDFAMT")[i].value);
            if (isNaN(TRANPAMT)) TRANPAMT = "0";
            TRANPAMT = TRANPAMT + eval($(".TRANDPAMT")[i].value);
            if (isNaN(TRANAAMT)) TRANAAMT = "0";
            TRANAAMT = TRANAAMT + eval($(".TRANDAAMT")[i].value);
            count++;


        }
        else {

            $(".TRANDNAMT")[i].value = 0.00; if (isNaN(gamt)) gamt = "0";
            gamt = gamt + parseFloat($(".TRANDNAMT")[i].value);

            //$("#TRANGAMT").val(gamt.toFixed(2));
            //$("#TRANNAMT").val(gamt.toFixed(2));
            //$("#TRANREFAMT").val($("#TRANNAMT").val());
            $(".boolSTFDIDS")[i].value = false;
            // count--;
        }

        i++;
    });

    $("#TRANSAMT").val(TRANSAMT.toFixed(2));
    $("#TRANEAMT").val(TRANEAMT.toFixed(2));
    $("#TRANHAMT").val(TRANHAMT.toFixed(2));
    $("#TRANFAMT").val(TRANFAMT.toFixed(2));
    $("#TRANAAMT").val(TRANAAMT.toFixed(2));
    $("#TRANPAMT").val(TRANPAMT.toFixed(2));
    $("#TRANGAMT").val(gamt.toFixed(2));
    $("#TRANNAMT").val(gamt.toFixed(2));
    $("#TRANREFAMT").val($("#TRANNAMT").val());
    TRANGAMT = $("#TRANGAMT").val();
    var round = 0.00; i = 0; NETGAMT = 0; var cfval = 0; var cfdednos = 0;
    $('.TAX').each(function () {

        //$(".DEDNOS")[i].value = count;
        temp = 0;
        temp_tax = TRANGAMT;
        if (i > 0)
            temp_tax = eval(TRANGAMT) + eval(CF_ROW_TOTAL);
        // alert("1");

        //switch ($(".DEDORDR")[i].value) {/*inclusiv/exclusiv*/
        //    case "0": temp_tax = temp_tax; break;
        //    case "1": temp_tax = 0;

        //}

        //  alert("2call");

        //if ($(".CFTYPE")[i].value == 1)
        //    temp = eval(temp_tax) * eval($(".CFEXPR")[i].value * ($(".DEDNOS")[i].value)) / 100;
        //else
        //    temp = eval($(".CFEXPR")[i].value * ($(".DEDNOS")[i].value)).toFixed(2);




        //switch ($(".DORDRID")[i].value) {
        //    case "4":

        //        if (i < 1) {
        //            temp = 0;
        //            alert("1.Correct Excise Order");

        //            break;

        //        }
        //        ex = $(".DORDRID")[i - 1].value;
        //        ex = ex.trim();

        //        if ($(".CFTYPE")[i].value != 0)
        //            temp = eval($(".CFAMOUNT")[i].value) * eval($(".CFEXPR")[i].value * ($(".DEDNOS")[i].value)) / 100;
        //        else
        //            temp = eval($(".CFEXPR")[i].value * ($(".DEDNOS")[i].value));


        //        if (ex != "3") {
        //            temp = 0;
        //            alert("2.Correct Excise Order");
        //        }

        //        break;
        //    case "5":

        //        if (i < 2) {
        //            temp = 0;
        //            alert("3.Correct Excise Order");
        //            break;

        //        }

        //        ex = $(".DORDRID")[i - 2].value;
        //        ex = ex.trim();

        //        sec = $(".DORDRID")[i - 1].value;
        //        sec = ex.trim();


        //        if ($(".CFTYPE")[i].value != 0)
        //            temp = eval($(".CFAMOUNT")[i].value) * eval($(".CFEXPR")[i].value * ($(".DEDNOS")[i].value)) / 100;
        //        else
        //            temp = eval($(".CFEXPR")[i].value * ($(".DEDNOS")[i].value));


        //        if (ex != "3" && sec != "4") {
        //            temp = 0;
        //            alert("4.Correct Excise Order");
        //        }



        //        break;
        //}


        //  $(".DEDNOS")[i].value = "0";


        if (isNaN(temp))
            temp = 0;





        switch ($(".TAX")[i].value) {
            case "5": {/*Escort*/
                if ($(".TMPCFVAL")[i].value == "0") {
                    cfdednos = $(".DEDNOS")[i].value;
                    if (ecount > 0) {

                        if (eval(gpeamt) > 0) {
                            // alert(gpeamt);
                            cfdednos = 1;
                            // $(".DEDNOS")[i].value = 1;
                            $(".DEDNOS")[i].value = cfdednos;
                            $(".CFEXPR")[i].value = eval(gpeamt);
                            temp = eval($(".CFEXPR")[i].value * ($(".DEDNOS")[i].value)).toFixed(2);
                        }
                        else {
                            // alert("els"+gpeamt);
                            cfdednos = ecount;
                            if (eval($(".CFEXPR")[i].value) > 0)
                            { $(".DEDNOS")[i].value = cfdednos; temp = eval($(".CFEXPR")[i].value * ($(".DEDNOS")[i].value)).toFixed(2); }
                            else { $(".DEDNOS")[i].value = 0; temp = 0; }
                        }
                    }
                    // else { $(".DEDNOS")[i].value = 0; temp = 0; }
                }
                else {

                    temp = eval($(".CFEXPR")[i].value * ($(".DEDNOS")[i].value)).toFixed(2);
                }
                break;
            }
            case "6": {/*weighment*/
                if ($(".TMPCFVAL")[i].value == "0") {
                    //alert(wcount);
                    if (wcount > 0) {
                        //alert(wcount);
                        $(".DEDNOS")[i].value = wcount; temp = eval($(".CFEXPR")[i].value * ($(".DEDNOS")[i].value)).toFixed(2);
                    }
                    else {
                        temp = eval($(".CFEXPR")[i].value * ($(".DEDNOS")[i].value)).toFixed(2);
                    }
                } else {

                    temp = eval($(".CFEXPR")[i].value * ($(".DEDNOS")[i].value)).toFixed(2);
                }
                break;
            }
            case "65":/*other charges*/
                if ($(".TMPCFVAL")[i].value == "0") {
                    if (scount > 0) {

                        if (eval(gpaamt) > 0) {
                            // alert(gpeamt);
                            $(".DEDNOS")[i].value = 1;
                            $(".CFEXPR")[i].value = eval(gpaamt);
                            temp = eval($(".CFEXPR")[i].value * ($(".DEDNOS")[i].value)).toFixed(2);
                        }
                        else {
                            // alert("els"+gpeamt);
                            if (eval($(".CFEXPR")[i].value) > 0)
                            { $(".DEDNOS")[i].value = count; temp = eval($(".CFEXPR")[i].value * ($(".DEDNOS")[i].value)).toFixed(2); }
                            else { $(".DEDNOS")[i].value = 0; temp = 0; }
                        }
                        //  } else { $(".DEDNOS")[i].value = 0; temp = 0; 
                    }
                }
                else {
                    temp = eval($(".CFEXPR")[i].value * ($(".DEDNOS")[i].value)).toFixed(2);
                }
                break;
            case "90":/*Scan charges*/
                if ($(".TMPCFVAL")[i].value == "0") {
                    if (scncount > 0) {

                        $(".DEDNOS")[i].value = scncount;
                        temp = eval($(".CFEXPR")[i].value * ($(".DEDNOS")[i].value)).toFixed(2);
                        //  } else { $(".DEDNOS")[i].value = 0; temp = 0; 
                    }
                }
                else {
                    temp = eval($(".CFEXPR")[i].value * ($(".DEDNOS")[i].value)).toFixed(2);
                }
                break;
            case "4":/*OTL charges*/
                //alert(scncount);
                if ($(".TMPCFVAL")[i].value == "0") {
                    if (scncount > 0) {
                        SEALCOUNT = $(".Q_NOC").val();
                        $(".DEDNOS")[i].value = SEALCOUNT;
                        temp = eval($(".CFEXPR")[i].value * ($(".DEDNOS")[i].value)).toFixed(2);
                    } else { temp = eval($(".CFEXPR")[i].value * ($(".DEDNOS")[i].value)).toFixed(2); }
                }
                else {
                    temp = eval($(".CFEXPR")[i].value * ($(".DEDNOS")[i].value)).toFixed(2);
                }
                break;
            case "2": {
                NETGAMT = eval(temp_tax); $(".TRANTCAMT").val(eval(NETGAMT).toFixed(2));
                //  alert("case2" + temp + "...." + temp_tax + "...." + CF_ROW_TOTAL);

                if ($(".CFTYPE")[i].value == 1)
                { $(".DEDNOS")[i].value = 1; temp = eval(temp_tax) * eval($(".CFEXPR")[i].value * ($(".DEDNOS")[i].value)) / 100; }
                else
                { $(".DEDNOS")[i].value = count; temp = eval($(".CFEXPR")[i].value * ($(".DEDNOS")[i].value)).toFixed(2); }


                break;
            }
            case "96": {
                //  alert("case93" + temp + "...." + temp_tax + "...." + CF_ROW_TOTAL);

                if ($(".CFTYPE")[i].value == 1)
                { $(".DEDNOS")[i].value = 1; temp = Math.round(eval(NETGAMT) * eval($(".CFEXPR")[i].value * ($(".DEDNOS")[i].value)) / 100); }
                else
                {
                    $(".DEDNOS")[i].value = count; temp = Math.round(eval($(".CFEXPR")[i].value * ($(".DEDNOS")[i].value))).toFixed(2);

                }


                break;
            }
            case "97": {
                //  alert("case93" + temp + "...." + temp_tax + "...." + CF_ROW_TOTAL);

                if ($(".CFTYPE")[i].value == 1)
                { $(".DEDNOS")[i].value = 1; temp = Math.round(eval(NETGAMT) * eval($(".CFEXPR")[i].value * ($(".DEDNOS")[i].value)) / 100); }
                else
                {
                    $(".DEDNOS")[i].value = count; temp = Math.round(eval($(".CFEXPR")[i].value * ($(".DEDNOS")[i].value))).toFixed(2);

                }


                break;
            }
            default: {
                //  alert("def" + temp + "...." + temp_tax + "...." + CF_ROW_TOTAL);
                //NETGAMT = eval(NETGAMT) + eval(CF_ROW_TOTAL);
                if ($(".TMPCFVAL")[i].value == "0") {
                    if ($(".CFTYPE")[i].value == 1)
                    { $(".DEDNOS")[i].value = 1; temp = eval(temp_tax) * eval($(".CFEXPR")[i].value * ($(".DEDNOS")[i].value)) / 100; }
                    else
                    {
                        var dednos = count;
                        //   $(".DEDNOS")[i].value = count;
                        //  dednos = $(".DEDNOS")[i].value;
                        $(".DEDNOS")[i].value = dednos;
                        temp = eval($(".CFEXPR")[i].value * ($(".DEDNOS")[i].value)).toFixed(2);
                    }

                }
                else {

                    temp = eval($(".CFEXPR")[i].value * ($(".DEDNOS")[i].value)).toFixed(2);
                }
                break;
            }
        }
        if ($(".CFMODE")[i].value == 0)
            CF_ROW_TOTAL = Math.round(eval(CF_ROW_TOTAL) + eval(temp));
        else
            CF_ROW_TOTAL = Math.round(eval(CF_ROW_TOTAL) - eval(temp));

        if (isNaN(temp)) temp == "0";
        $(".CFAMOUNT")[i].value = Math.round(eval(temp)).toFixed(2);



        i++;
    });
    round = (eval(TRANGAMT) + eval(CF_ROW_TOTAL));

    ROCHECK = Math.abs(Math.round(round, 0) - parseFloat(round)).toFixed(2);
    if (isNaN(ROCHECK))
        ROCHECK = 0;


    $('.TRANROAMT').val(ROCHECK);
    TRANROAMT = Math.round(round, 0) - parseFloat(round);

    TRANNAMT = (eval(TRANGAMT) + eval(CF_ROW_TOTAL) + eval(TRANROAMT)).toFixed(2);

    if (isNaN(TRANNAMT))
        TRANNAMT = 0;

    $("#TRANNAMT").val(TRANNAMT);
    $("#TRANREFAMT").val(TRANNAMT);
}


$(document).ready(function () {

    function sleep(milliseconds) {
        var start = new Date().getTime();
        for (var i = 0; i < 1e7; i++) {
            if ((new Date().getTime() - start) > milliseconds) {
                break;
            }
        }
    }



    $(document).on("click", "#cfact", function () {


        var $tableBody = $('#CFACTOR').find("#CF_TBODY"),
$trLast = $tableBody.find("tr:last");
        $trFirst = $tableBody.find("tr:first");

        $("#cf_dynamic").css("display:block");


        var tax_param = "";
        i = 0;


        $('.TAX').each(function () {

            tax_param = tax_param + $.trim(this.value) + ",";
            pos = $('#CFACTOR tr').eq(($('#CFACTOR tr').length - ($('#CFACTOR tr').length)) + 1);
            idx = $('#CFACTOR tr').length - 2;
            desc = pos.find(".CFDESC").val();
            tax = pos.find(".TAX").val();
            cfval = pos.find(".TMPCFVAL").val();
            pos.find('td:eq(1)').html("<input type=text value=" + cfval + " id='TMPCFVAL' class='TMPCFVAL' name='TMPCFVAL' style='display:none' ><input type=text value=" + tax + " id='TAX' class='TAX' name='TAX' style='display:none' >" + desc + "<input type=text style='border:none' readonly=readonly value='" + desc + "' name=CFDESC id='CFDESC' class='CFDESC hide'> ");

            i++;
        });



        var formData = { term: tax_param }; //Array 

        $.ajax({
            url: "/Common/CostFactor",
            type: "POST",
            data: formData,
            success: function (data, textStatus, jqXHR) {
                //data - response from server

                if (data.length != 0)
                    $trFirst.before("<tr class='item-row'><Td> <button href='#'   type='button' onclick='del_factor(this)' class='btn btn-danger dfact btn-xs'><i class=glyphicon-minus></i> </button>  </td> <td>" + data + " </td><td><input  type=text value='' name=CFAMOUNT id='CFAMOUNT' class='CFAMOUNT' readonly='readonly'> </td>  </TD></tr>");

                // totalonchange(this);
                total();


            },
            error: function (jqXHR, textStatus, errorThrown) {

            }
        });

        // total();

        return false;

    });

});

function totalonchange(obj) {

    row = obj.parentNode.parentNode.rowIndex;
    row = row - 1;
    var $tableBody = $('#CFACTOR').find("#CF_TBODY")
    $cfrow = $tableBody.find("tr:eq(" + row + ")");
    //alert(1);
    $cfrow.find("#TMPCFVAL").val("-1");
    total();
}


function totalonchange1(obj) {
    row = obj.parentNode.parentNode.rowIndex;
    row = row - 1;
    var $tableBody = $('#CFACTOR').find("#CF_TBODY")
    $cfrow = $tableBody.find("tr:eq(" + row + ")");
    //TRANNAMT = 0;
    //TRANGAMT = 0;
    CF_ROW_TOTAL = 0;
    //i = 0;
    //TRANDGAMT = 0;
    //TRANROAMT = 0;
    //var total = 0; var gamt = 0.00;

    //count = 0; ecount = 0; scount = 0; wcount = 0; scncount = 0; gpeamt = 0; gpwamt = 0; gpaamt = 0; gpscnamt = 0;
    //TRANSAMT = 0; TRANHAMT = 0; TRANEAMT = 0; TRANFAMT = 0; TRANPAMT = 0; TRANTCAMT = 0; TRANAAMT = 0;
    //i = 0; //$("#TRANGAMT").val("0");
    //$(".TRANDID").each(function () {

    //    //ckb = $(this).is(':checked');

    //    //if (ckb == true) {

    //    //    $(".boolSTFDIDS")[i].value = ckb;
    //    //   if ($(".boolSTFDIDS")[i].value == "true") { count++; }

    //    if ($(".GPETYPE")[i].value == "1") ecount++;/*Escort*/
    //    if ($(".GPSTYPE")[i].value == "1") scount++;/*additional*/
    //    if ($(".GPWTYPE")[i].value == "1") wcount++;/*weighment*/
    //    if ($(".GPSCNTYPE")[i].value == "1") scncount++;/*scanned*/
    //    gpeamt = eval(gpeamt) + eval($(".GPEAMT")[i].value); gpaamt = eval(gpaamt) + eval($(".GPAAMT")[i].value);
    //    var TRANDSAMT = $(".TRANDSAMT")[i].value;
    //    var TRANDHAMT = $(".TRANDHAMT")[i].value;
    //    var TRANDFAMT = $(".TRANDFAMT")[i].value;
    //    var TRANDAAMT = $(".TRANDAAMT")[i].value;
    //    var TRANDEAMT = $(".TRANDEAMT")[i].value;
    //    if (isNaN(TRANDSAMT)) TRANDSAMT = "0"; if (isNaN(TRANDHAMT)) TRANDHAMT = "0"; if (isNaN(TRANDEAMT)) TRANDEAMT = "0"; if (isNaN(TRANDFAMT)) TRANDFAMT = "0"; if (isNaN(TRANDAAMT)) TRANDAAMT = "0";
    //    total = parseFloat(TRANDSAMT) + parseFloat(TRANDHAMT) + parseFloat(TRANDEAMT) + parseFloat(TRANDFAMT) + parseFloat(TRANDAAMT);

    //    $(".TRANDNAMT")[i].value = total.toFixed(2);
    //    if (isNaN(gamt)) gamt = "0";
    //    gamt = gamt + parseFloat($(".TRANDNAMT")[i].value);

    //    if (isNaN(TRANSAMT)) TRANSAMT = "0";
    //    TRANSAMT = TRANSAMT + eval($(".TRANDSAMT")[i].value);
    //    if (isNaN(TRANEAMT)) TRANEAMT = "0";
    //    TRANEAMT = TRANEAMT + eval($(".TRANDEAMT")[i].value);
    //    if (isNaN(TRANHAMT)) TRANHAMT = "0";
    //    TRANHAMT = TRANHAMT + eval($(".TRANDHAMT")[i].value);
    //    if (isNaN(TRANFAMT)) TRANFAMT = "0";
    //    TRANFAMT = TRANFAMT + eval($(".TRANDFAMT")[i].value);
    //    //if (isNaN(TRANPAMT)) TRANPAMT = "0";
    //    //TRANPAMT = TRANPAMT + eval($(".TRANDPAMT")[i].value);
    //    if (isNaN(TRANAAMT)) TRANAAMT = "0";
    //    TRANAAMT = TRANAAMT + eval($(".TRANDAAMT")[i].value);
    //    count++;


    //    // }
    //    //else {

    //    //    $(".TRANDNAMT")[i].value = 0.00; if (isNaN(gamt)) gamt = "0";
    //    //    gamt = gamt + parseFloat($(".TRANDNAMT")[i].value);

    //    //    //$("#TRANGAMT").val(gamt.toFixed(2));
    //    //    //$("#TRANNAMT").val(gamt.toFixed(2));
    //    //    //$("#TRANREFAMT").val($("#TRANNAMT").val());
    //    //    $(".boolSTFDIDS")[i].value = false;
    //    //    // count--;
    //    //}

    //    i++;
    //});

    //$("#TRANSAMT").val(TRANSAMT.toFixed(2));
    //$("#TRANEAMT").val(TRANEAMT.toFixed(2));
    //$("#TRANHAMT").val(TRANHAMT.toFixed(2));
    //$("#TRANFAMT").val(TRANFAMT.toFixed(2));
    //$("#TRANAAMT").val(TRANAAMT.toFixed(2));
    //$("#TRANPAMT").val("0.00");
    //$("#TRANGAMT").val(gamt.toFixed(2));
    //$("#TRANNAMT").val(gamt.toFixed(2));
    //$("#TRANREFAMT").val($("#TRANNAMT").val());
    TRANGAMT = $("#TRANGAMT").val();
    var round = 0.00; i = 0; NETGAMT = 0;


    if ($cfrow.find(".CFTYPE").val() == 1)
        temp = eval(temp_tax) * eval($cfrow.find(".CFEXPR").val() * ($cfrow.find(".DEDNOS").val())) / 100;
    else
        temp = eval($cfrow.find(".CFEXPR").val() * ($cfrow.find(".DEDNOS").val())).toFixed(2);

    $cfrow.find(".CFAMOUNT").val(Math.round(eval(temp)).toFixed(2));
    var i = 0;
    $('.TAX').each(function () {
        temp = 0;
        temp_tax = TRANGAMT;
        if (i > 0)
            temp_tax = eval(TRANGAMT) + eval(CF_ROW_TOTAL);
        switch ($(".TAX")[i].value) {

            case "2": {

                NETGAMT = eval(temp_tax); $(".TRANTCAMT").val(eval(NETGAMT).toFixed(2));
                // alert("case2" + temp_tax);

                if ($(".CFTYPE")[i].value == 1)
                { $(".DEDNOS")[i].value = 1; temp = eval(temp_tax) * eval($(".CFEXPR")[i].value * ($(".DEDNOS")[i].value)) / 100; }
                else
                { $(".DEDNOS")[i].value = count; temp = eval($(".CFEXPR")[i].value * ($(".DEDNOS")[i].value)).toFixed(2); }

                if ($(".CFMODE")[i].value == 0)
                    CF_ROW_TOTAL = Math.round(eval(CF_ROW_TOTAL) + eval(temp));
                else
                    CF_ROW_TOTAL = Math.round(eval(CF_ROW_TOTAL) - eval(temp));

                if (isNaN(temp)) temp == "0";
                $(".CFAMOUNT")[i].value = Math.round(eval(temp)).toFixed(2);

                break;
            }
            case "96": {
                //alert("case93" +$(".CFAMOUNT")[i].value);

                if ($(".CFTYPE")[i].value == 1)
                { $(".DEDNOS")[i].value = 1; temp = Math.round(eval(NETGAMT) * eval($(".CFEXPR")[i].value * ($(".DEDNOS")[i].value)) / 100); }
                else
                {
                    $(".DEDNOS")[i].value = count; temp = Math.round(eval($(".CFEXPR")[i].value * ($(".DEDNOS")[i].value))).toFixed(2);

                }
                if ($(".CFMODE")[i].value == 0)
                    CF_ROW_TOTAL = Math.round(eval(CF_ROW_TOTAL) + eval(temp));
                else
                    CF_ROW_TOTAL = Math.round(eval(CF_ROW_TOTAL) - eval(temp));

                if (isNaN(temp)) temp == "0";
                $(".CFAMOUNT")[i].value = Math.round(eval(temp)).toFixed(2);

                break;
            }
            default: {
                //alert($(".CFAMOUNT")[i].value);
                temp = temp + eval($(".CFAMOUNT")[i].value);
                if ($(".CFMODE")[i].value == 0)
                    CF_ROW_TOTAL = Math.round(eval(CF_ROW_TOTAL) + eval(temp));
                else
                    CF_ROW_TOTAL = Math.round(eval(CF_ROW_TOTAL) - eval(temp));
                break;
            }
        }


        i++;
    })
    round = (eval(TRANGAMT) + eval(CF_ROW_TOTAL));

    ROCHECK = Math.abs(Math.round(round, 0) - parseFloat(round)).toFixed(2);
    if (isNaN(ROCHECK))
        ROCHECK = 0;


    $('.TRANROAMT').val(ROCHECK);
    TRANROAMT = Math.round(round, 0) - parseFloat(round);

    TRANNAMT = (eval(TRANGAMT) + eval(CF_ROW_TOTAL) + eval(TRANROAMT)).toFixed(2);

    if (isNaN(TRANNAMT))
        TRANNAMT = 0;

    $("#TRANNAMT").val(TRANNAMT);
    $("#TRANREFAMT").val(TRANNAMT);
}