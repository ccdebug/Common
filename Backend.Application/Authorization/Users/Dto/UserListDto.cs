using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Backend.Core.Authorization.Users;

namespace Backend.Application.Authorization.Users.Dto
{
    [AutoMapFrom(typeof(User))]
    public class UserListDto : EntityDto<long>, IPassivable, IHasCreationTime
    {
        public string Username { get; set; }

        public string EmailAddress { get; set; }

        public string PhoneNumber { get; set; }

        public DateTime? LastLoginTime { get; set; }

        public bool IsActive { get; set; }

        public bool IsEmailConfirmed { get; set; }

        public List<UserListRoleDto> Roles { get; set; }

        public DateTime CreationTime { get; set; }

        [AutoMapFrom(typeof(UserRole))]
        public class UserListRoleDto
        {
            public int RoleId { get; set; }

            public string RoleName { get; set; }
        }
    }
}