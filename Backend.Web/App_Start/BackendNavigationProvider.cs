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
                        "System",
                        L("System"),
                        "fa fa-wrench",
                        "/System"
                    ).AddItem(
                        new MenuItemDefinition(
                            "Users",
                            L("Users"),
                            "fa fa-users",
                            "/Users",
                            requiredPermissionName: PermissionNames.Pages_Users
                        )
                    )
                )
                .AddItem(
                    new MenuItemDefinition(
                            PageNames.DisConfManager,
                            L("DisConfManager"),
                            "fa fa-cog", "",
                            requiredPermissionName: PermissionNames.Pages_DisConf
                        )
                        .AddItem(
                            new MenuItemDefinition(
                                "DisConfList",
                                L("DisConfList"),
                                "fa fa-list-ul",
                                "/DisConfig/List",
                                requiredPermissionName: PermissionNames.Pages_DisConf
                            )
                        )
                        .AddItem(
                            new MenuItemDefinition(
                                "DisConfGroup",
                                L("DisConfGroup"),
                                "fa fa-list-ul",
                                "/DisConfig/Group",
                                requiredPermissionName: PermissionNames.Pages_DisConf_Group
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