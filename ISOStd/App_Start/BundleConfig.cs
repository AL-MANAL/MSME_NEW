using System.Web.Optimization;

namespace ISOStd
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/ckeditor").Include(
                       "~/Scripts/ckeditor.js",
                         "~/Scripts/config.js",
                         "~/Scripts/styles.js"
                       ));
            bundles.Add(new ScriptBundle("~/bundles/ckeditor/adapters").Include(
                     "~/Scripts/ckeditor/adapters/jquery.js"
                     ));

            bundles.Add(new ScriptBundle("~/bundles/datatables").Include(
                         //"~/DataTables/datatables.min.js",
                         "~/DataTables/DataTables-1.10.18/js/jquery.dataTables.min.js",
                        "~/DataTables/DataTables-1.10.18/js/dataTables.jqueryui.min.js",
                        "~/DataTables/dataTables.responsive.min.js",
                           //"~/DataTables/DataTables-1.10.19/media/js/jquery.dataTables.min.js",
                           "~/DataTables/DataTables-1.10.19/extensions/Buttons/js/dataTables.buttons.min.js",
                             "~/DataTables/DataTables-1.10.19/extensions/Buttons/js/buttons.flash.min.js",
                                 "~/DataTables/DataTables-1.10.19/extensions/Buttons/js/buttons.html5.min.js",
                               "~/DataTables/DataTables-1.10.19/extensions/Buttons/js/buttons.print.min.js",
                                "~/DataTables/DataTables-1.10.19/extensions/ColReorder/js/dataTables.colReorder.min.js",
                                 "~/DataTables/DataTables-1.10.19/extensions/FixedHeader/js/dataTables.fixedHeader.min.js",
                                    "~/DataTables/DataTables-1.10.19/extensions/Buttons/js/buttons.colVis.min.js"

                       ));

            bundles.Add(new ScriptBundle("~/bundles/charts").Include(
                     "~/Charts/js/columns_waterfalls.js",
                       "~/Charts/js/echarts.min.js",
                          "~/Charts/js/app.js"
                     //"~/Charts/js/blockui.min.js"
                     //"~/Charts/js/bootstrap.bundle.min.js"
                     ));

            bundles.Add(new ScriptBundle("~/bundles/ajax").Include(
                      "~/Scripts/ajax/jszip.min.js",
                       "~/Scripts/ajax/pdfmake.min.js",
                        "~/Scripts/ajax/vfs_fonts.js"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/chosen").Include(
                      "~/Scripts/chosen.jquery.min.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));
            bundles.Add(new StyleBundle("~/Content/popover").Include("~/Content/popover/popover.css"));
            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));
            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/editor.css"));
            bundles.Add(new StyleBundle("~/Content/choosen").Include("~/Content/choosen/chosen.min.css"));
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
                        "~/Content/themes/base/jquery.ui.theme.css"));

            bundles.Add(new ScriptBundle("~/bundles/datetime").Include(
                    "~/Scripts/moment*",
                    "~/Scripts/bootstrap-datetimepicker*"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      //"~/DataTables/datatables.min.css",
                      //"~/DataTables/DataTables-1.10.18/css/dataTables.bootstrap.min.css",
                      //"~/DataTables/DataTables-1.10.18/css/dataTables.bootstrap4.min.css",
                      //"~/DataTables/DataTables-1.10.18/css/dataTables.foundation.min.css",
                      "~/DataTables/DataTables-1.10.18/css/dataTables.jqueryui.min.css",
                       //"~/DataTables/DataTables-1.10.18/css/dataTables.semanticui.min.css",
                       "~/DataTables/DataTables-1.10.18/css/jquery.dataTables.min.css",
                       "~/DataTables/responsive.dataTables.min.css",
                         //"~/DataTables/DataTables-1.10.19/media/css/jquery.dataTables.min.css",
                         "~/DataTables/DataTables-1.10.19/extensions/Buttons/css/buttons.dataTables.min.css",
                          "~/DataTables/DataTables-1.10.19/extensions/ColReorder/css/colReorder.dataTables.min.css",
                          "~/DataTables/DataTables-1.10.19/extensions/FixedHeader/css/fixedHeader.dataTables.min.css",
                                //"~/Charts/css/bootstrap.min.css",
                                //"~/Charts/css/bootstrap_limitless.min.css",
                                "~/Charts/css/colors.min.css",
                                   "~/Charts/css/components.css"
                          //"~/Charts/css/components.min.css"

                          ));
        }
    }
}