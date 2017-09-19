using System.Reflection;
using Abp.AutoMapper;
using Abp.Modules;
using Backend.Core;

namespace Backend.Application
{
    [DependsOn(typeof(BackendCoreModule), typeof(AbpAutoMapperModule), typeof(AbpAutoMapperModule))]
    public class BackendApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}