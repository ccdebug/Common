using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Backend.Core.MultiTenancy;

namespace Backend.Application.Session.Dto
{
    [AutoMapFrom(typeof(Tenant))]
    public class TenantLoginInfoDto : EntityDto
    {
        public string TenancyName { get; set; }

        public string Name { get; set; }
    }
}