$(document).ready(function () {
    $('#copy-btn').on('click', function () {
        
    });

    // Auto-resize textarea
    initAutoResize();
});

function copyToClipboard(sourceEl) {
    // Lấy nội dung cần sao chép
    var copyText = $(`#${sourceEl}`);
    if (copyText.val() === '') {
        return;
    }

    // Tạo một phần tử input tạm để sao chép nội dung vào đó
    var tempInput = $('<textarea>');
    $('body').append(tempInput);
    tempInput.val(copyText.val()).select();

    // Sử dụng API Clipboard để sao chép nội dung vào Clipboard
    document.execCommand('copy');

    // Xóa phần tử input tạm đi
    tempInput.remove();

    // Thông báo hoặc thực hiện các hành động khác sau khi sao chép
    alert('Copied to clipboard: ' + copyText.val());
}

function initAutoResize() {
    $('textarea').on('input', function () {
        this.style.height = 'auto';
        this.style.height = (this.scrollHeight) + 'px';
    });
}

function getColumnDetail() {
    if ($('#table-design').val() == '') {
        toastr.error("Please enter input", "Error");
        return;
    }

    var dataObj = {
        Input: $('#table-design').val()
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
    if ($('#table-design').val() == '') {
        toastr.error("Please enter input", "Error");
        return;
    }

    var dataObj = {
        Input: $('#table-design').val()
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
                $.each(result.data.dataList, function (index, item) {
                    htmlDiv += `<div>${item}</div>`;
                });
                $('#table-data').html(htmlDiv);
                $('#table-data-textarea').val(result.data.data);
                initAutoResize();
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
    if ($('#table-design').val() == '') {
        toastr.error("Please enter input", "Error");
        return;
    }

    var dataObj = {
        Input: $('#table-design').val()
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
                $.each(result.data.dataList, function (index, item) {
                    htmlDiv += `<div>${item}</div>`;
                });
                $('#table-data').html(htmlDiv);
                $('#table-data-textarea').val(result.data.data);
                initAutoResize();
            } else {
                toastr.error(result.msg, "Error");
            }
        },
        error: function (errormessage) {
            toastr.error(errormessage.responseText, "Error");
        }
    });
}