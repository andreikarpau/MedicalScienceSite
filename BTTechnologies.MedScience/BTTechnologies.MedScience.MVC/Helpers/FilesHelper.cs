using System;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace BTTechnologies.MedScience.MVC.Helpers
{
    public static class FilesHelper
    {
        public static string GetUploadFileDirectoryName(string baseArticlesPath, string articleName)
        {
            return Path.Combine(baseArticlesPath, articleName);
        }

        public static bool DeleteFile(string fileUrl, string baseDocumentsUploadDirectory)
        {
            try
            {
                string path = GetFilePathByUrl(baseDocumentsUploadDirectory, fileUrl);

                if (!File.Exists(path))
                    return true;

                try
                {
                    File.SetAttributes(path, FileAttributes.Normal);
                }
                catch (Exception e)
                {
                    ExceptionsLogger.LogException(e);
                }

                File.Delete(path);
                return true;
            }
            catch (Exception e)
            {
                ExceptionsLogger.LogException(e);
                return false;
            }
        }

        public static string GetFileUrlByBasePath(string fullBasePath, string filePath)
        {
            string[] splitedStrings = fullBasePath.Split('\\');
            return GetFileUrlByPath(fullBasePath.Replace(splitedStrings[splitedStrings.Length - 1], string.Empty), filePath);
        }
        
        public static string GetFileUrlByPath(string baseDirectoryPath, string path)
        {
            string newUrl = path;

            if (string.IsNullOrEmpty(newUrl))
                return string.Empty;

            newUrl = newUrl.ToLowerInvariant().Replace(baseDirectoryPath.ToLowerInvariant(), "~/");

            while (newUrl.Contains("//") || newUrl.Contains("\\"))
            {
                newUrl = newUrl.Replace("\\", "/");
                newUrl = newUrl.Replace("//", "/");
            }
            
            return newUrl;
        }

        public static string GetFilePathByUrl(string baseDirectoryPath, string url)
        {
            string newPath = url;

            if (string.IsNullOrEmpty(newPath))
                return string.Empty;

            string[] splitedStrings = baseDirectoryPath.Split('\\');
            string basePath = baseDirectoryPath.Replace(splitedStrings[splitedStrings.Length - 1], string.Empty);

            newPath = newPath.Replace("~/", basePath);

            while (newPath.Contains("/"))
            {
                newPath = newPath.Replace("/", "\\");
            }

            return newPath;
        }

        public static string GetContentFullPath(UrlHelper url, string virtualPath)
        {
            Uri requestUrl = url.RequestContext.HttpContext.Request.Url;
            return string.Format("{0}://{1}{2}", requestUrl.Scheme, requestUrl.Authority, VirtualPathUtility.ToAbsolute(virtualPath));
        }
    }
}