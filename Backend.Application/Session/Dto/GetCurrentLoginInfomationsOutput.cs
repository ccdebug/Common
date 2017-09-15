namespace Backend.Application.Session.Dto
{
    public class GetCurrentLoginInfomationsOutput
    {
        public UserLoginInfoDto User { get; set; }

        public TenantLoginInfoDto Tenant { get; set; }
    }
}