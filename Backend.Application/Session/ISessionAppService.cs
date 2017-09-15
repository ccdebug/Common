using System.Threading.Tasks;
using Abp.Application.Services;
using Backend.Application.Session.Dto;

namespace Backend.Application.Session
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInfomationsOutput> GetCurrentLoginInfomations();
    }
}