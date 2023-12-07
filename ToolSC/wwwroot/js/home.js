$(document).ready(function () {
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
    toastr.success("Copied!", "");
    //alert('Copied to clipboard: ' + copyText.val());
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
        Input: $('#table-design').val(),
        KinoId: $('#kino-id').val(),
        TableName: $('#table-name').val(),
        SystemName: $('#system-name').val(),
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

                let htmlTextareaName = '';
                let htmlTextareaType = '';
                let htmlTextareaLength = '';

                $.each(result.data, function (index, item) {
                    htmlDivNo += `<div>${index + 1}</div>`;
                    htmlDivName += `<div>${item.name}</div>`;
                    htmlDivType += `<div>${item.type}</div>`;
                    htmlDivLength += `<div>${item.length != '' ? item.length : '-'}</div>`;

                    htmlTextareaName += `${item.name}\n`;
                    htmlTextareaType += `${item.type}\n`;
                    htmlTextareaLength += `${item.length != '' ? item.length : '-'}\n`;
                });

                $('#column-no').html(htmlDivNo);
                $('#column-name').html(htmlDivName + `<textarea id="column-name-textarea" hidden></textarea>` + `<button type="button" class="btn btn-primary" onclick="copyToClipboard('column-name-textarea')">Copy</button>`);
                $('#column-type').html(htmlDivType + `<textarea id="column-type-textarea" hidden></textarea>` + `<button type="button" class="btn btn-primary" onclick="copyToClipboard('column-type-textarea')">Copy</button>`);
                $('#column-length').html(htmlDivLength + `<textarea id="column-length-textarea" hidden></textarea>` + `<button type="button" class="btn btn-primary" onclick="copyToClipboard('column-length-textarea')">Copy</button>`);

                $('#column-name-textarea').val(htmlTextareaName);
                $('#column-type-textarea').val(htmlTextareaType);
                $('#column-length-textarea').val(htmlTextareaLength);
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

    let numberRecord = $('#number-record').val() != '' ? parseInt($('#number-record').val()) : 1;
    if (numberRecord > 1 && $('#column-key').val() == '') {
        toastr.error("Please enter column key", "Error");
        return;
    }

    var dataObj = {
        Input: $('#table-design').val(),
        SiteCode: $('#site-code').val(),
        KinoId: $('#kino-id').val(),
        ColumnKey: $('#column-key').val(),
        NumberRecord: $('#number-record').val() != '' ? parseInt($('#number-record').val()) : 1,
        TableName: $('#table-name').val(),
        SystemName: $('#system-name').val(),
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

                if (result.data.multiData && result.data.multiData.length > 0) {
                    htmlDiv += `<div class="mt-3"><button type="button" class="btn btn-primary" onclick="showNormalDataModal()">Show more data</button></div>`;
                    
                    let htmlthead = '<thead><tr>';
                    let htmltbody = '<tbody><tr>';
                    $.each(result.data.multiData, function (index, multiData) {
                        htmlthead += `<th>Data ${index + 1}</th>`;

                        let htmlTd = '<td>';
                        $.each(multiData.dataList, function (index, multiDataItem) {
                            htmlTd += `<div>${multiDataItem}</div>`;
                        });
                        htmlTd += '</td>';
                        htmltbody += htmlTd;
                    });
                    htmlthead += '</tr></thead>';
                    htmltbody += '</tr></tbody>';
                    $('#normal-table').html(htmlthead + htmltbody);
                }                
                
                $('#table-data').html(htmlDiv);
                $('#table-data-textarea').val(result.data.data);
                $('#table-data-column').val(result.data.dataColumn);
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
        Input: $('#table-design').val(),
        SiteCode: $('#site-code').val(),
        KinoId: $('#kino-id').val(),
        ColumnKey: $('#column-key').val(),
        NumberRecord: $('#number-record').val() != '' ? parseInt($('#number-record').val()) : 1,
        TableName: $('#table-name').val(),
        SystemName: $('#system-name').val(),
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
                $('#table-data-column').val(result.data.dataColumn);
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

function showNormalDataModal() {
    $('#normal-modal').modal('show');
}