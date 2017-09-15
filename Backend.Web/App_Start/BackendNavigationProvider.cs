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
                            PageNames.DisConfManager, 
                            L("DisConfManager"),
                            "fa fa-th", "",
                            true,
                            requiredPermissionName: PermissionNames.Pages_DisConf
                        )
                        .AddItem(
                            new MenuItemDefinition(
                                "DisConfList",
                                L("DisConfList"),
                                "fa fa-circle-o",
                                "/DisConfig/List",
                                true,
                                requiredPermissionName: PermissionNames.Pages_DisConf
                            )
                        )
                        .AddItem(
                            new MenuItemDefinition(
                                "DisConfGroup",
                                L("DisConfGroup"),
                                "fa fa-circle-o",
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