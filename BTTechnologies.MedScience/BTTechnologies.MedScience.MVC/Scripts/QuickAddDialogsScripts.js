function ValidateAndAddNewAuthorData() {
    var errorsFound = false;

    $('#AuthorDialogErrors').hide();
    var surname = $('#AuthorSurname').val();

    if (surname == null || surname == "" || surname.split() == "") {
        $('#AuthorSurnameValidation').show();
        errorsFound = true;
    } else {
        $('#AuthorSurnameValidation').hide();
    }

    var name = $('#AuthorName').val();

    if (name == null || name == "" || name.split() == "") {
        $('#AuthorNameValidation').show();
        errorsFound = true;
    } else {
        $('#AuthorNameValidation').hide();
    }

    if (errorsFound)
        return;

    var patronymic = $('#AuthorPatronymic').val();
    var degree = $('#AuthorDegree').val();
    var url = $('#QuickAddAuthorBox').attr('quickauthoraddurl');
    
    var data = { Name: name, Surname: surname, Patronymic: patronymic, Degree: degree };
    data = JSON.stringify(data);

    $.ajax({
        cache: false,
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        data: data,
        url: url,
        success: function (inputData) {
            if (inputData.newId <= 0) {
                $('#AuthorDialogErrors').text(inputData.errorDescription);
                $('#AuthorDialogErrors').show();
                return;
            }

            NewAuthorAdded(inputData.newId);
        }
    });
}

function ValidateAndAddNewCategoryData() {
    var errorsFound = false;

    $('#CategoryDialogErrors').hide();
    var title = $('#CategoryTitle').val();

    if (title == null || title == "" || title.split() == "") {
        $('#CategoryTitleValidation').show();
        errorsFound = true;
    } else {
        $('#CategoryTitleValidation').hide();
    }

    if (errorsFound)
        return;

    var description = $('#CategoryDescription').val();
    var url = $('#QuickAddCategoryBox').attr('quickcategoryaddurl');
    
    var data = { DisplayName: title, CategoryDescription: description };
    data = JSON.stringify(data);

    $.ajax({
        cache: false,
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        data: data,
        url: url,
        success: function (inputData) {
            if (inputData.newId <= 0) {
                $('#CategoryDialogErrors').text(inputData.errorDescription);
                $('#CategoryDialogErrors').show();
                return;
            }

            NewCategoryAdded(inputData.newId);
        }
    });
}