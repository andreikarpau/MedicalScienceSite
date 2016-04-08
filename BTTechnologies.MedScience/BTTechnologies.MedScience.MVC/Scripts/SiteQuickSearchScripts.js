function ClearSearch() {
    $('#QuickSearchResultsTopDiv').slideUp();
    $('#QuickSearchPanel').attr('itemscount', 0);
}

function StartSearch(currentVal) {
    if (currentVal == '') {
        ClearSearch();
        return;
    }

    var dataUrl = $('#QuickSearchPanel').attr('quicksearchurl');

    var data = { MaxItemsCount: maxItemsCount, Pattern: currentVal };
    data = JSON.stringify(data);


    $.ajax({
        cache: false,
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        data: data,
        url: dataUrl,
        success: function (inputData) {
            if (inputData.length > 0) {
                var resultsUl = $('ul#QuickSearchPanelResultsList');
                $(resultsUl).empty();

                for (var i = 0; i < inputData.length; i++) {
                    var newLi = '<li itemIndex="' + i + '">';
                    newLi += '<a href="' + inputData[i].ItemUrl + '">' + inputData[i].ItemName + '</a>';
                    newLi += '</li>';

                    $(resultsUl).append(newLi);
                }
            }

            $('#QuickSearchPanel').attr('itemscount', inputData.length);
            $('#QuickSearchPanelInProgressImg').hide();

            ShowPageByNum(0);

            if (inputData.length <= 0) {
                $('#QuickSearchResultsTopDiv').slideUp();
            } else {
                $('#QuickSearchResultsTopDiv').slideDown();
                RecountElementPosition();
            }
        },
        error: function () {
            $('#QuickSearchPanelInProgressImg').hide();
        }
    });
}

function ShowPageByNum(pageNum) {
    $('#QuickSearchPanelResultsList li').hide();

    var firstItemNum = pageNum * itemsPerPage;
    var lastItemNum = (pageNum + 1) * itemsPerPage - 1;
    for (var i = firstItemNum; i <= lastItemNum; i++) {
        var li = $('#QuickSearchPanelResultsList li[itemIndex="' + i + '"]');
        if (li.length <= 0)
            continue;

        $(li).show();
    }

    $('#QuickSearchPanel').attr('currentPage', pageNum);

    if (NextPageExists()) {
        $('#QuickSearchPanelNextButton').show();
    } else {
        $('#QuickSearchPanelNextButton').hide();
    }

    if (PrevPageExists()) {
        $('#QuickSearchPanelPrevButton').show();
    } else {
        $('#QuickSearchPanelPrevButton').hide();
    }
}

function RecountElementPosition() {
    var height = $('#QuickSearchPanelTitleDiv').height() + $('#QuickSearchPanelResultsListContainerSpan').height() + bottomMargin;
    var currentX = $('#QuickSearchPanel').position().left;

    var yandexBarY = $('#SiteFullSearchContainer').position().top;

    var newY = yandexBarY - height;
    $("#QuickSearchPanel").css({ top: newY, left: currentX });
}

function ShowNextPage() {
    var currentPage = parseInt($('#QuickSearchPanel').attr('currentPage'));
    ShowPageByNum(currentPage + 1);
    return true;
}

function ShowPrevPage() {
    var currentPage = parseInt($('#QuickSearchPanel').attr('currentPage'));
    ShowPageByNum(currentPage - 1);
    return true;
}

function NextPageExists() {
    var page = parseInt($('#QuickSearchPanel').attr('currentPage'));
    var maxShownItem = (page + 1) * itemsPerPage - 1;
    var totalItemsCount = parseInt($('#QuickSearchPanel').attr('itemscount'));

    if (maxShownItem < (totalItemsCount - 1))
        return true;

    return false;
}

function PrevPageExists() {
    var page = parseInt($('#QuickSearchPanel').attr('currentPage'));
    if (page <= 0)
        return false;

    return true;
}

function StartSearchIfNeeded(index) {
    var currentIndex = parseInt($('#QuickSearchPanel').attr('searchindex'));

    if (currentIndex != index)
        return;

    var currentVal = $(inputSelector).val().trim();

    quickSearchIsInProgress = true;
    
    if (currentVal == '') {
        ClearSearch();
    } else {
        StartSearch(currentVal);
    }

    quickSearchIsInProgress = false;
}

var maxItemsCount = 30;
var itemsPerPage = 3;
var delay = 1000;
var quickSearchIsInProgress = false;
var inputSelector = 'input.ya-site-form__input-text';
var bottomMargin = 10;

$('div.ya-site-form').bind("DOMSubtreeModified", function () {
    var input = $(inputSelector);

//    $('#QuickSearchResultsTopDiv').slideUp();

    if (input.length > 0 && $('#QuickSearchPanel').attr('inputkeyuphandled') != 'true') {
        $('#QuickSearchPanel').attr('inputkeyuphandled', 'true');
        $('#QuickSearchPanel').attr('searchindex', '0');

        $(input).keyup(function () {
            if (quickSearchIsInProgress || $(this).val() == '') {
                $('#QuickSearchPanelInProgressImg').hide();
                return;
            }

            $('#QuickSearchPanel').show();
            $('#QuickSearchResultsTopDiv').hide();
            RecountElementPosition();

            $('#QuickSearchResultsTopDiv').slideUp();
            $('#QuickSearchPanelInProgressImg').show();

            var index = parseInt($('#QuickSearchPanel').attr('searchindex'));
            index++;
            
            $('#QuickSearchPanel').attr('searchindex', index);

            $('#QuickSearchPanel').attr('inputkeyuphandled', 'true');
            setTimeout(function () { StartSearchIfNeeded(index); }, delay);

        });
    }
});

$(document).mousedown(function (e) {
    var container = $("input.ya-site-form__input-text");

    if (container.is(e.target) || container.has(e.target).length != 0)
        return true;

    var panel = $("#QuickSearchPanel");

    if (panel.is(e.target) || panel.has(e.target).length != 0)
        return true;

    $('#QuickSearchPanel').hide();

    return true;
});

