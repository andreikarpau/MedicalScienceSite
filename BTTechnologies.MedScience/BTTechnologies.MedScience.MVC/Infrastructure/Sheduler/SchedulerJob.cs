using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Routing;
using BTTechnologies.MedScience.MVC.ControllersServices.Abstract;
using BTTechnologies.MedScience.MVC.Helpers;
using BTTechnologies.MedScience.MVC.Helpers.SitemapGenerator;
using Quartz;
using UrlHelper = System.Web.Mvc.UrlHelper;


namespace BTTechnologies.MedScience.MVC.Infrastructure.Sheduler
{
    public class SchedulerJob: IJob
    {
        private UrlHelper urlHelper;
        private readonly object lockObj = new object();

        public void Execute(IJobExecutionContext context)
        {
            lock (lockObj)
            {
                try
                {
                    var httpContext = HttpContext.Current;

                    if (httpContext == null)
                    {
                        var request = new HttpRequest("/", ConfigurationManagerHelper.SiteUrl, "");
                        var response = new HttpResponse(new StringWriter());
                        httpContext = new HttpContext(request, response);
                    }

                    var httpContextBase = new HttpContextWrapper(httpContext);
                    var routeData = new RouteData();
                    var requestContext = new RequestContext(httpContextBase, routeData);
                    urlHelper = new UrlHelper(requestContext);

                    ISupportService service = ServicesHelper.GetSupportService();
                    IList<ISitemapItem> sitemapItems = service.GetPublishedArticlesSiteMaps(GetArticleUrl, GetAttachmentUrl);

                    sitemapItems.Insert(0, new SitemapItem(urlHelper.QualifiedAction("Index", "Home"), DateTime.Now, SitemapChangeFrequency.Always, 1.0));
                    sitemapItems.Insert(0, new SitemapItem(urlHelper.QualifiedAction("ShowAllCategories", "ReadArticles"), changeFrequency: SitemapChangeFrequency.Monthly));
                    sitemapItems.Insert(0, new SitemapItem(urlHelper.QualifiedAction("ShowAllArticlesList", "ReadArticles"), changeFrequency: SitemapChangeFrequency.Monthly));

                    SitemapGenerator generator = new SitemapGenerator { PathToFullUrlFunc = GetAttachmentUrl };
                    generator.WriteSitemapToFile(sitemapItems);
                }
                catch (Exception ex)
                {
                    ExceptionsLogger.LogException(ex);
                }
                finally
                {
                    urlHelper = null;
                }
            }
        }


        private string GetArticleUrl(int articleId)
        {
            return urlHelper.QualifiedAction("ShowArticleView", "ReadArticles", new { id = articleId });
        }

        private string GetAttachmentUrl(string fileUrl)
        {
            string newPath = UrlHelper.GenerateContentUrl(fileUrl, urlHelper.RequestContext.HttpContext);
            string baseUri = ConfigurationManagerHelper.SiteUrl.TrimEnd('/');
            newPath = newPath.TrimStart('/');

            return string.Format("{0}/{1}", baseUri, newPath);
        }
    }
}