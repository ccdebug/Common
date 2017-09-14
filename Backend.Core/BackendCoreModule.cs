using System.Reflection;
using Abp.Modules;
using Abp.Zero;
using Abp.Zero.Configuration;
using Backend.Core.Authorization.Roles;
using Backend.Core.Authorization.Users;
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
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}