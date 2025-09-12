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

    count = 0; TRANSAMT = 0; TRANHAMT = 0; TRANEAMT = 0; TRANFAMT = 0; TRANPAMT = 0; TRANTCAMT = 0; TRANAAMT = 0;
    i = 0; //$("#TRANGAMT").val("0");
    $(".STFDIDS").each(function () {
       
        ckb = $(this).is(':checked');
     
        if (ckb == true) {
           
            $(".boolSTFDIDS")[i].value = ckb;
            if ($(".boolSTFDIDS")[i].value == "true") { count++; }
            var TRANDSAMT = $(".TRANDSAMT")[i].value;
            var TRANDHAMT = $(".TRANDHAMT")[i].value;
            var TRANDFAMT = $(".TRANDFAMT")[i].value;
            var TRANDPAMT = $(".TRANDPAMT")[i].value;
            var TRANDEAMT = $(".TRANDEAMT")[i].value;
            if (isNaN(TRANDSAMT)) TRANDSAMT = "0"; if (isNaN(TRANDHAMT)) TRANDHAMT = "0"; if (isNaN(TRANDEAMT)) TRANDEAMT = "0"; if (isNaN(TRANDFAMT)) TRANDFAMT = "0"; if (isNaN(TRANDPAMT)) TRANDPAMT = "0";
            total = parseFloat(TRANDSAMT) + parseFloat(TRANDHAMT) + parseFloat(TRANDEAMT) + parseFloat(TRANDFAMT) + parseFloat(TRANDPAMT);

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
            //if (isNaN(TRANAAMT)) TRANAAMT = "0";
            //TRANAAMT = TRANAAMT + eval($(".TRANDAAMT")[i].value);
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
    $("#TRANPAMT").val(TRANPAMT.toFixed(2));
    $("#TRANAAMT").val("0.00");


    $("#TRANGAMT").val(gamt.toFixed(2));
    $("#TRANNAMT").val(gamt.toFixed(2));
    $("#TRANREFAMT").val($("#TRANNAMT").val());
    TRANGAMT = $("#TRANGAMT").val();
    var round = 0.00; i = 0; NETGAMT = 0;
    $('.TAX').each(function () {
       
     //   $(".DEDNOS")[i].value = count;
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
    round = (eval(TRANGAMT) + eval(CF_ROW_TOTAL));

    ROCHECK = Math.abs(Math.round(round, 0) - parseFloat(round)).toFixed(2);
    if (isNaN(ROCHECK))
        ROCHECK = 0;


    $('.TRANROAMT').val(ROCHECK);
    TRANROAMT = Math.round(round, 0) - parseFloat(round);

    TRANNAMT = (eval(TRANGAMT) + eval(CF_ROW_TOTAL)+eval(TRANROAMT)).toFixed(2);

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
            pos = $('#CFACTOR tr').eq(($('#CFACTOR tr').length-($('#CFACTOR tr').length))+1);
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