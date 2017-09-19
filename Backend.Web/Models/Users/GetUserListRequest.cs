using Backend.Web.Models.Common;

namespace Backend.Web.Models.Users
{
    public class GetUserListRequest : JqDatatableRequest
    {
        public string Filter { get; set; }
    }
}