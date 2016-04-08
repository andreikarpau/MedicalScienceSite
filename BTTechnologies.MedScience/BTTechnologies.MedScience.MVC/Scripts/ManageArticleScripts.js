var fileNameNum = 1;

function DeleteCategory(id) {
    var li = $('#CategoriesList li[categoryid="' + id + '"]');

    if ($(li).length <= 0)
        return;

    $(li).remove();
}

function DeleteAuthor(id) {
    var li = $('#AuthorsList li[authorid="' + id + '"]');

    if ($(li).length <= 0)
        return;

    $(li).remove();
}

function DeleteDocument(id) {
    var li = $('#DocumentsList li[documentid="' + id + '"]');

    if ($(li).length <= 0)
        return;

    $(li).remove();
}

function AddAuthorToList(id) {
    var getDataUrl = $("#AuthorsList").attr('fullnameurl');

    if (0 < $('#AuthorsList li[authorid="' + id + '"]').length)
        return;
    
    $.ajax({
        cache: false,
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify({id: id }),
        url: getDataUrl,
        success: function (inputData) {
            var authorName = inputData.fullName;
            var newLi = '<li authorid="' + id + '"><a href="' + inputData.url + '">' + authorName + '</a>';
            newLi += '<img class="deleteImg" src="../Home/../../Content/../Images/delete-button-red.png" onclick="DeleteAuthor(' + id + ')"/>';
            newLi += '<input type="hidden" name="AuthorsIds" id="author' + id + '" value="' + id + '" /></li>';
            $('#AuthorsList').append(newLi);
        }
    });
}

function AddCategoryToList(id) {
    var getDataUrl = $("#CategoriesList").attr('fullnameurl');

    if (0 < $('#CategoriesList li[categoryid="' + id + '"]').length)
        return;
    
    $.ajax({
        cache: false,
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify({id: id }),
        url: getDataUrl,
        success: function (inputData) {
            var categoryName = inputData.DisplayName;
            var newLi = '<li categoryid="' + id + '"><span>' + categoryName + '</span>';
            newLi += '<img class="deleteImg" src="../Home/../../Content/../Images/delete-button-red.png" onclick="DeleteCategory(' + id + ')"/>';
            newLi += '<input type="hidden" name="CategoriesIds" id="category' + id + '" value="' + id + '" /></li>';
            $('#CategoriesList').append(newLi);
        }
    });
}

function quickSearchItemClicked(id, obj) {
    if ($(obj).parents('#authorsLi').length) {
        AddAuthorToList(id);
        return;
    }

    if ($(obj).parents('#categoriesLi').length) {
        AddCategoryToList(id);
    }
}

function showAddAuthorDialog(enteredValue) {
    var box = $('#QuickAddAuthorBox');

    $('#AuthorName').val('');
    $('#AuthorPatronymic').val('');
    $('#AuthorDegree').val('');
    $('#AuthorSurname').val(enteredValue);
    $(box).show();
    $(box).dialog({
        resizable: false,
        modal: true
    });
    return;
}

function showCategoryDialog(enteredValue) {
    var box = $('#QuickAddCategoryBox');

    $('#CategoryTitle').val(enteredValue);
    $('#CategoryDescription').val('');
    $(box).show();
    $(box).dialog({
        resizable: false,
        modal: true
    });
    return;
}

function nothingFoundClicked(enteredValue, obj) {

    if ($(obj).parents('#authorsLi').length) {
        showAddAuthorDialog(enteredValue);
        return;
    }

    if ($(obj).parents('#categoriesLi').length) {
        showCategoryDialog(enteredValue);
    }
}

function NewAuthorAdded(newId) {
    $('#QuickAddAuthorBox').dialog('close');
    AddAuthorToList(newId);
}

function NewCategoryAdded(newId) {
    $('#QuickAddCategoryBox').dialog('close');
    AddCategoryToList(newId);
}