using System.Reflection;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Xml;
using Abp.Modules;
using Abp.Zero;
using Abp.Zero.Configuration;
using Backend.Core.Authorization;
using Backend.Core.Authorization.Roles;
using Backend.Core.Authorization.Users;
using Backend.Core.Configuration;
using Backend.Core.MultiTenancy;

namespace Backend.Core
{
    [DependsOn(typeof(AbpZeroCoreModule))]
    public class BackendCoreModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Auditing.IsEnabledForAnonymousUsers = true;

            //Declare entity types
            Configuration.Modules.Zero().EntityTypes.Tenant = typeof(Tenant);
            Configuration.Modules.Zero().EntityTypes.Role = typeof(Role);
            Configuration.Modules.Zero().EntityTypes.User = typeof(User);

            Configuration.Localization.Sources.Add(
                new DictionaryBasedLocalizationSource(
                    BackendConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        Assembly.GetExecutingAssembly(),
                        "Backend.Core.Localization.Source"
                    )
                )
            );

            Configuration.Authorization.Providers.Add<BackendAuthorizationProvider>();

            //Configuration.Settings.Providers.Add<AppSettingProvider>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}