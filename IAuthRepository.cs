using devops.Helpers;
using devops.Models;

namespace devops.Repository
{
    public interface IAuthRepository
    {
        Task<ApiResponse<object>> RegisterUserAsync(Registration registration);
    }   
}
