using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;

namespace Backend.Application.Dto
{
    public class PageInputDto : IPagedResultRequest
    {
        [Range(1, AppConsts.MaxPageSize)]
        public int MaxResultCount { get; set; }

        [Range(0, int.MaxValue)]
        public int SkipCount { get; set; }

        public PageInputDto()
        {
            MaxResultCount = AppConsts.DefaultPageSize;
        }
    }
}