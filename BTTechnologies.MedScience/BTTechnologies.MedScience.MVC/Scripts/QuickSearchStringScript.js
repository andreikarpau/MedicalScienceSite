function SendAjaxToUrl(input) {
    var obj = $(input).parent();

    if ($(obj).attr('wassent') == 'true')
        return;

    var value = $(input).val();

    if (value == $(obj).attr('oldvalue'))
        return;

    $(obj).attr('oldvalue', value);

    var data = { CurrentValue: value };
    data = JSON.stringify(data);

    var getDataUrl = $(obj).attr('getdataurl');

    $(obj).attr('wassent', 'true');
    $.ajax({
        cache: false,
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        data: data,
        url: getDataUrl,
        success: function (inputData) {
            var ol = $(obj).find('ol');
            $(ol).empty();

            if (inputData.SearchResults.length <= 0) {
                var nothingFoundText = $(obj).attr('nothingfound');
                var nothingFoundClickable = $(obj).attr('nothingfoundclickable');
                var spanText = '<span';
                if (nothingFoundClickable == 'true') {
                    spanText += ' onclick="nothingFoundClicked(\'' + value + '\', this)" class="a-span"';
                }
                spanText += '>' + nothingFoundText + '</span>';
                var emptyList = '<li>' + spanText + '</li>';
                $(ol).append(emptyList);
            } else {
                for (var i = 0; i < inputData.SearchResults.length; i++) {
                    var itemUrl = '';
                    if (inputData.SearchResults[i].Url != null && 0 < inputData.SearchResults[i].Url.length)
                        itemUrl = 'itemUrl = "' + inputData.SearchResults[i].Url + '"';

                    var itemId = '';
                    if (inputData.SearchResults[i].Id != null)
                        itemId = 'itemid="' + inputData.SearchResults[i].Id + '"';

                    var description  = '';
                    if (inputData.SearchResults[i].Description != null && 0 < inputData.SearchResults[i].Description.length)
                        description = inputData.SearchResults[i].Description;
                    
                    var list = '<li>';
                    list += '<span ' + itemUrl + ' ' + itemId + ' onclick="quickSearchItemClicked(' + inputData.SearchResults[i].Id + ', this)"';
                    list += ' class="search-item-title a-span">' + inputData.SearchResults[i].Title + '</span>';
                    list += '<span class="search-item-description">' + description + '</span>';
                    list += '</li>';
                    $(ol).append(list);
                }
            }

            var resultsContainer = $(obj).children('div');
            if ($(resultsContainer).is(':hidden'))
                $(resultsContainer).slideDown();

            $(obj).attr('wassent', 'false');
        }
    });
}

function InitQuickSearches() {
    $('.btt-quick-search-string').each(function () {
        var input = $(this).children('input');
        var resultsContainer = $(this).children('div');

        $(resultsContainer).slideUp();
        $(input).keyup(function () { SendAjaxToUrl(this); });
        $(input).focusout(function () {
            var obj = $(input).parent();
            var initText = $(obj).attr('inittext');
            $(input).val(initText);
            $(obj).attr('oldvalue', '');

            $(input).addClass('italic-gray-text');
            setTimeout(function () { $(resultsContainer).slideUp(); }, 200);
        });

        $(input).focus(function () {
            $(input).val('');
            $(input).removeClass('italic-gray-text');
        });
    });
}

InitQuickSearches();