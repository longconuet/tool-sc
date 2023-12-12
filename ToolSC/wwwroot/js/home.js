$(document).ready(function () {
    // Auto-resize textarea
    initAutoResize();

    $("#search-column").on("input", function () {
        // Lấy giá trị từ trường tìm kiếm
        var searchText = $(this).val().toLowerCase();

        // Ẩn tất cả các kết quả tìm kiếm
        $(".column-name-item").hide();

        // Hiển thị các kết quả tìm kiếm phù hợp với giá trị nhập vào
        $(".column-name-item:contains('" + searchText + "')").show();
    });
});

// Mở rộng jQuery để hỗ trợ :contains không phân biệt chữ hoa/chữ thường
$.expr[':'].contains = function (a, i, m) {
    return jQuery(a).text().toLowerCase()
        .indexOf(m[3].toLowerCase()) >= 0;
};

function copyToClipboard(sourceEl) {
    // Lấy nội dung cần sao chép
    var copyText = $(`#${sourceEl}`);
    if (copyText.val() === '') {
        toastr.warning('Nothing to copy', '')
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

function showEnterDataManuallyModal() {
    let tableDesign = $('#table-design').val();
    if (tableDesign == '') {
        toastr.error('No table design', '');
        return;
    }

    splitTableColumn(tableDesign);

    $('#enter-data-modal').modal('show');
}

function splitTableColumn(tableDesign = "") {
    let html = '';

    // Chia chuỗi thành mảng các dòng
    var lines = tableDesign.split("\n");

    // Lặp qua từng dòng và xử lý
    $.each(lines, function (index, line) {
        // Kiểm tra nếu dòng không trống
        if (line.trim() !== "") {
            // Sử dụng biểu thức chính quy để lấy ra văn bản 予定枝番
            var match = line.match(/<([^,>]+),/);

            // Kiểm tra nếu có sự trùng khớp
            if (match) {
                var name = match[1];
                html += `<div class="col-md-6 column-name-item">`;
                html += `   <div class="mb-1 row">`;
                html += `       <label class="col-md-6 col-form-label">${line.replace(/[<,>]/g, '')}</label>`;
                html += `       <div class="col-md-6">`;
                html += `           <input type="text" class="form-control column-name-input" data-column-name=${name}>`;
                html += `       </div>`;
                html += `   </div>`;
                html += `</div>`
            }           
        }
    });

    $('#enter-data-content').html(html);
}