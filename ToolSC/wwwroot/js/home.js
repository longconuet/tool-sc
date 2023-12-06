$(document).ready(function () {
    $("#copyButton").on("click", function () {
        var hiddenInput = $("#hiddenInput");

        // Copy text from the hidden input
        hiddenInput.select();
        document.execCommand("copy");

        console.log("Text successfully copied to clipboard");
    });
});

function getColumnDetail() {
    if ($('#input').val() == '') {
        toastr.error("Please enter input", "Error");
        return;
    }

    var dataObj = {
        Input: $('#input').val()
    };

    $.ajax({
        url: "/Home/GetColumnDetail",
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

function genTableData() {
    if ($('#input').val() == '') {
        toastr.error("Please enter input", "Error");
        return;
    }

    var dataObj = {
        Input: $('#input').val()
    };

    $.ajax({
        url: "/Home/GenTableData",
        data: JSON.stringify(dataObj),
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        async: true,
        processData: false,
        success: function (result) {
            if (result.status == 1) {
                let htmlDiv = '';
                $.each(result.data, function (index, item) {
                    htmlDiv += `<div>${item}</div>`;
                });
                $('#table-data').html(htmlDiv);
            } else {
                toastr.error(result.msg, "Error");
            }
        },
        error: function (errormessage) {
            toastr.error(errormessage.responseText, "Error");
        }
    });
}

function genTableDataFullLength() {
    if ($('#input').val() == '') {
        toastr.error("Please enter input", "Error");
        return;
    }

    var dataObj = {
        Input: $('#input').val()
    };

    $.ajax({
        url: "/Home/GenTableDataFullLength",
        data: JSON.stringify(dataObj),
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        async: true,
        processData: false,
        success: function (result) {
            if (result.status == 1) {
                let htmlDiv = '';
                $.each(result.data, function (index, item) {
                    htmlDiv += `<div>${item}</div>`;
                });
                $('#table-data').html(htmlDiv);
            } else {
                toastr.error(result.msg, "Error");
            }
        },
        error: function (errormessage) {
            toastr.error(errormessage.responseText, "Error");
        }
    });
}