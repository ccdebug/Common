using System.Threading.Tasks;

namespace Backend.Core.Authorization.Users
{
    public interface IUserEmailer
    {
        Task SendEmailActivationLinkAsync(User user, string plainPassword = null);
    }
}