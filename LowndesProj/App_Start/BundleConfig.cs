using System.Web;
using System.Web.Optimization;

namespace LowndesProj {
    public class BundleConfig {
    // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles( BundleCollection bundles ) {
            bundles.Add( new ScriptBundle( "~/bundles/jquery" ).Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/typeahead.bundle.js") );

            bundles.Add( new ScriptBundle( "~/bundles/jqueryval" ).Include(
                        "~/Scripts/jquery.validate.js" ) );


            bundles.Add( new ScriptBundle( "~/bundles/angular" ).Include(
                "~/Scripts/angular.js",
                "~/Scripts/angular-animate.js",
                "~/Scripts/angular-ui/ui-bootstrap-tpls.min.js",
                "~/Scripts/angular-ui-router.js",
                "~/Scripts/angular-sanitize.js"
                //"~/Scripts/angular-strap.js",
                //"~/Scripts/angular-strap.tpl.js",
            ) );

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add( new ScriptBundle( "~/bundles/modernizr" ).Include(
                        "~/Scripts/modernizr-*" ) );

            bundles.Add( new ScriptBundle( "~/bundles/user" ).Include( 
                "~/Scripts/script.js") );

            bundles.Add( new ScriptBundle( "~/bundles/bootstrap" ).Include(
                        "~/Scripts/bootstrap.js",
                        "~/Scripts/respond.js" ) );

            bundles.Add( new StyleBundle( "~/Content/css" ).Include(
                //"~/Content/jquery-ui.min.css",
                        "~/Content/bootstrap.css",
                        "~/Content/css/font-awesome.css",
                        "~/Content/angular-motion.css",
                        "~/Content/bootstrap-additions.css",
                        "~/Content/Site.css" ) );
        }
    }
}
