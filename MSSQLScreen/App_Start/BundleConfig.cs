using System.Web;
using System.Web.Optimization;

namespace MSSQLScreen
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/adminLTE").Include(
                        "~/admin-lte/js/app.js",
                        "~/admin-lte/plugins/fastclick/lib/fastclick.js"

                        ));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/login").Include(
                      "~/admin-lte/plugins/iCheck/iCheck.min.js",
                      "~/Scripts/iCheck.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/job").Include(
                        "~/admin-lte/plugins/datatables.net/js/jquery.dataTables.min.js",
                        "~/admin-lte/plugins/datatables.net-bs/js/dataTables.bootstrap.min.js",
                        "~/Scripts/dataTables.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/resource").Include(
                        "~/Scripts/cpuChart.js",
                        "~/Scripts/memoryChart.js",
                        "~/Scripts/hubConn.js",
                        "~/Scripts/jquery.signalR-2.2.2.min.js"
                        ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/Site.css",
                      "~/Content/css/font-awesome.min.css",
                      "~/admin-lte/plugins/Ionicons/css/ionicons.min.css",
                      "~/admin-lte/css/AdminLTE.css",
                      "~/admin-lte/css/skins/_all-skins.min.css"
                      ));

            bundles.Add(new StyleBundle("~/Login/css").Include(
                      "~/admin-lte/plugins/iCheck/square/blue.css"
                      ));

            bundles.Add(new StyleBundle("~/Job/css").Include(
                      "~/admin-lte/plugins/datatables.net-bs/css/dataTables.bootstrap.min.css"
                      ));
        }
    }
}
