using Abp.Authorization;
using Abp.Localization;
using Abp.MultiTenancy;

namespace Backend.Core.Authorization
{
    public class BackendAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            var pages = context.GetPermissionOrNull(AppPermissions.Pages) ?? context.CreatePermission(AppPermissions.Pages, L("Pages"));

            var administration = pages.CreateChildPermission(AppPermissions.Pages_Administration, L("Administration"));
            var users = administration.CreateChildPermission(AppPermissions.Pages_Administration_Users, L("Users"));
            users.CreateChildPermission(AppPermissions.Pages_Administration_Users_Create, L("CreatingNewUser"));
            users.CreateChildPermission(AppPermissions.Pages_Administration_Users_Edit, L("EditingUser"));
            users.CreateChildPermission(AppPermissions.Pages_Administration_Users_Delete, L("DeletingUser"));
            users.CreateChildPermission(AppPermissions.Pages_Administration_Users_ChangePermissions, L("ChangingPermissions"));
            users.CreateChildPermission(AppPermissions.Pages_Administration_Users_Impersonation, L("LoginForUsers"));

            var disconf = pages.CreateChildPermission(AppPermissions.Pages_DisConf, L("DisConfManager"));
            disconf.CreateChildPermission(AppPermissions.Pages_DisConf_Group, L("DisConfGroup"));
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, BackendConsts.LocalizationSourceName);
        }
    }
}