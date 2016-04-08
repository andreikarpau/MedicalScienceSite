using System;

namespace BTTechnologies.MedScience.MVC.Helpers.SitemapGenerator
{
    /// <summary>
    /// How frequently the page is likely to change. This value provides general information to search engines and may not correlate exactly to how often they crawl the page.
    /// </summary>
    /// <remarks>
    /// The value "always" should be used to describe documents that change each time they are accessed. The value "never" should be used to describe archived URLs.
    /// </remarks>
    public enum SitemapChangeFrequency
    {
        Always,
        Hourly,
        Daily,
        Weekly,
        Monthly,
        Yearly,
        Never
    }

    /// <summary>
    /// An interface for sitemap items
    /// </summary>
    public interface ISitemapItem
    {
        /// <summary>
        /// URL of the page.
        /// </summary>
        string Url { get; }

        /// <summary>
        /// The date of last modification of the file.
        /// </summary>
        DateTime? LastModified { get; }

        /// <summary>
        /// How frequently the page is likely to change.
        /// </summary>
        SitemapChangeFrequency? ChangeFrequency { get; }

        /// <summary>
        /// The priority of this URL relative to other URLs on your site. Valid values range from 0.0 to 1.0.
        /// </summary>
        double? Priority { get; }
    }
}
