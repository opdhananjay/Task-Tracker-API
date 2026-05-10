using devops.Helpers;
using devops.Models;

namespace devops.Repository
{
    public interface IAuthRepository
    {
        Task<ApiResponse<object>> RegisterUserAsync(Registration registration);
        Task<ApiResponse<object>> LoginAsync(Login login);
        Task<ApiResponse<object>> ResetPasswordAsync(ResetPassword resetPassword);
        Task<ApiResponse<object>> RemoveUserAsync(int userId);
        Task<ApiResponse<object>> UpdateUserAsync(UpdateUser updateUser);
    }   
}
