using System.Collections.Generic;
using System.Data.Entity;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Collections.Extensions;
using Abp.Extensions;
using Backend.Application.Authorization.Users.Dto;
using Backend.Core.Authorization.Users;
using Abp.Linq.Extensions;
using System.Linq;
using Backend.Core.Authorization.Roles;

namespace Backend.Application.Authorization.Users
{
    public class UserAppService : BackendAppServiceBase, IUserAppService
    {
        private readonly UserManager _userManager;
        private readonly RoleManager _roleManager;

        public UserAppService(UserManager userManager,
            RoleManager roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<PagedResultDto<UserListDto>> GetUsers(GetUsersInput input)
        {
            var query = UserManager.Users
                .Include(u => u.Roles)
                .WhereIf(input.Role.HasValue, u => u.Roles.Any(r => r.RoleId == input.Role.Value))
                .WhereIf(
                    !input.Filter.IsNullOrWhiteSpace(),
                    u =>
                        u.Name.Contains(input.Filter) ||
                        u.Surname.Contains(input.Filter) ||
                        u.UserName.Contains(input.Filter) ||
                        u.EmailAddress.Contains(input.Filter)
                );

            var userCount = await query.CountAsync();
            var users = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();

            var userListDtos = users.MapTo<List<UserListDto>>();
            await FillRoleNames(userListDtos);

            return new PagedResultDto<UserListDto>(userCount, userListDtos);
        }

        private async Task FillRoleNames(List<UserListDto> userListDtos)
        {
            var distinctRoleIds = (
                from userListDto in  userListDtos
                from userListRoleDto in  userListDto.Roles
                select userListRoleDto.RoleId
                ).Distinct();

            var roleNames = new Dictionary<int, string>();
            foreach (var roleId in distinctRoleIds)
            {
                roleNames[roleId] = (await _roleManager.GetRoleByIdAsync(roleId)).DisplayName;
            }

            foreach (var userListDto in userListDtos)
            {
                foreach (var userListRoleDto in userListDto.Roles)
                {
                    userListRoleDto.RoleName = roleNames[userListRoleDto.RoleId];
                }

                userListDto.Roles = userListDto.Roles.OrderBy(r => r.RoleName).ToList();
            }
        }
    }
}