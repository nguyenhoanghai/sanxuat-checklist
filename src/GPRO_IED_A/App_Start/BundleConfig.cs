using System.Web;
using System.Web.Optimization;

namespace SanXuatCheckList
{
    public class BundleConfig
    { 
        public static void RegisterBundles(BundleCollection bundles)
        {  
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/Admin/jquery").Include(
                       "~/Scripts/IED/Authenticate/Login.js"));

            bundles.Add(new ScriptBundle("~/bundles/Cjquery").Include(
                    "~/Scripts/Client/Layout/js/jquery.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/ClientJquery").Include(
                        "~/Scripts/Client/Layout/js/nprogress.js",
                      "~/Scripts/Client/Layout/js/flot/jquery.flot.js"  
                        //"~/Scripts/Client/nprogress.js",
                        //"~/Scripts/Client/nprogress.js",
                        //"~/Scripts/Client/nprogress.js",
                        //"~/Scripts/Client/nprogress.js",
                        //"~/Scripts/Client/nprogress.js",
                        //"~/Scripts/Client/nprogress.js",
                        //"~/Scripts/Client/nprogress.js",
                        //"~/Scripts/Client/nprogress.js",
                        //"~/Scripts/Client/nprogress.js",
                        //"~/Scripts/Client/nprogress.js",                      
                        
                        ));




            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Admin/css").Include(
                     "~/Content/Login/LoginPage.css",
                     "~/Content/Login/style.css"));

            bundles.Add(new StyleBundle("~/Content/font").Include(
                "~/Content/font-awesome-4.7.0/css/font-awesome.css" 
                   ));

            bundles.Add(new StyleBundle("~/Content/Client/css").Include(
                "~/Content/font-awesome-4.7.0/css/font-awesome.css",
                      "~/Content/Layout/animate.min.css",
                      "~/Content/Layout/Web/main.min.css",
                      "~/Content/Layout/Web/Layout.min.css",
                      "~/Content/Layout/Web/Responsive.min.css", 
                       "~/Content/Layout/icheck/flat/green.css",
                       "~/Content/Layout/nprogress.css" ,
                       "~/Content/Layout/AdminCss.css"  
                       //"~/Content/Client/", 
                       //"~/Content/Client/", 
                       //"~/Content/Client/", 
                       //"~/Content/Client/", 
                       //"~/Content/Client/", 
                       //"~/Content/Client/", 
                       //"~/Content/Client/", 

                      ));
             
            BundleTable.EnableOptimizations = true;
        }
    }
}
