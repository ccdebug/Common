namespace Backend.Application.Authorization.Users.Dto
{
    public class GetUserForEditOutput
    {
        public UserEditDto User { get; set; }

        public UserRoleDto[] Roles { get; set; }
    }
}