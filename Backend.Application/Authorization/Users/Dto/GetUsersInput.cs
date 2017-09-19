using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;

namespace Backend.Application.Authorization.Users.Dto
{
    public class GetUsersInput : PagedAndSortedResultRequestDto, IShouldNormalize
    {
        public string Filter { get; set; }

        public int? Role { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "CreationTime";
            }
        }
    }
}