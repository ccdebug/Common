using Abp.Authorization;
using Backend.Core.Authorization.Roles;
using Backend.Core.Authorization.Users;
using Backend.Core.MultiTenancy;

namespace Backend.Core.Authorization
{
    public class PermissionChecker : PermissionChecker<Tenant, Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {

        }
    }
}