using Abp.Runtime.Validation;
using Backend.Application.Dto;

namespace Backend.Application.Authorization.Users.Dto
{
    public class GetUsersInput : PagedSortedAndFilterdInputDto, IShouldNormalize
    {
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