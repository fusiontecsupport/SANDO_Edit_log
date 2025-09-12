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

    Trand_TaxableAmt = 0;
    Ins_Trand_TaxableAmt = 0;
    TOTCGSTAMT = 0;
    TOTSGSTAMT = 0;
    TOTIGSTAMT = 0;
    var total = 0; var gamt = 0.00; 

    count = 0; TRANSAMT = 0; TRANHAMT = 0; TRANIAMT = 0; TRANFAMT = 0; TRANPAMT = 0; TRANTCAMT = 0; TRANAAMT = 0; 
    i = 0; //$("#TRANGAMT").val("0");
    $(".TRANDIDS").each(function () {
       
        ckb = $(this).is(':checked');
     
        if (ckb == true) {
            //alert(ckb);
            $(".boolTRANDIDS")[i].value = ckb;
            if ($(".boolTRANDIDS")[i].value == "true") { count++; }
            var TRANDGAMT = $(".TRANDGAMT")[i].value;
            var TRANDSAMT = $(".TRANDSAMT")[i].value;            
            var TRANDIAMT = $(".TRANDIAMT")[i].value;
            var TRANDCGSTAMT = $(".TRAND_CGST_AMT")[i].value;
            var TRANDSGSTAMT = $(".TRAND_SGST_AMT")[i].value;
            var TRANDIGSTAMT = $(".TRAND_IGST_AMT")[i].value;
            //alert(TRANDCGSTAMT);
            //alert(TRANDSGSTAMT);
            //alert(TRANDICGSTAMT);
            if (isNaN(TRANDSAMT)) TRANDSAMT = "0";
            if (isNaN(TRANDGAMT)) TRANDGAMT = "0";                        
            if (isNaN(TRANDIAMT)) TRANDIAMT = "0"; 

            if (isNaN(TRANDCGSTAMT)) TRANDCGSTAMT = "0";
            if (isNaN(TRANDSGSTAMT)) TRANDSGSTAMT = "0";
            if (isNaN(TRANDIGSTAMT)) TRANDIGSTAMT = "0"; 
            total = parseFloat(TRANDGAMT) + parseFloat(TRANDSAMT) + parseFloat(TRANDIAMT);
            total = total + parseFloat(TRANDCGSTAMT) + parseFloat(TRANDSGSTAMT) + parseFloat(TRANDIGSTAMT);
            TOTCGSTAMT = TOTCGSTAMT + parseFloat(TRANDCGSTAMT);
            TOTSGSTAMT = TOTSGSTAMT + parseFloat(TRANDSGSTAMT); 
            TOTIGSTAMT = TOTIGSTAMT + parseFloat(TRANDIGSTAMT);
            //alert(total);
            $(".TRANDNAMT")[i].value = total.toFixed(2);
            if (isNaN(gamt)) gamt = "0";
            gamt = gamt + parseFloat($(".TRANDGAMT")[i].value);

            
            if (isNaN(TRANGAMT)) TRANGAMT = "0";
            TRANGAMT = TRANGAMT + eval($(".TRANDGAMT")[i].value);
            if (isNaN(TRANSAMT)) TRANSAMT = "0";
            TRANSAMT = TRANSAMT + eval($(".TRANDSAMT")[i].value);
            if (isNaN(TRANIAMT)) TRANIAMT = "0";
            TRANIAMT = TRANIAMT + eval($(".TRANDIAMT")[i].value);
            

        }
        else {
           
            $(".TRAND_CGST_AMT")[i].value = 0.00;
            $(".TRAND_SGST_AMT")[i].value = 0.00;
            $(".TRAND_IGST_AMT")[i].value = 0.00;
            $(".TRAND_CGST_EXPRN")[i].value = 0.00;
            $(".TRAND_SGST_EXPRN")[i].value = 0.00;
            $(".TRAND_IGST_EXPRN")[i].value = 0.00;            
            $(".TRANDNAMT")[i].value = 0.00; if (isNaN(gamt)) gamt = "0";
            gamt = gamt + parseFloat($(".TRANDGAMT")[i].value);
         
            //$("#TRANGAMT").val(gamt.toFixed(2));
            //$("#TRANNAMT").val(gamt.toFixed(2));
            //$("#TRANREFAMT").val($("#TRANNAMT").val());
            $(".boolTRANDIDS")[i].value = false;
           // count--;
        }

        i++;
    });

    
    
    $("#TRANSAMT").val(TRANSAMT.toFixed(2));
    $("#TRANIAMT").val(TRANIAMT.toFixed(2));    

    $("#TRANGAMT").val(gamt.toFixed(2));
    $("#TRANNAMT").val(total.toFixed(2));
    $("#TRANREFAMT").val($("#TRANNAMT").val());
    TRANGAMT = $("#TRANGAMT").val();
    var round = 0.00; i = 0; NETGAMT = 0;
    $('.TAX').each(function () {
       
     //   $(".DEDNOS")[i].value = count;
        temp = 0;
        temp_tax = TRANGAMT;
        if (i > 0)
            temp_tax = eval(TRANGAMT) + eval(CF_ROW_TOTAL);

        if (isNaN(temp))
            temp = 0;

        switch ($(".TAX")[i].value) {
            case "2": {
                NETGAMT = eval(temp_tax);
                $(".TRANTCAMT").val(eval(NETGAMT).toFixed(2));
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
                var dednos = $(".DEDNOS")[i].value;
                if ($(".TMPCFVAL")[i].value == "0") {
                    if ($(".CFTYPE")[i].value == 1) {
                        //  $(".DEDNOS")[i].value = 1;
                        temp = eval(temp_tax) * eval($(".CFEXPR")[i].value * (dednos)) / 100;
                    }
                    else {// $(".DEDNOS")[i].value = count;
                        temp = eval($(".CFEXPR")[i].value * (dednos)).toFixed(2);
                    }

                }
                else {

                    temp = eval($(".CFEXPR")[i].value * ($(".DEDNOS")[i].value)).toFixed(2);
                }
                //var dednos = $(".DEDNOS")[i].value;
                //if ($(".CFTYPE")[i].value == 1)
                //{
                //  //  $(".DEDNOS")[i].value = 1;
                //    temp = eval(temp_tax) * eval($(".CFEXPR")[i].value * (dednos)) / 100;
                //}
                //else
                //{// $(".DEDNOS")[i].value = count;
                //    temp = eval($(".CFEXPR")[i].value * (dednos)).toFixed(2); }


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

    //Trand_TaxableAmt = $("#TRANSAMT").val();
    Trand_TaxableAmt = parseFloat(TRANGAMT);
    Ins_Trand_TaxableAmt = parseFloat(TRANIAMT)+ parseFloat(CF_ROW_TOTAL);

    //$('.TRAND_TAXABLE_AMT').val(Trand_TaxableAmt);
    //$('.INS_TRAND_TAXABLE_AMT').val(Ins_Trand_TaxableAmt);
   // alert(Ins_Trand_TaxableAmt);

    //Trand_CGSTExprn = $(".TRAND_CGST_EXPRN").val();
    //Trand_SGSTExprn = $(".TRAND_SGST_EXPRN").val();
    //Trand_IGSTExprn = $(".TRAND_IGST_EXPRN").val();

    //Ins_Trand_CGSTExprn = $(".INS_TRAND_CGST_EXPRN").val();
    //alert(Ins_Trand_CGSTExprn);
    //Ins_Trand_SGSTExprn = $(".INS_TRAND_SGST_EXPRN").val();
    //alert(Ins_Trand_SGSTExprn);
    //Ins_Trand_IGSTExprn = $(".INS_TRAND_IGST_EXPRN").val();

   // Trand_CGSTAmt = eval(((Trand_TaxableAmt * Trand_CGSTExprn) / 100));
    //Trand_CGSTAmt = Math.abs(Math.round(Trand_CGSTAmt, 0));
    
    //Trand_SGSTAmt = eval(((Trand_TaxableAmt * Trand_SGSTExprn) / 100));
    //Trand_SGSTAmt = Math.abs(Math.round(Trand_SGSTAmt, 0));

    //Trand_IGSTAmt = eval(((Trand_TaxableAmt * Trand_IGSTExprn) / 100));
    //Trand_IGSTAmt = Math.abs(Math.round(Trand_IGSTAmt, 0));

    //Ins_Trand_CGSTAmt = eval(((Ins_Trand_TaxableAmt * Ins_Trand_CGSTExprn) / 100));
    //Ins_Trand_CGSTAmt = Math.abs(Math.round(Ins_Trand_CGSTAmt, 0));

    //Ins_Trand_SGSTAmt = eval(((Ins_Trand_TaxableAmt * Ins_Trand_SGSTExprn) / 100));
    //Ins_Trand_SGSTAmt = Math.abs(Math.round(Ins_Trand_SGSTAmt, 0));

    //Ins_Trand_IGSTAmt = eval(((Ins_Trand_TaxableAmt * Ins_Trand_IGSTExprn) / 100));
    //Ins_Trand_IGSTAmt = Math.abs(Math.round(Ins_Trand_IGSTAmt, 0));

    //Tot_CGSTAmt = parseFloat(Trand_CGSTAmt + Ins_Trand_CGSTAmt).toFixed(2);
    //Tot_SGSTAmt = parseFloat(Trand_SGSTAmt + Ins_Trand_SGSTAmt).toFixed(2);
    //Tot_IGSTAmt = parseFloat(Trand_IGSTAmt + Ins_Trand_IGSTAmt).toFixed(2);

    //$('.TRAND_CGST_AMT').val(Trand_CGSTAmt);
    //$('.TRAND_SGST_AMT').val(Trand_SGSTAmt);
    //$('.TRAND_IGST_AMT').val(Trand_IGSTAmt);

    //$('.INS_TRAND_CGST_AMT').val(Ins_Trand_CGSTAmt);
    //$('.INS_TRAND_SGST_AMT').val(Ins_Trand_SGSTAmt);
    //$('.INS_TRAND_IGST_AMT').val(Ins_Trand_IGSTAmt);

    $('.TRAN_CGST_AMT').val(TOTCGSTAMT.toFixed(0));
    $('.TRAN_SGST_AMT').val(TOTSGSTAMT.toFixed(0));
    $('.TRAN_IGST_AMT').val(TOTIGSTAMT.toFixed(0));

    // round = (eval(TRANGAMT) + eval(CF_ROW_TOTAL));
    round = parseFloat(Trand_TaxableAmt) + parseFloat(Ins_Trand_TaxableAmt) + parseFloat(TOTCGSTAMT.toFixed(0)) + parseFloat(TOTSGSTAMT.toFixed(0)) + parseFloat(TOTIGSTAMT.toFixed(0));

    ROCHECK = Math.abs(Math.round(round, 0) - parseFloat(round)).toFixed(2);
    if (isNaN(ROCHECK))
        ROCHECK = 0;


    $('.TRANROAMT').val(ROCHECK);
    TRANROAMT = Math.round(round, 0) - parseFloat(round);

   // TRANNAMT = (eval(TRANGAMT) + eval(CF_ROW_TOTAL)+eval(TRANROAMT)).toFixed(2);
    TRANNAMT = (parseFloat(round) + parseFloat(TRANROAMT)).toFixed(2);

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

        var $tableBody = $('#CFACTOR').find("tbody"),
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

            pos.find('td:eq(1)').html("<input type=text value=" + tax + " id='TAX' class='TAX' name='TAX' style='display:none' >" + desc + "<input type=text style='border:none' readonly=readonly value='" + desc + "' name=CFDESC id='CFDESC' class='CFDESC hide'> ");

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
                    $trFirst.after("<tr class='item-row'><Td> <button href='#'   type='button' onclick='del_factor(this)' class='btn btn-danger dfact btn-xs'><i class=glyphicon-minus></i> </button>  </td> <td>" + data + " </td><td><input  type=text value='' name=CFAMOUNT id='CFAMOUNT' class='CFAMOUNT' readonly='readonly'> </td>  </TD></tr>");


                total();


            },
            error: function (jqXHR, textStatus, errorThrown) {

            }
        });

        total();
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