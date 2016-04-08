using System;
using System.Collections.Generic;
using BTTechnologies.MedScience.MVC.Helpers.SitemapGenerator;

namespace BTTechnologies.MedScience.MVC.ControllersServices.Abstract
{
    public interface ISupportService
    {
        IList<ISitemapItem> GetPublishedArticlesSiteMaps(Func<int, string> getArticleUrl, Func<string, string> getAttachmentsUrl);
    }
}
