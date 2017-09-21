namespace Backend.Application.Dto
{
    public class PagedSortedAndFilterdInputDto : PagedAndSortedInputDto
    {
        public string Filter { get; set; }
    }
}