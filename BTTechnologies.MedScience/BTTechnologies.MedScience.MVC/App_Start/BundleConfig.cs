using System.Web.Optimization;

namespace BTTechnologies.MedScience.MVC.App_Start
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/DrowdownMenu").Include(
                        "~/Scripts/BTTDropdownMenu.js"));        
            
            bundles.Add(new ScriptBundle("~/bundles/SiteQuickSearch").Include(
                        "~/Scripts/SiteQuickSearchScripts.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/BTTAjaxGrid").Include(
                        "~/Scripts/BTTAjaxGridScripts.js"));

            bundles.Add(new ScriptBundle("~/bundles/ManageUserPermissions").Include(
                        "~/Scripts/ManageUserPermissions.js"));

            bundles.Add(new ScriptBundle("~/bundles/EditAuthorByAdmin").Include(
                        "~/Scripts/EditAuthorByAdmin.js"));

            bundles.Add(new ScriptBundle("~/bundles/BTTAjaxFileUploader").Include(
                        "~/Scripts/BTTAjaxFilesUploader.js"));

            bundles.Add(new ScriptBundle("~/bundles/PageTilesTableScripts").Include(
                        "~/Scripts/PageTilesTableScripts.js"));

            bundles.Add(new ScriptBundle("~/bundles/DropDown").Include(
                        "~/Scripts/hoverIntent.js", "~/Scripts/jquery.dropdown.js", "~/Scripts/jquery.dropdownPlain.js"));

            bundles.Add(new ScriptBundle("~/bundles/ManageArticleScripts").Include("~/Scripts/ManageArticleScripts.js", "~/Scripts/QuickAddDialogsScripts.js"));
            bundles.Add(new ScriptBundle("~/bundles/QuickSearchString").Include("~/Scripts/QuickSearchStringScript.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/site.css",
                "~/Content/BTTAjaxTableStyles.css",
                "~/Content/dropdownStyle.css", 
                "~/Content/QuickSearchStringStyles.css", 
                "~/Content/BTTDropDownMenuStyles.css",
                "~/Content/BTTAjaxFilesUploaderStyles.css"));

            bundles.Add(new StyleBundle("~/Content/siteSearchCss").Include(
                "~/Content/YandexSearchStyles.css",
                "~/Content/SiteQuickSearchPanelStyles.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css",
                        "~/Content/themes/base/jquery-ui.css"));
        }
    }
}