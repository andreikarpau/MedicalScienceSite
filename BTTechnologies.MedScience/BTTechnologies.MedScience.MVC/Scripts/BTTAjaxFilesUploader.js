function progressHandlingFunction(evt) {
    if (evt.lengthComputable) {  
        var percentComplete = (evt.loaded / evt.total) * 100;
        $('#BTTAjaxFilesUploaderContainer .percentage-element').css("width", percentComplete + "%");
    } 
}

function DeleteUploadedFile(containerId) {
    $('#' + containerId).remove();
}

$('#BTTFileUpload').change(function () {
    $('#BTTAjaxFilesUploaderContainer .message-error').hide();

    var value = $(this).val();

    if (value == null || value == "") {
        $('#BTTFileUploadSubmit').prop("disabled", true);
    }
    else {
        $('#BTTFileUploadSubmit').prop("disabled", false);
    }
});

var fileNameNum = 1;

$('button#BTTFileUploadSubmit').click(function () {
    var fileInput = document.getElementById('BTTFileUpload');
    $(fileInput).prop("disabled", true);

    var formData = new FormData();

    for (i = 0; i < fileInput.files.length; i++) {
        formData.append(fileInput.files[i].name, fileInput.files[i]);
    }

    var url = $('#BTTAjaxFilesUploaderContainer').attr('uploadurl');

    $(fileInput).val("");
    $('#BTTFileUploadSubmit').prop("disabled", true);

    $.ajax({
        url: url,
        type: 'POST',
        data: formData,
        //Options to tell jQuery not to process data or worry about content-type.
        cache: false,
        contentType: false,
        processData: false,

        // Custom XMLHttpRequest
        xhr: function () {
            var myXhr = $.ajaxSettings.xhr();
            if (myXhr.upload) { // Check if upload property exists
                myXhr.upload.addEventListener('progress', progressHandlingFunction, false); // For handling the progress of the upload
            }
            return myXhr;
        },

        success: function (inputData) {
            if (inputData == null || inputData == "")
                return;

            for (var i = 0; i < inputData.UploadedFileInfos.length; i++) {
                var fileName = inputData.UploadedFileInfos[i].FileName;
                var guid = inputData.UploadedFileInfos[i].FileGuid;

                var elementId = 'BTTUploaderLoadedFileContainer' + fileNameNum;
                var element = '<div id="' + elementId + '">';
                element += '<span>' + fileName + '</span>';
                element += '<img class="deleteImg" src="../Home/../../Content/../Images/delete-button-red.png" onclick="DeleteUploadedFile(\'' + elementId + '\')" alt="Delete"/>';
                element += '<input type="hidden" class="" name="UploadedFilesGuids" id="UploadedFileGuid' + fileNameNum + '" value="' + guid + '"/></div>';
                $('#BTTUploaderNewFilesInputs').prepend(element);

                fileNameNum++;
            }

            $('#BTTAjaxFilesUploaderContainer .progress-bar').hide();
            $('#BTTFileUpload').prop("disabled", false);
        },

        beforeSend: function () {
            $('#BTTAjaxFilesUploaderContainer .percentage-element').css("width", 0 + "%");
            $('#BTTAjaxFilesUploaderContainer .progress-bar').show();
        },
        error: function () {
            $('#BTTAjaxFilesUploaderContainer .message-error').show();
            $('#BTTAjaxFilesUploaderContainer .progress-bar').hide();
            $('#BTTFileUpload').prop("disabled", false);
        }
    });

    return false;
});
