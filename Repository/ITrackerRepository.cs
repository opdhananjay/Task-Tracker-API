using devops.Helpers;
using devops.Models;

namespace devops.Repository
{
    public interface ITrackerRepository
    {
        Task<ApiResponse<object>> CreateUpdateTaskAsyc(CreateTask task);
        Task<ApiResponse<object>> GetTaskListAsync();
    }
}
