using Abp.MultiTenancy;
using Backend.Core.Authorization.Users;

namespace Backend.Core.MultiTenancy
{
    public class Tenant : AbpTenant<User>
    {
        public Tenant()
        {

        }

        public Tenant(string tenancyName, string name)
            : base(tenancyName, name)
        {
        }
    }
}