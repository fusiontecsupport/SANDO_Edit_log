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

    Strg_TaxableAmt = 0;
    Handl_TaxableAmt = 0

    var total = 0; var gamt = 0.00;
    count = 0; ecount = 0; scount = 0; wcount = 0; scncount = 0; gpeamt = 0; gpwamt = 0; gpaamt = 0; gpscnamt = 0;
    TRANSAMT = 0; TRANHAMT = 0; TRANEAMT = 0; TRANFAMT = 0; TRANPAMT = 0; TRANTCAMT = 0; TRANAAMT = 0; TRAN_COVID_DISC_AMT = 0;
    i = 0; //$("#TRANGAMT").val("0");
   // $(".TRANDID").each(function () {
    $(".STFDIDS").each(function () {
        ckb = $(this).is(':checked');

        if (ckb == true) {

            $(".boolSTFDIDS")[i].value = ckb;
        //   if ($(".boolSTFDIDS")[i].value == "true") { count++; }

        if ($(".GPETYPE")[i].value == "1") ecount++;/*Escort*/
        if ($(".GPSTYPE")[i].value == "1") scount++;/*additional*/
        if ($(".GPWTYPE")[i].value == "1") wcount++;/*weighment*/
        if ($(".GPSCNTYPE")[i].value == "1") scncount++;/*scanned*/
        gpeamt = eval(gpeamt)+eval($(".GPEAMT")[i].value); gpaamt = eval(gpaamt)+eval($(".GPAAMT")[i].value);
            var TRANDSAMT = $(".TRANDSAMT")[i].value;
            var TRANDHAMT = $(".TRANDHAMT")[i].value;
            var TRANDFAMT = $(".TRANDFAMT")[i].value;
            var TRANDAAMT = $(".TRANDAAMT")[i].value;
            var TRANDEAMT = $(".TRANDEAMT")[i].value;
            var TRAND_COVID_DISC_AMT = $(".TRAND_COVID_DISC_AMT")[i].value;
            if (isNaN(TRANDSAMT)) TRANDSAMT = "0"; if (isNaN(TRANDHAMT)) TRANDHAMT = "0"; if (isNaN(TRANDEAMT)) TRANDEAMT = "0"; if (isNaN(TRANDFAMT)) TRANDFAMT = "0"; if (isNaN(TRANDAAMT)) TRANDAAMT = "0"; if (isNaN(TRAND_COVID_DISC_AMT)) TRAND_COVID_DISC_AMT = "0";
            total = parseFloat(TRANDSAMT) + parseFloat(TRANDHAMT) + parseFloat(TRANDEAMT) + parseFloat(TRANDFAMT) + parseFloat(TRANDAAMT) - parseFloat(TRAND_COVID_DISC_AMT);

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
            //if (isNaN(TRANPAMT)) TRANPAMT = "0";
            //TRANPAMT = TRANPAMT + eval($(".TRANDPAMT")[i].value);
            if (isNaN(TRANAAMT)) TRANAAMT = "0";
            TRANAAMT = TRANAAMT + eval($(".TRANDAAMT")[i].value);
            if (isNaN(TRAN_COVID_DISC_AMT)) TRAN_COVID_DISC_AMT = "0";
            TRAN_COVID_DISC_AMT = TRAN_COVID_DISC_AMT + eval($(".TRAND_COVID_DISC_AMT")[i].value);

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

    $("#TRAN_COVID_DISC_AMT").val(TRAN_COVID_DISC_AMT.toFixed(2));
    $("#TRANSAMT").val(TRANSAMT.toFixed(2));
    $("#TRANEAMT").val(TRANEAMT.toFixed(2));
    $("#TRANHAMT").val(TRANHAMT.toFixed(2));
    $("#TRANFAMT").val(TRANFAMT.toFixed(2));
    $("#TRANAAMT").val(TRANAAMT.toFixed(2));
    $("#TRANPAMT").val("0.00");
    $("#TRANGAMT").val(gamt.toFixed(2));
    $("#TRANNAMT").val(gamt.toFixed(2));
    $("#TRANREFAMT").val($("#TRANNAMT").val());
    TRANGAMT = $("#TRANGAMT").val();

    var round = 0.00; i = 0; NETGAMT = 0; var cfval = 0; var cfdednos = 0;
    $('.TAX').each(function () {
             
        temp = 0;
        temp_tax = TRANGAMT;
        if (i > 0)
            temp_tax = eval(TRANGAMT) + eval(CF_ROW_TOTAL);

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
            case "69": {/*SSR CHARGES insted of 126 using 69*/
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
                            if (eval($(".CFEXPR")[i].value) > 0) { $(".DEDNOS")[i].value = cfdednos; temp = eval($(".CFEXPR")[i].value * ($(".DEDNOS")[i].value)).toFixed(2); }
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


    //Strg_TaxableAmt = $("#TRANSAMT").val();
    Strg_TaxableAmt = parseFloat(TRANSAMT) - parseFloat(TRAN_COVID_DISC_AMT);
    Handl_TaxableAmt = parseFloat(TRANHAMT) + parseFloat(TRANEAMT) + parseFloat(TRANFAMT) + parseFloat(TRANAAMT) + parseFloat(CF_ROW_TOTAL);

    $('.STRG_TAXABLE_AMT').val(Strg_TaxableAmt);
    $('.HANDL_TAXABLE_AMT').val(Handl_TaxableAmt);
   // alert(Handl_TaxableAmt);

    Strg_CGSTExprn = 0;// $("#STRG_CGST_EXPRN").val();
    Strg_SGSTExprn = $("#STRG_SGST_EXPRN").val();
    Strg_IGSTExprn = $("#STRG_IGST_EXPRN").val();

    Handl_CGSTExprn = 0;//$("#HANDL_CGST_EXPRN").val();
    Handl_SGSTExprn = 0;//$("#HANDL_SGST_EXPRN").val();
    Handl_IGSTExprn = 0;//$("#HANDL_IGST_EXPRN").val();

    Strg_CGSTAmt = eval(((Strg_TaxableAmt * Strg_CGSTExprn) / 100));
    Strg_CGSTAmt = Math.abs(Math.round(Strg_CGSTAmt, 0));
    
    Strg_SGSTAmt = eval(((Strg_TaxableAmt * Strg_SGSTExprn) / 100));
    Strg_SGSTAmt = Math.abs(Math.round(Strg_SGSTAmt, 0));

    Strg_IGSTAmt = eval(((Strg_TaxableAmt * Strg_IGSTExprn) / 100));
    Strg_IGSTAmt = Math.abs(Math.round(Strg_IGSTAmt, 0));

    Handl_CGSTAmt = eval(((Handl_TaxableAmt * Handl_CGSTExprn) / 100));
    Handl_CGSTAmt = Math.abs(Math.round(Handl_CGSTAmt, 0));

    Handl_SGSTAmt = eval(((Handl_TaxableAmt * Handl_SGSTExprn) / 100));
    Handl_SGSTAmt = Math.abs(Math.round(Handl_SGSTAmt, 0));

    Handl_IGSTAmt = eval(((Handl_TaxableAmt * Handl_IGSTExprn) / 100));
    Handl_IGSTAmt = Math.abs(Math.round(Handl_IGSTAmt, 0));

    Tot_CGSTAmt = parseFloat(Strg_CGSTAmt + Handl_CGSTAmt).toFixed(2);
    Tot_SGSTAmt = parseFloat(Strg_SGSTAmt + Handl_SGSTAmt).toFixed(2);
    Tot_IGSTAmt = parseFloat(Strg_IGSTAmt + Handl_IGSTAmt).toFixed(2);

    $('.STRG_CGST_AMT').val(Strg_CGSTAmt);
    $('.STRG_SGST_AMT').val(Strg_SGSTAmt);
    $('.STRG_IGST_AMT').val(Strg_IGSTAmt);

    $('.HANDL_CGST_AMT').val(Handl_CGSTAmt);
    $('.HANDL_SGST_AMT').val(Handl_SGSTAmt);
    $('.HANDL_IGST_AMT').val(Handl_IGSTAmt);

    $('.TRANCGSTAMT').val(Tot_CGSTAmt);
    $('.TRANSGSTAMT').val(Tot_SGSTAmt);
    $('.TRANIGSTAMT').val(Tot_IGSTAmt);

   // round = (eval(TRANGAMT) + eval(CF_ROW_TOTAL));

    round = parseFloat(Strg_TaxableAmt) + parseFloat(Handl_TaxableAmt) + parseFloat(Tot_CGSTAmt) + parseFloat(Tot_SGSTAmt) + parseFloat(Tot_IGSTAmt);

    ROCHECK = Math.abs(Math.round(round, 0) - parseFloat(round)).toFixed(2);
    if (isNaN(ROCHECK))
        ROCHECK = 0;


    $('.TRANROAMT').val(ROCHECK);
    TRANROAMT = Math.round(round, 0) - parseFloat(round);

    
    //TRANNAMT = (eval(TRANGAMT) + eval(CF_ROW_TOTAL) + eval(TRANROAMT)).toFixed(2);
    TRANNAMT = (parseFloat(round) + parseFloat(TRANROAMT)).toFixed(2);
 //   alert(round);
 //   alert(TRANNAMT);

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
                temp =temp+ eval($(".CFAMOUNT")[i].value);
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