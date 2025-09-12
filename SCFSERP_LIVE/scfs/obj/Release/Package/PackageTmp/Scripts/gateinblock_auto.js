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
                                    jsonArg.label = data;
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
                                case 7:
                                    jsonArg.xparam5 = data;
                                    break;
                                case 8:
                                    jsonArg.xparam6 = data;
                                    break
                                case 9:
                                    jsonArg.xparam7 = data;
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
            count = 0;
            $.each(controls.split(','), function (index, value) {
                switch (count) {
                    case 0: break;
                    default:
                        $("#" + value).val(""); break;
                } count++;
            })
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
                        break;
                    case 7:
                        $("#" + value).val(ui.item.xparam5);
                        break;
                    case 8:
                        $("#" + value).val(ui.item.xparam6);
                        break;
                    case 9:
                        var MyDate_String_Value = ui.item.xparam7;
                        var f_value = new Date
                                    (
                                         parseInt(MyDate_String_Value.replace(/(^.*\()|([+-].*$)/g, ''))
                                    );
                        var dat = f_value.getMonth() +
                                                 1 +
                                               "/" +
                                   f_value.getDate() +
                                               "/" +
                               f_value.getFullYear();
                      
                        $("#" + value).val(dat);
                        break
                }
                count++
            });
            size();
            return false
        },
        messages: {
            noResults: "",
            results: ""
        }
    })
}