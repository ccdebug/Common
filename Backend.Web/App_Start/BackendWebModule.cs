using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Abp.Configuration.Startup;
using Abp.Modules;
using Abp.Web.Mvc;
using Abp.Zero.Configuration;
using Backend.Application;
using Backend.Entityframwork;
using Backend.WebApi;
using Castle.MicroKernel.Registration;
using Microsoft.Owin.Security;

namespace Backend.Web
{
    [DependsOn(
        typeof(BackendDataModule),
        typeof(BackendApplicationModule),
        typeof(BackendWebApiModule),
        typeof(AbpWebMvcModule))]
    public class BackendWebModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Modules.Zero().LanguageManagement.EnableDbLocalization();

            Configuration.Navigation.Providers.Add<BackendNavigationProvider>();

            Configuration.Modules.AbpWeb().AntiForgery.IsEnabled = false;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            IocManager.IocContainer.Register(
                Component
                    .For<IAuthenticationManager>()
                    .UsingFactoryMethod(() => HttpContext.Current.GetOwinContext().Authentication)
                    .LifestyleTransient()
            );
            //Areas
            AreaRegistration.RegisterAllAreas();

            //Routes
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            //Bundling
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}