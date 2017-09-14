using Backend.Core.MultiTenancy;
using Backend.EntityFramework;
using System.Linq;

namespace Backend.Migrations.SeedData
{
    public class DefaultTenantCreator
    {
        private readonly BackendDbContext _context;

        public DefaultTenantCreator(BackendDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateUserAndRoles();
        }

        private void CreateUserAndRoles()
        {
            //Default tenant

            var defaultTenant = _context.Tenants.FirstOrDefault(t => t.TenancyName == Tenant.DefaultTenantName);
            if (defaultTenant == null)
            {
                _context.Tenants.Add(new Tenant {TenancyName = Tenant.DefaultTenantName, Name = Tenant.DefaultTenantName});
                _context.SaveChanges();
            }
        }
    }
}
