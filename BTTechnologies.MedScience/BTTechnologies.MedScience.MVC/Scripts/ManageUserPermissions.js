function RefreshAllPermissions() {
    var item = $('#DropDownWithFieldDiv');
    var userId = $(item).attr("userid");
    var refreshUrl = $(item).attr("refreshurl");
    var canEdit = $(item).attr("canedit");

    $.ajax({
        cache: false,
        type: 'POST',
        data: { userid: userId },
        url: refreshUrl,
        success: function (inputData) {
            var userRolesItem = $('#UserRoles');
            $(userRolesItem).empty();

            for (var i = 0; i < inputData.currentRoles.length; i++) {
                var newDiv = '<div>' + inputData.currentRoles[i];

                if (canEdit.toLowerCase() == "true") {
                    newDiv += '<img src="../Home/../../Content/../Images/delete-button-red.png" onclick="DeletePermission(' + inputData.currentRolesId[i] + ')"/>';
                }

                newDiv += '</div>';
                $(userRolesItem).append(newDiv);
            }

            if (canEdit.toLowerCase() == "true") {
                var allRoles = $('#DropDownWithFieldDiv .sub_menu');
                $(allRoles).empty();

                for (var index = 0; index < inputData.rolesCanBeSelected.length; index++) {
                    var newLi = '<li><div onclick="AddPermission(' + inputData.rolesCanBeSelectedId[index] + ')">' + inputData.rolesCanBeSelected[index] + '</div></li>';
                    $(allRoles).append(newLi);
                }
            }
        }
    });
}

function DeletePermission(permissionId) {
    var item = $('#DropDownWithFieldDiv');
    var userId = $(item).attr("userid");
    var deleteUrl = $(item).attr("deleteurl");

    $.ajax({
        cache: false,
        type: 'POST',
        data: { userId: userId, permissionId: permissionId },
        url: deleteUrl,
        success: function (error) {
            if (error != "") {
                alert(error);
            }

            RefreshAllPermissions();
        }
    });
}

function AddPermission(permissionId) {
    var item = $('#DropDownWithFieldDiv');
    var userId = $(item).attr("userid");
    var addUrl = $(item).attr("addurl");

    $.ajax({
        cache: false,
        type: 'POST',
        data: { userId: userId, permissionId: permissionId },
        url: addUrl,
        success: function (error) {
            if (error != "") {
                alert(error);
            }
                

            RefreshAllPermissions();
        }
    });
}

RefreshAllPermissions();
