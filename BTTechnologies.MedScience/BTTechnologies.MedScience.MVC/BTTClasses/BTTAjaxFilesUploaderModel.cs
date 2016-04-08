using System;
using System.Collections.Generic;
using System.Web;

namespace BTTechnologies.MedScience.MVC.BTTClasses
{
    public class BTTAjaxFilesUploaderModel
    {
        public string UploadUrl { get; set; }
        public string UploadText { get; set; }
        public string UploadErrorText { get; set; }

        public BTTAjaxFilesUploaderModel(string uploadUrl)
        {
            UploadErrorText = "Error occured";
            UploadText = "Upload";
            UploadUrl = uploadUrl;
        }
    }

    public class UploadFilesInputModel
    {
        public IDictionary<string, HttpPostedFileBase> PostedFiles { get; private set; }

        public UploadFilesInputModel()
        {
            PostedFiles = new Dictionary<string, HttpPostedFileBase>();
        }
    }

    public class UploadFilesOutputModel
    {
        public IList<UploadedFileInfo> UploadedFileInfos { get; private set; }

        public UploadFilesOutputModel()
        {
            UploadedFileInfos = new List<UploadedFileInfo>();
        }

        public class UploadedFileInfo
        {
            public string FileName { get; set; }
            public Guid FileGuid { get; set; }

            public UploadedFileInfo(Guid guid, string name)
            {
                FileName = name;
                FileGuid = guid;
            }
        }
    }
}