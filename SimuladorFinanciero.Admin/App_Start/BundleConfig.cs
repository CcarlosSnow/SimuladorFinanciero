using System.Web;
using System.Web.Optimization;

namespace SimuladorFinanciero
{
    public class BundleConfig
    {
        // Para obtener más información sobre Bundles, visite http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Utilice la versión de desarrollo de Modernizr para desarrollar y obtener información. De este modo, estará
            // preparado para la producción y podrá utilizar la herramienta de compilación disponible en http://modernizr.com para seleccionar solo las pruebas que necesite.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            //bundles.Add(new StyleBundle("~/Administrador/css").Include(
            //    "~/assets/js/jquery-ui/css/no-theme/jquery-ui-1.10.3.custom.min.css",
            //    "~/assets/css/font-icons/entypo/css/entypo.css",
            //    "~/assets/css/font-icons/font-awesome/css/font-awesome.min.css",
            //    "~/assets/css/bootstrap.min.css",
            //    "~/assets/css/neon-core.css",
            //    "~/assets/css/neon-theme.css",
            //    "~/assets/css/neon-forms.css",
            //    "~/assets/css/custom.css",
            //    "~/assets/css/red.css",
            //    "~/assets/css/datatables.css"));

            bundles.Add(new StyleBundle("~/Administrador/css").Include(
                "~/assets/js/jquery-ui/css/no-theme/jquery-ui-1.10.3.custom.min.css",
                "~/assets/css/bootstrap.min.css",
                "~/assets/css/neon-core.css",
                "~/assets/css/neon-theme.css",
                "~/assets/css/neon-forms.css",
                "~/assets/css/red.css",
                "~/assets/css/datatables.css"));

            bundles.Add(new ScriptBundle("~/Administrador/jsTop").Include("~/assets/js/jquery-1.11.3.min.js"));

            bundles.Add(new ScriptBundle("~/Administrador/jsBottom").Include("~/assets/js/gsap/TweenMax.min.js",
                "~/assets/js/jquery-ui/js/jquery-ui-1.10.3.minimal.min.js",
                "~/assets/js/bootstrap.js",
                "~/assets/js/jquery.sidebar.min.js",
                "~/assets/js/jquery.blockUI.js",
                "~/assets/js/joinable.js",
                "~/assets/js/resizeable.js",
                "~/assets/js/neon-api.js",
                "~/assets/js/bootstrap-datepicker.js",
                "~/assets/js/bootstrap-datepicker.es.min.js",
                "~/assets/js/confirm.js",
                "~/assets/js/functions.js",
                "~/assets/js/custom.js"));

            bundles.Add(new ScriptBundle("~/Administrador/jsContacto1").Include("~/assets/js/gsap/TweenMax.min.js",
                "~/assets/js/jquery-ui/js/jquery-ui-1.10.3.minimal.min.js",
                "~/assets/js/bootstrap.js",
                "~/assets/js/joinable.js",
                "~/assets/js/js/jquery.blockUI.js",
                "~/assets/js/resizeable.js",
                "~/assets/js/neon-api.js",
                "~/assets/js/functions.js",
                "~/assets/js/custom.js"));

            bundles.Add(new ScriptBundle("~/Administrador/jsContacto2").Include("~/assets/js/datatables.js",
                "~/assets/js/confirm.js",
                "~/assets/js/jquery.sidebar.min.js",
                "~/assets/js/bootstrap-datepicker.js",
                "~/assets/js/bootstrap-datepicker.es.min.js"
                ));
        }
    }
}
