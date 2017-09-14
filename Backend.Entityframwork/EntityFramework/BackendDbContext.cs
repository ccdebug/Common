using System.Data.Common;
using System.Data.Entity;
using Abp.Zero.EntityFramework;
using Backend.Core.Authorization.Roles;
using Backend.Core.Authorization.Users;
using Backend.Core.MultiTenancy;

namespace Backend.EntityFramework
{
    public class BackendDbContext : AbpZeroDbContext<Tenant, Role, User>
    {
        //TODO: Define an IDbSet for your Entities...

        /* NOTE: 
         *   Setting "Default" to base class helps us when working migration commands on Package Manager Console.
         *   But it may cause problems when working Migrate.exe of EF. If you will apply migrations on command line, do not
         *   pass connection string name to base classes. ABP works either way.
         */
        public BackendDbContext()
            : base("Default")
        {

        }

        /* NOTE:
         *   This constructor is used by ABP to pass connection string defined in BackendDataModule.PreInitialize.
         *   Notice that, actually you will not directly create an instance of BackendDbContext since ABP automatically handles it.
         */
        public BackendDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {

        }

        //This constructor is used in tests
        public BackendDbContext(DbConnection existingConnection)
            : base(existingConnection, false)
        {

        }

        public BackendDbContext(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.ChangeAbpTablePrefix<Tenant, Role, User>("");

            base.OnModelCreating(modelBuilder);
        }
    }
}