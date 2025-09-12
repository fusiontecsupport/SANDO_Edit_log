
function isNumberOnlyKey(evt) {
    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if ((charCode >= 48 && charCode <= 57) || ((charCode <= 110 && charCode >= 96))) {
        return true;    
    }
    else
        return false;
    
}

function isDate(txtDate) {
    //return txtDate instanceof Date && !isNaN(date.getTime());
    var currVal = txtDate;
    var dtVal = txtDate.split('/');
    if (dtVal == null)
        dtVal = txtDate.split('-');
    //Declare Regex  

    var rxDatePattern = /^(?:(?:31(\/|-|\.)(?:0?[13578]|1[02]))\1|(?:(?:29|30)(\/|-|\.)(?:0?[13-9]|1[0-2])\2))(?:(?:1[6-9]|[2-9]\d)?\d{2})$|^(?:29(\/|-|\.)0?2\3(?:(?:(?:1[6-9]|[2-9]\d)?(?:0[48]|[2468][048]|[13579][26])|(?:(?:16|[2468][048]|[3579][26])00))))$|^(?:0?[1-9]|1\d|2[0-8])(\/|-|\.)(?:(?:0?[1-9])|(?:1[0-2]))\4(?:(?:1[6-9]|[2-9]\d)?\d{2})$/
    var dtArray = currVal.match(rxDatePattern); // is format OK?
    //alert(dtVal);
    //Checks for mm/dd/yyyy format.
    dtMonth = eval(dtVal[1]);
    dtDay = eval(dtVal[0]);
    dtYear = eval(dtVal[2]);
    alert(dtMonth);
    if (currVal == '' || dtArray == null)
        return false;
    else if (dtMonth < 1 || dtMonth > 12)
        return false;
    else if (dtDay < 1 || dtDay > 31)
        return false;
    else if ((dtMonth == 4 || dtMonth == 6 || dtMonth == 9 || dtMonth == 11) && dtDay == 31)
        return false;
    else if (dtMonth == 2) {
        var isleap = (dtYear % 4 == 0 && (dtYear % 100 != 0 || dtYear % 400 == 0));
        if (dtDay > 29 || (dtDay == 29 && !isleap))
            return false;
    }
    else
        return true;
}


function isAlphaNumberOnlyKey(e) {
    var key = e.which || e.keyCode;
    if (e.shiftKey && key >= 48 && key <= 57) {
        return false;
    }
    else {
        if (key >= 186 && key <= 187 || key >= 191 && key <= 222 || key == 32) {
            return false;
        }
        else {
            return true;
        }
    }
}

function isNumerDecimalOnly(evt, element) {
    var charCode = (evt.which) ? evt.which : event.keyCode
    if (charCode > 31 &&  !((charCode >= 48 && charCode <= 57) || ((charCode <= 110 && charCode >= 96))) && !(charCode == 46 || charCode == 8))
        return false;
    else {
        var len = $(element).val().length;
        var index = $(element).val().indexOf('.');
        if (index > 0 && charCode == 46) {
            return false;
        }
        if (index > 0) {
            var CharAfterdot = (len + 1) - index;
            if (CharAfterdot > 3) {
                return false;
            }
        }

    }
    return true;
}

function isNumberCommaDot(evt) {
    var theEvent = evt || window.event;
    var key = theEvent.keyCode || theEvent.which;

    if (key === 9) { //TAB was pressed
        return;
    }

    key = String.fromCharCode(key);
    if (key.length == 0) return;
    var regex = /^[0-9,\9\b]*\.?[0-9]*$/;
    var charCode = (evt.which) ? evt.which : event.keyCode
    if (!regex.test(key) && !((charCode >= 48 && charCode <= 57) || ((charCode <= 110 && charCode >= 96)))) {
        theEvent.returnValue = false;
        if (theEvent.preventDefault) theEvent.preventDefault();
    }
}

function isNumerDecimalFourOnly(evt, element) {
    var charCode = (evt.which) ? evt.which : event.keyCode
    if (charCode > 31 &&  !((charCode >= 48 && charCode <= 57) || ((charCode <= 110 && charCode >= 96))) && !(charCode == 46 || charCode == 8))
        return false;
    else {
        var len = $(element).val().length;
        var index = $(element).val().indexOf('.');
        if (index > 0 && charCode == 46) {
            return false;
        }
        if (index > 0) {
            var CharAfterdot = (len + -1) - index;
            if (CharAfterdot > 3) {
                return false;
            }
        }

    }
    return true;
}


//function isAlphaNumberOnlyKey(e) {
//    var k = evt.keyCode || evt.which;
//    var ok = k >= 65 && k <= 90 || // A-Z
//        k >= 96 && k <= 105 || // a-z
//        k >= 35 && k <= 40 || // arrows
//        k == 9 || //tab
//        k == 46 || //del
//        k == 8 || // backspaces
//        (!evt.shiftKey && k >= 48 && k <= 57); // only 0-9 (ignore SHIFT options)

//    if (!ok || (evt.ctrlKey && evt.altKey)) {
//        evt.preventDefault();
//    }
//}

function isAlphaNumberOnlyKey1(evt) {
    var keyCode = e.keyCode || e.which;

    //Regex for Valid Characters i.e. Alphabets and Numbers.
    var regex = /^[A-Za-z0-9]+$/;

    var isValid = regex.test(String.fromCharCode(keyCode));
    if (!isValid) {
        SwalErrMsg("Only Alphabets and Numbers allowed !!");
        return false;
    }
    else {
        return true;
    }
}

function emailvalidate(email) {
    var aemail = email;
    var expr = /^([\w-\.]+)@@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;
    if (!expr.test(aemail)) {
       SwalErrMsg("Invalid email address. !!");
        return false;        
    }
    else {
        return true;
    }
}

function dfdemailvalidate(email) {        
    var expr = /^([\w-\.]+)@@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;    
    if (!expr.test(email)) {
        SwalErrMsg("Invalid email address.");
    }    
}

function add_autocomplete($obj, controls) {
    var oldFn = $.ui.autocomplete.prototype._renderItem;
    $.ui.autocomplete.prototype._renderItem = function (ul, item) {
        var re = new RegExp(this.term, "i");
        var t = item.label.replace(re, "<strong style='font-weight:bold;background:#C7DFFE;border-radius:2px;border:1px solid #98B8E1'>" + this.term + "</strong>");
        return $("<li></li>")
            .data("item.autocomplete", item)
            .append("<a>" + t + "</a>")
            .appendTo(ul);
    };
    $obj.autocomplete({
        source: function (request, response) {
            $.ajax({
                url: $obj.data("autocomplete-url"),
                type: "POST",
                dataType: "json",
                max: 10,
                scrollable: true,
                data: {
                    term: request.term
                },
                success: function (data) {

                    response($.map(data, function (item) {
                        count = 0;
                        item_str = "";
                        var jsonArg = new Object();
                        count = 0;
                        $.each(item, function (i, data) {
                            switch (count) {
                                case 0:
                                    jsonArg.label = data;// + " (" + item['WRDCODE'] + ")";
                                    jsonArg.value = data;
                                    break;
                                case 1:
                                    jsonArg.id = data;
                                    break;
                                case 2:
                                    jsonArg.desc = data;
                                    break;
                                case 3:
                                    jsonArg.xparam1 = data;
                                    break;
                                case 4:
                                    jsonArg.xparam2 = data;
                                    break;
                                case 5:
                                    jsonArg.xparam3 = data;
                                    break
                                case 6:
                                    jsonArg.xparam4 = data;
                                    break


                            }
                            count++
                        });
                        return jsonArg
                    }))
                }
            })
        },
        search: function () {
            var term = extractLast(this.value);
            if (term.length < 2) {
                return false
            }
        },
        select: function (event, ui) {
            $(this).val(ui.item.label);
            count = 0;
            $.each(controls.split(','), function (index, value) {
                switch (count) {

                    case 1:
                        $("#" + value).val(ui.item.id);
                        break;
                    case 2:
                        $("#" + value).val(ui.item.desc);
                        break;
                    case 3:
                        $("#" + value).val(ui.item.xparam1);
                        break;
                    case 4:
                        $("#" + value).val(ui.item.xparam2);
                        break;
                    case 5:
                        $("#" + value).val(ui.item.xparam3);
                        break
                    case 6:
                        $("#" + value).val(ui.item.xparam4);
                        break



                }
                count++
            });



            return false
        },
        change: function (event, ui) {
            //debugger;
            var opt = $(this).val();

            var crntval = event.currentTarget.value;
            $.ajax({
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                url: $obj.data("autocomplete-url"),
                data: "{'term':'" + crntval + "'}",
                dataType: 'json',
                success: function (data) {
                    //debugger;
                    if (data.length == 0) {
                        //$(event.currentTarget).val('');
                        alert('Select Items from the list.')
                        //SwalErrMsg('Select Items from the list.');
                    }
                },
                error: function (data) {
                    $(event.currentTarget).val('');
                    console.log('Error retrieving options.');
                }
            });
        },
        messages: {
            noResults: "",
            results: ""
        }
    })
}

function isAlphaNumeric(chkval) {
    var TCode = chkval;
    var Exp = /((^[0-9]+[a-z]+)|(^[a-z]+[0-9]+))+[0-9a-z]+$/i;
    if (!TCode.match(Exp))
        return false;
    else
        return true;
}

function isGstNumber(chkgst) {
    var gst_value = chkgst.toUpperCase();
    var reg = /^([0-9]{2}[a-zA-Z]{4}([a-zA-Z]{1}|[0-9]{1})[0-9]{4}[a-zA-Z]{1}([a-zA-Z]|[0-9]){3}){0,15}$/;
    if (gst_value.match(reg)) {
        return true;
    } else {
        return false;
    }
}

function GetFormattedDate1(obj) {
    alert(obj);
    var MyDate_String_Value = obj;
    
    var JavaScriptDate = new Date(parseInt(MyDate_String_Value.substr(6))); //to js format
    var dt = new Date(obj); //Date object
    //alert(dt);
    //console.log(dt);
    var year = dt.getFullYear();
    var month = dt.getMonth() + 1;
    
    var day = dt.getDate();
    var day1 = month;
    var month1 = day;
    
    month = month1;
    day = day1;

    if (month < 10) month = "0" + month;
    if (day < 10) day = "0" + day;

    var dat = year + month + day;
    //alert(dat);
    
    return dat;
}

function datdidff(start_date, end_date) {
    if (start_date.match("/")) { start_date = start_date.split("/"); }
    else if (start_date.match("-")) { start_date = start_date.split("-"); }

    if (end_date.match("/")) { end_date = end_date.split("/"); }
    else if (end_date.match("-")) { end_date = end_date.split("-"); }
    // end_date = end_date.split("/");


    start_date = start_date[2] + "-" + start_date[1] + "-" + start_date[0];
    end_date = end_date[2] + "-" + end_date[1] + "-" + end_date[0];

    var diff = Math.floor((Date.parse(end_date.replace(/-/g, '\/')) - Date.parse(start_date.replace(/-/g, '\/'))) / 86400000);

    return diff;

}

function Splitthedate(obj) {
    var pieces = obj.split("/");

    var year = pieces[2];
    var month = pieces[1];
    var day = pieces[0];

    var dat = year + month + day;

    return dat;
    //$("#CID").val(pieces[0]);
    //$("#CIM").val(pieces[1]);
    //$("#CIY").val(pieces[2]);
}


function CompareDates(rawdate1, rawdate2) {
    if (rawdate1 != "" && rawdate1 != undefined && rawdate1 != null && rawdate2 != "" && rawdate2 != undefined && rawdate2 != null) {
        var pieces1 = rawdate1.split("/");
        var pieces2 = rawdate2.split("/");

        var year1 = pieces1[2];
        var month1 = pieces1[1];
        var day1 = pieces1[0];
        //alert(year1);
        //alert(month1);
        //alert(day1);
        var year2 = pieces2[2];
        var month2 = pieces2[1];
        var day2 = pieces2[0];
        //alert(year2);
        //alert(month2);
        //alert(day2);
        
        var dat1 = new Date(year1, month1-1, day1);
        var dat2 = new Date(year2, month2-1, day2);
        //alert(dat1);
        //alert(dat2);
        if (dat1 > dat2) {
            console.log("Date 1 is greater than Date 2");
            return 1;
        } else if (dat1 < dat2) {
            console.log("Date 1 is less than Date 2");
            return 2;
        } else if (dat1.getTime() === dat2.getTime()) {
            console.log("Both Dates are same");
            return 0;
        }
    }
    else {
        console.log("Not Valid Dates");
        return -1;
    }
    //$("#CIM").val(pieces[1]);
    //$("#CIY").val(pieces[2]);
}

function Splitthetime(obj) {
    var pieces = obj.split(":");

    var hour = pieces[0];
    var minutes = pieces[1];
   
    var dat = hour + minutes;

    return dat;
    //$("#CID").val(pieces[0]);
    //$("#CIM").val(pieces[1]);
    //$("#CIY").val(pieces[2]);
}

function GetFormattedDate(obj) {
    var MyDate_String_Value = obj;
    var value1 = new Date
        (
            parseInt(MyDate_String_Value.replace(/(^.*\()|([+-].*$)/g, ''))
        );

    var year = value1.getFullYear();
    var month = value1.getMonth() + 1;
    var day = value1.getDate();

    if (month < 10) month = "0" + month;
    if (day < 10) day = "0" + day;

    var dat = day +
        "/" +
        month +
        "/" +
        year;
    return dat;
}

function GetFormattedTime(obj) {
    var MyDate_String_Value = obj;

    var dt = new Date
        (
            parseInt(MyDate_String_Value.replace(/(^.*\()|([+-].*$)/g, ''))
        );

    var Hours = dt.getHours();
    var Minutes = dt.getMinutes();
    var Seconds = dt.getSeconds();

    if (Hours < 10) Hours = "0" + Hours;
    if (Minutes < 10) Minutes = "0" + Minutes;
    if (Seconds < 10) Seconds = "0" + Seconds;

    var dat = Hours + ':' + Minutes + ':' + Seconds;

    return dat;
}

function GetFormattedDateTime(obj) {
    var MyDate_String_Value = obj;
    var value1 = new Date
        (
            parseInt(MyDate_String_Value.replace(/(^.*\()|([+-].*$)/g, ''))
        );

    var year = value1.getFullYear();
    var month = value1.getMonth() + 1;
    var day = value1.getDate();

    if (month < 10) month = "0" + month;
    if (day < 10) day = "0" + day;

    var Hours = value1.getHours();
    var Minutes = value1.getMinutes();
    var Seconds = value1.getSeconds();

    if (Hours < 10) Hours = "0" + Hours;
    if (Minutes < 10) Minutes = "0" + Minutes;
    if (Seconds < 10) Seconds = "0" + Seconds;

    var dat = day +
        "/" +
        month +
        "/" +
        year + " " + Hours + ":" + Minutes + ":" + Seconds;

    return dat;
}

function SwalErrMsg(msg) {
    //Swal.fire(
    //    'Error!',
    //    '<strong>'+ msg+'</strong>',
    //    'error'
    //);
    alert(msg);

}

function SwalSucMsg(msg) {
    //Swal.fire(
    //    'Success!',
    //    '<strong>' + msg + '</strong>',
    //    'success'
    //);
    alert(msg);

}

function SwalInfMsg(msg) {
    //Swal.fire(
    //    'Information!',
    //    '<strong>' + msg + '</strong>',
    //    'info'
    //);
    alert(msg);

}

function SwalAlert(msg) {
    //Swal.fire(
    //    'Alert!',
    //    '<strong>' + msg + '</strong>',
    //    'info'
    //);
    alert(msg);

}
