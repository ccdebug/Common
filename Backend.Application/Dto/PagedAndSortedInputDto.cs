using Abp.Application.Services.Dto;

namespace Backend.Application.Dto
{
    public class PagedAndSortedInputDto : PageInputDto, ISortedResultRequest
    {
        public string Sorting { get; set; }

        public PagedAndSortedInputDto()
        {
            MaxResultCount = AppConsts.DefaultPageSize;
        }
    }
}