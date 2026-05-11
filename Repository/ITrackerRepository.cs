using devops.Helpers;
using devops.Models;

namespace devops.Repository
{
    public interface ITrackerRepository
    {
        Task<ApiResponse<object>> CreateUpdateTaskAsyc(CreateTask task);
        Task<ApiResponse<object>> GetTaskListAsync();
        Task<ApiResponse<object>> CreateUpdateOrganizationAsync(Organization organization);

        Task<ApiResponse<object>> GetOrganization(string? Id);

    }
}
