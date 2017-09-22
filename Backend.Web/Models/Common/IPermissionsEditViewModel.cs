using System.Collections.Generic;
using Backend.Application.Authorization.Permissions.Dto;

namespace Backend.Web.Models.Common
{
    public interface IPermissionsEditViewModel
    {
        List<FlatPermissionDto> Permissions { get; set; }

        List<string> GrantedPermissionNames { get; set; }
    }
}