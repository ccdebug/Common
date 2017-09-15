using Abp.Authorization;
using Abp.Localization;
using Abp.MultiTenancy;

namespace Backend.Core.Authorization
{
    public class BackendAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            context.CreatePermission(PermissionNames.Pages_Users, L("Users"));
            context.CreatePermission(PermissionNames.Pages_Roles, L("Roles"));
            context.CreatePermission(PermissionNames.Pages_Tenants, L("Tenants"), multiTenancySides: MultiTenancySides.Host);
            context.CreatePermission(PermissionNames.Pages_DisConf, L("DisConf"));
            context.CreatePermission(PermissionNames.Pages_DisConf_Group, L("DisConf.Group"));
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, BackendConsts.LocalizationSourceName);
        }
    }
}