using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.IdentityFramework;
using Abp.Localization;
using Abp.Organizations;
using Abp.Runtime.Caching;
using Abp.Runtime.Validation;
using Abp.UI;
using Backend.Core.Authorization.Roles;

namespace Backend.Core.Authorization.Users
{
    public class UserManager : AbpUserManager<Role, User>
    {
        public UserManager(
            UserStore userStore,
            RoleManager roleManager,
            IPermissionManager permissionManager,
            IUnitOfWorkManager unitOfWorkManager,
            ICacheManager cacheManager,
            IRepository<OrganizationUnit, long> organizationUnitRepository,
            IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository,
            IOrganizationUnitSettings organizationUnitSettings,
            ILocalizationManager localizationManager,
            ISettingManager settingManager,
            IdentityEmailMessageService emailService,
            IUserTokenProviderAccessor userTokenProviderAccessor)
            : base(
                userStore,
                roleManager,
                permissionManager,
                unitOfWorkManager,
                cacheManager,
                organizationUnitRepository,
                userOrganizationUnitRepository,
                organizationUnitSettings,
                localizationManager,
                emailService,
                settingManager,
                userTokenProviderAccessor)
        {
        }

        public override Task SetGrantedPermissionsAsync(User user, IEnumerable<Permission> permissions)
        {
            var permissionArr = permissions as Permission[] ?? permissions.ToArray();
            if (user.UserName == User.AdminUserName && (!permissionArr.Any() ||
                                                        AppPermissions.AdminRequiredPermissions
                                                            .Intersect(permissionArr.Select(p => p.Name)).Count()
                                                        != AppPermissions.AdminRequiredPermissions.Count()))
            {
                throw new UserFriendlyException("不能删除Admin用户的[用户/角色]权限");
            }
            return base.SetGrantedPermissionsAsync(user, permissionArr);
        }
    }
}