using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Backend.Application.Authorization.Users.Dto;

namespace Backend.Application.Authorization.Users
{
    public interface IUserAppService : IApplicationService
    {
        Task<PagedResultDto<UserListDto>> GetUsers(GetUsersInput input);

        Task<GetUserForEditOutput> GetUserForEdit(NullableIdDto<long> input);

        Task CreateOrUpdateUser(CreateOrUpdateUserInput input);
    }
}