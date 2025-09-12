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
    TRANGAMT = 0;
    $('.TRANDID').each(function () {

        TRANDQTY = $(".TRANDQTY")[i].value;
        if (TRANDQTY == "")
            TRANDQTY = 0;
        TRANDBRATE = $(".TRANDBRATE")[i].value;
        TRANDDISCEXPRN = $(".TRANDDISCEXPRN")[i].value;
        TRANDFEXPRN = $(".TRANDFEXPRN")[i].value;
        TRANDAAMT = $(".TRANDAAMT")[i].value;

        TEMP_PUR = TRANDAAMT * TRANDQTY;
        TRANDDISCAMT = 0;
        TRANDFAMT = 0;


        if (TRANDDISCEXPRN > 0)
            TRANDDISCAMT = ((TRANDBRATE * TRANDDISCEXPRN) / 100).toFixed(2);

        if (TRANDFEXPRN > 0)
            TRANDFAMT = ((TRANDBRATE * TRANDFEXPRN) / 100).toFixed(2);



        j = 0;
        TRANDPACKAMT = 0;
        TRANDPACKEXPRN = 0;
        TRANDEAMT = 0;
        TRANDEEXPRN = 0;
        TRANDEDUEXPRN = 0;
        TRANDEDUAMT = 0;
        TRANDSHEDEXPRN = 0;
        TRANDSHEDAMT = 0;
        TRANDTEXPRN = 0;
        TRANDTAMT = 0;

        TRANDPEAMT = 0;
        TRANDPEDUAMT = 0;
        TRANDPSHEDAMT = 0;

        $('.CFNO').each(function () {

            DORDRID = $('.CFNO')[j].value;
            DEDEXPRN = $('.CF')[j].value;



            switch (DORDRID) {
                case "2":

                    TRANDPACKEXPRN = DEDEXPRN;
                    TRANDPACKAMT = ((TRANDBRATE * DEDEXPRN) / 100).toFixed(2);

                    break;
                case "3":

                    EXP3 = TRANDBRATE - TRANDDISCAMT + eval(TRANDPACKAMT);

                    TRANDEEXPRN = DEDEXPRN;
                    TRANDEAMT = ((EXP3 * DEDEXPRN) / 100).toFixed(2);


                    TRANDPEAMT = ((TEMP_PUR * DEDEXPRN) / 100).toFixed(2);

                    break;
                case "4":

                    TRANDEDUEXPRN = DEDEXPRN;
                    TRANDEDUAMT = ((TRANDEAMT * DEDEXPRN) / 100).toFixed(2);

                    TRANDPEDUAMT = ((TRANDPEAMT * DEDEXPRN) / 100).toFixed(2);

                    break;
                case "5":
                    TRANDSHEDEXPRN = DEDEXPRN;
                    TRANDSHEDAMT = ((TRANDEAMT * DEDEXPRN) / 100).toFixed(2);

                    TRANDPSHEDAMT = ((TRANDPEAMT * DEDEXPRN) / 100).toFixed(2);


                    break;
                case "6":
                    TRANDTEXPRN = DEDEXPRN;
                    EXP6 = eval(TRANDBRATE) - eval(TRANDDISCAMT) + eval(TRANDPACKAMT) + eval(TRANDEAMT) + eval(TRANDEDUAMT) + eval(TRANDSHEDAMT);
                    TRANDTAMT = ((EXP6 * DEDEXPRN) / 100).toFixed(2);
                    break;
            }
            j++;
        });

        TRANDGRATE = (eval(TRANDBRATE) - eval(TRANDDISCAMT) + eval(TRANDPACKAMT) + eval(TRANDEAMT) + eval(TRANDEDUAMT) + eval(TRANDSHEDAMT) + eval(TRANDTAMT)).toFixed(2);
        TRANDNRATE = (eval(TRANDGRATE) + eval(TRANDFAMT)).toFixed(2);
        TRANDGAMT = (eval(TRANDQTY) * eval(TRANDNRATE)).toFixed(2);
        TRANGAMT = (eval(TRANGAMT) + eval(TRANDGAMT)).toFixed(2);

        if (isNaN(TRANDGAMT))
            TRANDGAMT = 0;

        $(".TRANDTAMT")[i].value = TRANDGAMT;
        $(".TRANDDISCAMT")[i].value = TRANDDISCAMT;
        $(".TRANDFAMT")[i].value = TRANDFAMT;

        $(".TRANDBRATE")[i].value = TRANDBRATE;
        $(".TRANDGRATE")[i].value = TRANDGRATE;
        $(".TRANDNRATE")[i].value = TRANDNRATE;


        $(".TRANPACKAMT1")[i].value = TRANDPACKAMT;
        $(".TRANDPACKEXPRN")[i].value = TRANDPACKEXPRN;

        $(".TRANEAMT1")[i].value = TRANDEAMT;
        $(".TRANDEEXPRN")[i].value = TRANDEEXPRN;
        $(".TRANDEDUEXPRN")[i].value = TRANDEDUEXPRN;
        $(".TRANSHEDAMT1")[i].value = TRANDSHEDAMT;
        $(".TRANDSHEDEXPRN")[i].value = TRANDSHEDEXPRN;
        $(".TRANEDUAMT1")[i].value = TRANDEDUAMT;
        $(".TRANTAMT1")[i].value = TRANDTAMT;
        $(".TRANDTEXPRN")[i].value = TRANDTEXPRN;
        $(".TRANDPEAMT")[i].value = TRANDPEAMT;
        $(".TRANDPEDUAMT")[i].value = TRANDPEDUAMT;
        $(".TRANDPSHEDAMT")[i].value = TRANDPSHEDAMT;

        $(".ASSBL")[i].value = eval(TRANDBRATE) - eval(TRANDDISCAMT) + eval(TRANDPACKAMT);


        i++;

    });

    if (isNaN(TRANGAMT))
        TRANGAMT = 0;

    $("#TRANGAMT").val(TRANGAMT);
   
    i = 0;
    $('.TAX').each(function () {


        temp = 0;

        temp_tax = TRANGAMT;
       

      
        if (i > 0)
            temp_tax = eval(TRANGAMT) + eval(CF_ROW_TOTAL);



        if ($(".CFTYPE")[i].value == 1)
            temp = eval(temp_tax) * eval($(".CFEXPR")[i].value) / 100;
        else
            temp = eval($(".CFEXPR")[i].value).toFixed(2);




        switch ($(".DORDRID")[i].value) {
            case "4":

                if (i < 1) {
                    temp = 0;
                    alert("Correct Excise Order");

                    break;

                }
                ex = $(".DORDRID")[i - 1].value;
                ex = ex.trim();

                if ($(".CFTYPE")[i].value != 0)
                    temp = eval($(".CFAMOUNT")[i - 1].value) * eval($(".CFEXPR")[i].value) / 100;
                else
                    temp = eval($(".CFEXPR")[i].value);


                if (ex != "3") {
                    temp = 0;
                    alert("Correct Excise Order");
                }

                break;
            case "5":

                if (i < 2) {
                    temp = 0;
                    alert("Correct Excise Order");

                    break;

                }

                ex = $(".DORDRID")[i - 2].value;
                ex = ex.trim();

                sec = $(".DORDRID")[i - 1].value;
                sec = ex.trim();


                if ($(".CFTYPE")[i].value != 0)
                    temp = eval($(".CFAMOUNT")[i - 2].value) * eval($(".CFEXPR")[i].value) / 100;
                else
                    temp = eval($(".CFEXPR")[i].value);


                if (ex != "3" && sec != "4") {
                    temp = 0;
                    alert("Correct Excise Order");
                }



                break;
        }

        if (isNaN(temp))
            temp = 0;


        if ($(".CFMODE")[i].value == 0)
            CF_ROW_TOTAL = eval(CF_ROW_TOTAL) + eval(temp);
        else
            CF_ROW_TOTAL = eval(CF_ROW_TOTAL) - eval(temp);

        $(".CFAMOUNT")[i].value = eval(temp).toFixed(2);
        i++;
    });
    TRANNAMT = (eval(TRANGAMT) + eval(CF_ROW_TOTAL)).toFixed(2);

    if (isNaN(TRANNAMT))
        TRANNAMT = 0;

    $("#TRANNAMT").val(TRANNAMT);
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

        $("#cf_dynamic").css("display:block");


        var tax_param = "";
        i = 0;


        $('.TAX').each(function () {

            tax_param = tax_param + $.trim(this.value) + ",";
            pos = $('#CFACTOR tr').eq($('#CFACTOR tr').length - 1);
            idx = $('#CFACTOR tr').length - 2;

            desc = $(".CFDESC")[idx].value;
            pos.find('td:eq(1)').html("<input type=text value=" + this.value + " id='TAX' class='TAX' name='TAX' style='display:none' >" + desc + "<input type=text style='border:none' readonly=readonly value='" + desc + "' name=CFDESC id='CFDESC' class='CFDESC hide'> ");
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
                    $trLast.after("<tr class='item-row'><Td> <button href='#'   type='button' onclick='del_factor(this)' class='btn btn-default dfact'><i class=glyphicon-minus></i> </button>  </td> <td>" + data + " </td><td ><input  type=text value='' name=CFAMOUNT id='CFAMOUNT' class='CFAMOUNT' readonly='readonly'> </td>  </TD></tr>");




                total();


            },
            error: function (jqXHR, textStatus, errorThrown) {

            }
        });



        total();
        return false;

    });

});
