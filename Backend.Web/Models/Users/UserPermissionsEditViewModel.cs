using System.Collections.Generic;
using Abp.AutoMapper;
using Backend.Application.Authorization.Users.Dto;
using Backend.Core.Authorization.Users;
using Backend.Web.Models.Common;

namespace Backend.Web.Models.Users
{
    [AutoMapFrom(typeof(GetUserPermissionsForEditOutput))]
    public class UserPermissionsEditViewModel : GetUserPermissionsForEditOutput, IPermissionsEditViewModel
    {
        public User User { get; private set; }

        public UserPermissionsEditViewModel(GetUserPermissionsForEditOutput output, User user)
        {
            User = user;
            output.MapTo(this);
        }
    }
}