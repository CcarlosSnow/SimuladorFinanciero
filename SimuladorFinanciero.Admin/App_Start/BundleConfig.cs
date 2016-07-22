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
            //    "~/js/jquery-ui/css/no-theme/jquery-ui-1.10.3.custom.min.css",
            //    "~/css/font-icons/entypo/css/entypo.css",
            //    "~/css/font-icons/font-awesome/css/font-awesome.min.css",
            //    "~/css/bootstrap.min.css",
            //    "~/css/neon-core.css",
            //    "~/css/neon-theme.css",
            //    "~/css/neon-forms.css",
            //    "~/css/custom.css",
            //    "~/css/red.css",
            //    "~/css/datatables.css"));

            bundles.Add(new StyleBundle("~/Administrador/css").Include(
                "~/js/jquery-ui/css/no-theme/jquery-ui-1.10.3.custom.min.css",
                "~/css/bootstrap.min.css",
                "~/css/neon-core.css",
                "~/css/neon-theme.css",
                "~/css/neon-forms.css",
                "~/css/red.css",
                "~/css/datatables.css"));

            bundles.Add(new ScriptBundle("~/Administrador/jsTop").Include("~/js/jquery-1.11.3.min.js"));

            bundles.Add(new ScriptBundle("~/Administrador/jsBottom").Include("~/js/gsap/TweenMax.min.js",
                "~/js/jquery-ui/js/jquery-ui-1.10.3.minimal.min.js",
                "~/js/bootstrap.js",
                "~/js/jquery.sidebar.min.js",
                "~/js/jquery.blockUI.js",
                "~/js/joinable.js",
                "~/js/resizeable.js",
                "~/js/neon-api.js",
                "~/js/bootstrap-datepicker.js",
                "~/js/bootstrap-datepicker.es.min.js",
                "~/js/confirm.js",
                "~/js/functions.js",
                "~/js/custom.js"));

            bundles.Add(new ScriptBundle("~/Administrador/jsContacto1").Include("~/js/gsap/TweenMax.min.js",
                "~/js/jquery-ui/js/jquery-ui-1.10.3.minimal.min.js",
                "~/js/bootstrap.js",
                "~/js/joinable.js",
                "~/js/js/jquery.blockUI.js",
                "~/js/resizeable.js",
                "~/js/neon-api.js",
                "~/js/functions.js",
                "~/js/custom.js"));

            bundles.Add(new ScriptBundle("~/Administrador/jsContacto2").Include("~/js/datatables.js",
                "~/js/confirm.js",
                "~/js/jquery.sidebar.min.js",
                "~/js/bootstrap-datepicker.js",
                "~/js/bootstrap-datepicker.es.min.js"
                ));
        }
    }
}
