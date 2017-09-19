using Abp.Application.Navigation;
using Abp.Localization;
using Backend.Core;
using Backend.Core.Authorization;

namespace Backend.Web
{
    public class BackendNavigationProvider : NavigationProvider
    {
        public override void SetNavigation(INavigationProviderContext context)
        {
            context.Manager.MainMenu
                .AddItem(
                    new MenuItemDefinition(
                        PageNames.Backend.Common.Administration,
                        L("Administration"),
                        "fa fa-wrench"
                    ).AddItem(
                        new MenuItemDefinition(
                            PageNames.Backend.Common.Users,
                            L("Users"),
                            "fa fa-users",
                            "/Users",
                            requiredPermissionName: AppPermissions.Pages_Administration_Users
                        )
                    )
                )
                .AddItem(
                    new MenuItemDefinition(
                            PageNames.Backend.DisConf.DisConfManager,
                            L("DisConfManager"),
                            "fa fa-cog"
                        )
                        .AddItem(
                            new MenuItemDefinition(
                                PageNames.Backend.DisConf.DisConfManager,
                                L("DisConfList"),
                                "fa fa-list-ul",
                                "/DisConfig/List",
                                requiredPermissionName: AppPermissions.Pages_DisConf
                            )
                        )
                        .AddItem(
                            new MenuItemDefinition(
                                PageNames.Backend.DisConf.DisConfGroup,
                                L("DisConfGroup"),
                                "fa fa-list-ul",
                                "/DisConfig/Group",
                                requiredPermissionName: AppPermissions.Pages_DisConf_Group
                            )
                        )
                );
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, BackendConsts.LocalizationSourceName);
        }
    }
}