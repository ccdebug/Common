using System.Threading.Tasks;
using Abp.Auditing;
using Abp.AutoMapper;
using Backend.Application.Session.Dto;

namespace Backend.Application.Session
{
    public class SessionAppService : BackendAppServiceBase, ISessionAppService
    {
        [DisableAuditing]
        public async Task<GetCurrentLoginInfomationsOutput> GetCurrentLoginInfomations()
        {
            var output = new GetCurrentLoginInfomationsOutput();

            if (AbpSession.UserId.HasValue)
            {
                output.User = (await GetCurrentUserAsync()).MapTo<UserLoginInfoDto>();;
            }

            if (AbpSession.TenantId.HasValue)
            {
                output.Tenant = (await GetCurrentTenantAsync()).MapTo<TenantLoginInfoDto>();
            }

            return output;
        }
    }
}