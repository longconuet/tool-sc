$(document).ready(function () {

});

function submit() {
    if ($('#input').val() == '') {
        toastr.error("Please enter input", "Error");
        return;
    }

    var dataObj = {
        Input: $('#input').val()
    };

    $.ajax({
        url: "/Home/Submit",
        data: JSON.stringify(dataObj),
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        async: true,
        processData: false,
        success: function (result) {
            if (result.status == 1) {
                let htmlDivNo = '';
                let htmlDivName = '';
                let htmlDivType = '';
                let htmlDivLength = '';
                $.each(result.data, function (index, item) {
                    htmlDivNo += `<div>${index + 1}</div>`;
                    htmlDivName += `<div>${item.name}</div>`;
                    htmlDivType += `<div>${item.type}</div>`;
                    htmlDivLength += `<div>${item.length != '' ? item.length : '-'}</div>`;
                });
                $('#column-no').html(htmlDivNo);
                $('#column-name').html(htmlDivName);
                $('#column-type').html(htmlDivType);
                $('#column-length').html(htmlDivLength);
            } else {
                toastr.error(result.msg, "Error");
            }
        },
        error: function (errormessage) {
            toastr.error(errormessage.responseText, "Error");
        }
    });
}