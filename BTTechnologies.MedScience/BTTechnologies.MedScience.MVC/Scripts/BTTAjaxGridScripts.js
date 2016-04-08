function GetGridData(tableName) {
    var tableContainer = $('#' + tableName);
    var url = $(tableContainer).attr("urlValue");
    var columns = [];
    var hiddenNumbers = [];
    var keyColumn = 0;
    var expression = '#' + tableName + ' th';
    var index = 0;
    var sortableColumn = $('#' + tableName + ' .BTT-ajax-table-sortable-img');
    var sortableColumnId = '';
    var ascentSort = true;
    var deleteColumnId = tableName + "DeleteColumn";
    var editColumnId = tableName + "EditColumn";
    
    if (sortableColumn != null || 0 < sortableColumn.length) {
        sortableColumnId = $(sortableColumn).attr('columnid');
        if ($(sortableColumn).attr('sortindicator') == 'down')
            ascentSort = false;
    }

    $(expression).not("#" + deleteColumnId).not("#" + editColumnId).each(
                function () {
                    columns.push($(this).attr('columnid'));

                    if ($(this).hasClass("hidden-column"))
                        hiddenNumbers.push(index);

                    if ($(this).hasClass("key-column"))
                        keyColumn = index;
                    
                    index++;
                }
            );

    var rowsPerPage = GetRowsPerPage(tableName);
    var curPage = $('#' + tableName + " .BTT-ajax-table-footer input").val();
    var data = { ColumnNames: columns, RowsPerPage: rowsPerPage, CurrentPage: curPage, SortableColumnId: sortableColumnId, AscentSort: ascentSort };
    data = JSON.stringify(data);

    var table = $('#' + tableName + ' > table');
    if (table == null)
        return;

    var addEditColumn = false;
    var editColumn = $("#" + editColumnId);
    if (editColumn != null && 0 < editColumn.length)
        addEditColumn = true;

    var addDeleteColumn = false;
    var deleteColumn = $("#" + deleteColumnId);
    if (deleteColumn != null && 0 < deleteColumn.length)
        addDeleteColumn = true;

    sendAjaxToUrlAndFillGrid(hiddenNumbers, table, tableName, url, data, addEditColumn, addDeleteColumn, keyColumn);
}

function GetRowsPerPage(tableName) {
    return parseInt($('#' + tableName + " .BTT-ajax-table-selected-item.BTT-ajax-table-perPage-button").text());
}

function sendAjaxToUrlAndFillGrid(hiddenNumbers, table, tableName, url, data, addEditColumn, addDeleteColumn, keyColumn) {
    $.ajax({
        cache: false,
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        data: data,
        url: url,
        success: function (inputData) {
            $('#' + tableName + ' > table > tbody > tr > td').remove();
            for (var i = 0; i < inputData.ResultValues.length; i++) {
                var keyValue = "";
                var row = '<tr>';
                for (var j = 0; j < inputData.ResultValues[i].length; j++) {
                    var classes = "";
                    if (j in hiddenNumbers)
                        classes += "hidden-column ";

                    if (j == keyColumn) {
                        classes += "key-column";
                        keyValue = inputData.ResultValues[i][j];
                    }

                    if (classes != "") {
                        row += '<td class="' + classes + '">';
                    } else {
                        row += '<td>';
                    }

                    row += inputData.ResultValues[i][j] + '</td>';
                }

                if (addEditColumn) {
                    row += '<td><img class="BTT-ajax-grid-row-image" onclick="editBTTTableRow(' + keyValue + ', \'' + tableName + '\')" src="../Home/../../Content/../Images/edit-metro-button.png"/></td>';
                }

                if (addDeleteColumn) {
                    row += '<td><img class="BTT-ajax-grid-row-image" onclick="deleteBTTTableRow(' + keyValue + ', \'' + tableName + '\')" src="../Home/../../Content/../Images/delete-metro-button.png"/></td>';
                }

                row += '</tr>';
                $(table).append(row);
            }
            $('#' + tableName).attr("totalrowscount", inputData.TotalRowsCount);
        }
    });
    
}

function editBTTTableRow(keyValue, gridId) {
    var url = $('#' + gridId + "EditUrl").text();

    $.ajax({
        cache: false,
        type: 'POST',
        data: { rowId: keyValue },
        url: url,
        success: function (data) {
            window.location.href = data.manageUrl;
        }
    });
}

function deleteBTTTableRow(keyValue, gridId) {
    var deleteInfoSpan = $('#' + gridId + "DeleteUrl");
    var text = $(deleteInfoSpan).attr("confirmtext");
    if (text == null || text.length <= 0 || text == "")
        text = "Удалить данную строку?";

    if (confirm(text)) {
        var url = $(deleteInfoSpan).text();
        $.ajax({
            cache: false,
            type: 'POST',
            data: { rowId: keyValue },
            url: url,
            success: function (inputData) {
                if (inputData != null && 0 < inputData.length)
                    alert(inputData);
                
                GetGridData(gridId);
            }
        });
    }
}

function countPerPageSelectionChanged(id, tableName, inputId) {
    var selectedClassName = "BTT-ajax-table-selected-item";
    $('#' + tableName + ' .' + selectedClassName).removeClass(selectedClassName);
    $('#' + id).addClass(selectedClassName);
    setInputValue(inputId, tableName, 1);
    GetGridData(tableName);
}

function normalizeInputValue(inputId, tableName) {
    var totalRowsCount = $('#' + tableName).attr("totalrowscount");
    var maxPagesCount = Math.ceil(totalRowsCount / GetRowsPerPage(tableName)); 
    var value = $('#' + inputId).val();

    if (parseInt(value) <= 0)
        $('#' + inputId).val(1);

    if (parseInt(maxPagesCount) < parseInt(value))
        $('#' + inputId).val(maxPagesCount);    
}

function inputValueChanged(id, tableName) {
    normalizeInputValue(id, tableName);
    GetGridData(tableName);
}

function setInputValue(inputId, tableName, value) {
    $('#' + inputId).val(value); 
    normalizeInputValue(inputId, tableName);
    GetGridData(tableName);
}

function setInputMaxValue(inputId, tableName) {
    var maxPagesCount = $('#' + tableName).attr("totalrowscount");
    $('#' + inputId).val(maxPagesCount); 
    normalizeInputValue(inputId, tableName);
    GetGridData(tableName);
}

function increaseOrDecreaseInputValue(inputId, tableName, increase) {
    var value = $('#' + inputId).val();
    var intValue = parseInt(value);

    if (increase)
        intValue++;
    else {
        intValue--;
    }
    $('#' + inputId).val(intValue);
    normalizeInputValue(inputId, tableName);
    GetGridData(tableName);
}

function orderByColumn(columnId, tableName) {
    var img = $('#' + columnId + " .BTT-ajax-table-sortable-img");
    if (img == null || img.length <= 0) {
        orderByColumnByIdentifier(columnId, tableName, true);
        return;
    }

    var attr = $(img).attr('sortindicator');
    if (attr == 'down') {
        orderByColumnByIdentifier(columnId, tableName, true);
    } else {
        orderByColumnByIdentifier(columnId, tableName, false);
    }
}

function orderByColumnByIdentifier(columnId, tableName, sortAsc) {
    var sortClass = 'BTT-ajax-table-sortable-img';
    $('#' + tableName + ' img').removeClass(sortClass).hide();
    
    if (sortAsc) {
        $('#' + columnId + ' [sortindicator="up"]').addClass(sortClass).show();
    } else {
        $('#' + columnId + ' [sortindicator="down"]').addClass(sortClass).show();
    }

    GetGridData(tableName);
}