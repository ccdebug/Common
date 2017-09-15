using Abp.Application.Navigation;

namespace Backend.Web.Models.Layout
{
    public class SideBarNavViewModel
    {
        public UserMenu UserMenu { get; set; }

        public string ActiveMenuItemName { get; set; }
    }
}