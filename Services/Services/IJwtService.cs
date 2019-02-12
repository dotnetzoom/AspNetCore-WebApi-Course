using Entities;
using System.Threading.Tasks;

namespace Services
{
    public interface IJwtService
    {
        Task<AccessToken> GenerateAsync(User user);
    }
}