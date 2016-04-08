using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using BTTechnologies.MedScience.Domain.Entities;
using BTTechnologies.MedScience.MVC.Models;

namespace BTTechnologies.MedScience.MVC.Helpers
{
    public static class HTMLHelper
    {
        private static bool showMainContentContainerStyle = true;
        
        public static bool ShowMainContentContainerStyle
        {
            get
            {
                if (showMainContentContainerStyle) 
                    return true;
                
                showMainContentContainerStyle = true;
                return false;
            }
        }

        public static void HideMainContentContainerStyle()
        {
            showMainContentContainerStyle = false;
        }

        public static MvcHtmlString GetLastModifiedMeta(this HtmlHelper helper, DateTime dateTime)
        {
            string meta = "<meta http-equiv=\"last-modified\" content=\"" + dateTime.ToString("R") + "\">";
            return new MvcHtmlString(meta);
        }    
        
        public static MvcHtmlString GetTitleMeta(this HtmlHelper helper, string pageTitle)
        {
            string meta = "<title>" + pageTitle + "</title>";
            return new MvcHtmlString(meta);
        }   
          
        public static MvcHtmlString GetDescriptionMeta(this HtmlHelper helper, string pageDescription)
        {
            string meta = "<meta name=\"description\" content=\"" + pageDescription + "\">";
            return new MvcHtmlString(meta);
        }
          
        public static MvcHtmlString GetKeyWordsMeta(this HtmlHelper helper, string pageKeyWords)
        {
            string meta = "<meta name=\"keywords\" content=\"" + pageKeyWords + "\">";
            return new MvcHtmlString(meta);
        }

        public static string GenerateKeyWordsForArticle(this HtmlHelper helper, IList<CategoryModel> articleCategories, IList<AttachmentsModel> articleAttachments)
        {
            string resultString = string.Empty;

            try
            {
                if (articleCategories != null)
                {
                    foreach (string result in articleCategories.Select(c => c.CategoryDisplayName))
                    {
                        if (!string.IsNullOrEmpty(resultString)&& !string.IsNullOrEmpty(result))
                            resultString += ", ";

                        resultString += result;
                    }
                }

                if (articleAttachments != null)
                {
                    foreach (string fileName in articleAttachments.Select(a => Path.GetFileNameWithoutExtension(a.GetFullFileName())))
                    {
                        if (!string.IsNullOrEmpty(resultString) && !string.IsNullOrEmpty(fileName))
                            resultString += ", ";

                        resultString += fileName;
                    }
                }
            }
            catch (Exception e)
            {
                ExceptionsLogger.LogException(e);
            }

            return resultString;
        }

        public static SelectList GetViewTypesSelectList(this HtmlHelper helper, Type enumType, object selectedObject)
        {
            return new SelectList(Enum.GetNames(enumType), selectedObject);
        }

        public static string NumberBox(this HtmlHelper htmlHelper, string name)
        {
            return NumberBox(htmlHelper, name, null /* value */);
        }

        public static string NumberBox(this HtmlHelper htmlHelper, string name, object value)
        {
            return NumberBox(htmlHelper, name, value, (object)null /* htmlAttributes */);
        }

        public static string NumberBox(this HtmlHelper htmlHelper, string name, object value, object htmlAttributes)
        {
            return NumberBox(htmlHelper, name, value, new RouteValueDictionary(htmlAttributes));
        }

        public static string NumberBox(this HtmlHelper htmlHelper, string name, object value, IDictionary<string, object> htmlAttributes)
        {
            return InputHelper(htmlHelper, "Number", name, value, (value == null) /* useViewData */, true /* setId */, true /* isExplicitValue */, htmlAttributes);
        }

        public static MvcHtmlString GetTilesTable(this HtmlHelper htmlHelper, IEnumerable<PageTile> tiles)
        {
            string table = "<table class=\"tiles-table\">";
            
            const int maxElementsPerRow = 3;
            int rowElementsCount = 0;

            table += "<tr>";

            foreach (PageTile tile in tiles.OrderBy(t => t.ItemOrder))
            {
                int size = 1;

                if (tile.TileStyles.Contains("middle-content-container"))
                    size = 2;
                if (tile.TileStyles.Contains("big-content-container"))
                    size = 3;

                if (maxElementsPerRow < (rowElementsCount + size))
                {
                    int colSpan = rowElementsCount - maxElementsPerRow;
                    if (0 < colSpan)
                    {
                        table += "<td colspan=\"" + colSpan + "\" />";
                    }
                    
                    table += "</tr>";
                    table += "<tr>";
                    rowElementsCount = 0;
                }

                rowElementsCount += size;

                string content = htmlHelper.Partial("_PageTile", tile).ToString();
                table += "<td class=\"content-border\" colspan=\"" + size + "\"><div class=\"content-tile-container " + tile.TileStyles + "\">" + content + "</div></td>";
            }

            table += "</tr>";
            table += "</table>";

            return new MvcHtmlString(table);
        }

        private static string InputHelper(this HtmlHelper htmlHelper, string inputType, string name, object value, bool useViewData, bool setId, bool isExplicitValue, IDictionary<string, object> htmlAttributes)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name parameter can't be null or empty", "name");
            }

            TagBuilder tagBuilder = new TagBuilder("input");
            tagBuilder.MergeAttributes(htmlAttributes);
            tagBuilder.MergeAttribute("type", inputType);
            tagBuilder.MergeAttribute("name", name, true);

            string valueParameter = Convert.ToString(value, CultureInfo.CurrentCulture);

            string attemptedValue = null;
            ModelState modelState = null;
            if (htmlHelper.ViewData.ModelState.TryGetValue(name, out modelState))
            {
                attemptedValue = (string)modelState.Value.ConvertTo(typeof(string), null /* culture */);
            }


            tagBuilder.MergeAttribute("value", attemptedValue ?? ((useViewData) ? Convert.ToString(htmlHelper.ViewData.Eval(name), CultureInfo.CurrentCulture) : valueParameter), isExplicitValue);

            if (setId)
            {
                tagBuilder.GenerateId(name);
            }

            // If there are any errors for a named field, we add the css attribute.
            if (modelState != null)
            {
                if (modelState.Errors.Count > 0)
                {
                    tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
                }
            }

            return tagBuilder.ToString(TagRenderMode.SelfClosing);
        }
    }
}