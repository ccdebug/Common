﻿using System.Web.Optimization;

namespace Backend.Web
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.IgnoreList.Clear();

            bundles.Add(
                new ScriptBundle("~/Bundles/vendor/js/bottom")
                    .Include(
                        "~/lib/json2/json2.js",
                        "~/lib/jquery-validation/js/jquery.validate.min.js",
                        "~/lib/blockUI/jquery.blockUI.js",
                        "~/lib/sweetalert/dist/sweetalert.min.js",
                        "~/lib/spin.js/spin.min.js",
                        "~/lib/spin.js/jquery.spin.js",
                        "~/lib/toastr/toastr.min.js",
                        "~/lib/jstree/jstree.min.js",
                        "~/lib/moment/moment-with-locales.min.js",
                        "~/lib/abp-web-resources/Abp/Framework/scripts/abp.js",
                        "~/lib/abp-web-resources/Abp/Framework/scripts/libs/abp.jquery.js",
                        "~/lib/abp-web-resources/Abp/Framework/scripts/libs/abp.blockUI.js",
                        "~/lib/abp-web-resources/Abp/Framework/scripts/libs/abp.spin.js",
                        "~/lib/abp-web-resources/Abp/Framework/scripts/libs/abp.sweet-alert.js",
                        "~/lib/abp-web-resources/Abp/Framework/scripts/libs/abp.toastr.js",
                        "~/Content/adminlte/plugins/jQuery/jquery-2.2.3.min.js",
                        "~/Content/adminlte/bootstrap/js/bootstrap.min.js",
                        "~/Content/adminlte/plugins/iCheck/icheck.min.js",
                        "~/Content/adminlte/dist/js/app.min.js"
                    )
            );

            bundles.Add(
                new StyleBundle("~/Bundles/vendor/css")
                    .Include("~/Content/adminlte/bootstrap/css/bootstrap.min.css", new CssRewriteUrlTransform())
                    .Include("~/Content/adminlte/plugins/font-awesome/font-awesome.min.css", new CssRewriteUrlTransform())
                    .Include("~/Content/adminlte/plugins/ionicons/ionicons.min.css", new CssRewriteUrlTransform())
                    .Include("~/Content/adminlte/plugins/iCheck/square/blue.css", new CssRewriteUrlTransform())
                    .Include("~/Content/adminlte/dist/css/AdminLTE.min.css", new CssRewriteUrlTransform())
                    .Include("~/Content/adminlte/dist/css/skins/skin-blue.min.css", new CssRewriteUrlTransform())
                    .Include("~/Content/adminlte/plugins/iCheck/all.css", new CssRewriteUrlTransform())
                    .Include("~/lib/sweetalert/dist/sweetalert.css", new CssRewriteUrlTransform())
                    .Include("~/lib/toastr/toastr.min.css", new CssRewriteUrlTransform())
                    .Include("~/lib/jstree/themes/default/style.css", new CssRewriteUrlTransform())
                );

            bundles.Add(
                new ScriptBundle("~/Bundles/Common/js")
                    .IncludeDirectory("~/Common/Scripts", "*.js", true)
            );
        }
    }
}