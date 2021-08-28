function updateDeptnlocation(svalue, Id, type) {
    if (type == 'department') {
        jQuery.ajax({
            url: baseUrl + "Global/FunGetGDeptListbymultiBranch",
            type: 'POST',
            dataType: "json",
            data: { 'branch': svalue },
            success: function (result) {

                $("#" + Id).children('option').remove();
                $("#" + Id).trigger('change');
                if (result != null && result.length > 0) {
                    $.each(result, function (index, item) {
                        $("#" + Id).append("<option value='" + item.Value + "'>" + item.Text + "</option>");
                    });
                    $("#" + Id).trigger('change');
                    return;
                }
            }
        });
    } else  {
        jQuery.ajax({
            url: baseUrl + "Global/FunGetGLocListbymultiBranch",
            type: 'POST',
            dataType: "json",
            data: { 'branch': svalue },
            success: function (result) {

                $("#" + Id).children('option').remove();
                $("#" + Id).trigger('change');
                if (result != null && result.length > 0) {
                    $.each(result, function (index, item) {
                        $("#" + Id).append("<option value='" + item.Value + "'>" + item.Text + "</option>");
                    });
                    $("#" + Id).trigger('change');
                    return;
                }
            }
        });
    } 
}


function updateEmployeeList(sBvalue, sDvalue, sLvalue,Id) {
    
        jQuery.ajax({
            url: baseUrl + "Global/FunGetGEmpListBymulitBDL",
            type: 'POST',
            dataType: "json",
            data: { 'sDivision': sBvalue, 'sDept': sDvalue, 'sLoc': sLvalue },
            success: function (result) {

                $("#" + Id).children('option').remove();
                $("#" + Id).trigger('change');
                if (result != null && result.length > 0) {
                    $("#" + Id).append("<option value=''> Select</option>");
                    $.each(result, function (index, item) {
                        $("#" + Id).append("<option value='" + item.Value + "'>" + item.Text + "</option>");
                    });
                    $("#" + Id).trigger('change');
                    return;
                }
            }
        });
}


function updateRoleList(sBvalue, sDvalue, sLvalue, Id, type) {
   
        jQuery.ajax({
            url: baseUrl + "Global/FunGetGRoleList",
            type: 'POST',
            dataType: "json",
            data: { 'sDivision': sBvalue, 'sDept': sDvalue, 'sLoc': sLvalue, 'sRole': type },
            success: function (result) {

                $("#" + Id).children('option').remove();
                $("#" + Id).trigger('change');
                if (result != null && result.length > 0) {
                    $("#" + Id).append("<option value=''> Select</option>");
                    $.each(result, function (index, item) {
                        $("#" + Id).append("<option value='" + item.Value + "'>" + item.Text + "</option>");
                    });
                    $("#" + Id).trigger('change');
                    return;
                }
            }
        });  
}

function updateRoleLikeList(sBvalue, sDvalue, sLvalue, Id, type) {

    jQuery.ajax({
        url: baseUrl + "Global/FunGetGRoleLikeList",
        type: 'POST',
        dataType: "json",
        data: { 'sDivision': sBvalue, 'sDept': sDvalue, 'sLoc': sLvalue, 'sRole': type },
        success: function (result) {

            $("#" + Id).children('option').remove();
            $("#" + Id).trigger('change');
            if (result != null && result.length > 0) {
                $("#" + Id).append("<option value=''> Select</option>");
                $.each(result, function (index, item) {
                    $("#" + Id).append("<option value='" + item.Value + "'>" + item.Text + "</option>");
                });
                $("#" + Id).trigger('change');
                return;
            }
        }
    });
}


function FileFormatValidation(file) {
    var ext = file.value.split(".").pop().toLowerCase();
    if ($.inArray(ext, ["pdf", "jpg", "jpeg", "gif", "png", "xls", "xlsx", "doc", "docx"]) == -1) {
        file.value = "";
        bootbox.alert({
            title: 'File type',
            message: 'Invalid file format, please upload Pdf,docx,doc,xls,xlsx,png,gif,jpg,jpeg format.'
        });
    }
}