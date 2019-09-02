using System.Threading.Tasks;
using Entities.User;

namespace Services.Services
{
    public interface IJwtService
    {
        Task<AccessToken> GenerateAsync(User user);
    }
}