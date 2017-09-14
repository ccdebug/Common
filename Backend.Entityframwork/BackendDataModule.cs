using Abp.Modules;
using Abp.Zero.EntityFramework;
using Backend.Core;
using Backend.EntityFramework;
using System.Data.Entity;
using System.Reflection;

namespace Backend.Entityframwork
{
    [DependsOn(typeof(AbpZeroEntityFrameworkModule), typeof(BackendCoreModule))]
    public class BackendDataModule : AbpModule
    {
        public override void PreInitialize()
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<BackendDbContext>());

            Configuration.DefaultNameOrConnectionString = "Default";
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}