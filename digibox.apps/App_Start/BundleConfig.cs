using System.Web;
using System.Web.Optimization;

namespace digibox.apps
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Content/plugins/jquery-ui/jquery-ui.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/digibox").Include(
                        "~/Content/plugins/bootstrap/js/bootstrap.bundle.min.js",
                        "~/Content/plugins/toastr/toastr.min.js",
                        "~/Content/plugins/moment/moment.min.js",
                        "~/Content/plugins/tempusdominus-bootstrap-4/js/tempusdominus-bootstrap-4.min.js",
                        "~/Content/plugins/sweetalert2/sweetalert2.min.js",
                        "~/Content/plugins/select2/js/select2.full.min.js",
                        "~/Content/plugins/bootstrap4-editable/js/bootstrap-editable.min.js",
                        "~/Content/dist/js/adminlte.js"));



             // Use the development version of Modernizr to develop with and learn from. Then, when you're
             // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
             bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/plugins/fontawesome-free/css/all.min.css",
                     "~/Content/plugins/tempusdominus-bootstrap-4/css/tempusdominus-bootstrap-4.min.css",
                      "~/Content/plugins/icheck-bootstrap/icheck-bootstrap.min.css",
                      "~/Content/plugins/jqvmap/jqvmap.min.css",
                      "~/Content/dist/css/adminlte.min.css",
                      "~/Content/plugins/overlayScrollbars/css/OverlayScrollbars.min.css",
                      "~/Content/plugins/daterangepicker/daterangepicker.css",
                      "~/Content/plugins/summernote/summernote-bs4.min.css",
                      "~/Content/plugins/toastr/toastr.min.css",
                      "~/Content/plugins/sweetalert2-theme-bootstrap-4/bootstrap-4.min.css",
                      "~/Content/plugins/select2/css/select2.min.css",
                      "~/Content/plugins/select2-bootstrap4-theme/select2-bootstrap4.min.css",
                     // "~/Content/plugins/bootstrap4-editable/css/bootstrap-editable.css",
                      "~/Content/site.css"));

            BundleTable.EnableOptimizations = false;


        }
    }
}
