using System.Collections.Generic;
using System.Xml.Linq;

namespace BTTechnologies.MedScience.MVC.Helpers.SitemapGenerator
{
    public interface ISitemapGenerator
    {
        XDocument GenerateSiteMapDocument(IEnumerable<ISitemapItem> items);
    }
}
