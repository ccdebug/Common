﻿using System.Collections.Generic;

namespace Backend.Core.Authorization
{
    public static class AppPermissions
    {
        public const string Pages = "Pages";

        public const string Pages_Administration = "Pages.Administration";

        public const string Pages_Administration_Users = "Pages.Administration.Users";
        public const string Pages_Administration_Users_Create = "Pages.Administration.Users.Create";
        public const string Pages_Administration_Users_Edit = "Pages.Administration.Users.Edit";
        public const string Pages_Administration_Users_Delete = "Pages.Administration.Users.Delete";
        public const string Pages_Administration_Users_ChangePermissions = "Pages.Administration.Users.ChangePermissions";
        public const string Pages_Administration_Users_Impersonation = "Pages.Administration.Users.Impersonation";

        public const string Pages_DisConf = "Pages.DisConf";

        public const string Pages_DisConf_Group = "Pages.DisConf.Group";

        public static IEnumerable<string> AdminRequiredPermissions = new List<string>
        {
            AppPermissions.Pages,
            AppPermissions.Pages_Administration,
            AppPermissions.Pages_Administration_Users,
            AppPermissions.Pages_Administration_Users_Create,
            AppPermissions.Pages_Administration_Users_ChangePermissions,
            AppPermissions.Pages_Administration_Users_Edit,
            AppPermissions.Pages_Administration_Users_Delete,
            AppPermissions.Pages_Administration_Users_Impersonation
        };
    }
}