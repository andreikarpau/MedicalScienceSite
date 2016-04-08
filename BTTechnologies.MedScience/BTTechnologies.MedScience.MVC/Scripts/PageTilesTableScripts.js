var newMaxItemsCount = 5;

function FillNewOnSiteContainers() {
    var containers = $('div.new-on-site-content');
    if (containers.length <= 0)
        return;

    var url = $($(containers).first()).attr("newsurl");
    var articleUrl = $($(containers).first()).attr("articleurl");
    
    $.ajax({
        cache: false,
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify({ count: newMaxItemsCount }),
        url: url,
        success: function (inputData) {
            var htmlData = "";

            for (var i = 0; i < inputData.length; i++) {
                htmlData += '<a href="' + articleUrl + inputData[i].Id + '">' + inputData[i].DisplayName + '</a>';
                htmlData += '<div class="all-articles-small-description">' + inputData[i].DocumentDescription + '</div>';
                htmlData += '<div class="new-on-site-divider"/>';
            }

            for (var j = 0; j < containers.length; j++) {
                $(containers[j]).html(htmlData);
            }
        }
    });
}

FillNewOnSiteContainers();