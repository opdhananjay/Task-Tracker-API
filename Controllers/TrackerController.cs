using devops.Helpers;
using devops.Models;
using devops.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace devops.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrackerController : ControllerBase
    {
        public readonly ITrackerRepository _trackerRepository;
        public TrackerController(ITrackerRepository trackerRepository)
        {
            _trackerRepository = trackerRepository;
        }

        #region Tasks 

        // Create Task 
        [HttpPost("CreateTask")]
        public async Task<IActionResult> CreateTask(CreateTask task)
        {
            if(string.IsNullOrEmpty(task.Title) || string.IsNullOrEmpty(task.TaskType) || string.IsNullOrEmpty(task.Priority) || string.IsNullOrEmpty(task.Status))
            {
                return BadRequest(new ApiResponse<object>(false, 400, "Title, TaskType, Priority and Status are required fields"));
            }
            
            var result = await _trackerRepository.CreateUpdateTaskAsyc(task);
            return StatusCode(result.StatusCode, result);
        }

        // Update Task 
        [HttpPost("UpdateTask")]
        public async Task<IActionResult> UpdateTask(CreateTask task)
        {
            if (string.IsNullOrEmpty(task.Title) || string.IsNullOrEmpty(task.TaskType) || string.IsNullOrEmpty(task.Priority) || string.IsNullOrEmpty(task.Status))
            {
                return BadRequest(new ApiResponse<object>(false, 400, "Title, TaskType, Priority and Status are required fields"));
            }

            if (task.Id == null)
            {
                return BadRequest(new ApiResponse<object>(false, 400, "Task Id is required for update"));
            }

            var result = await _trackerRepository.CreateUpdateTaskAsyc(task);
            return StatusCode(result.StatusCode, result);
        }

        // Get Task List 
        [HttpGet("GetTasks")]
        public async Task<IActionResult> GetTasks()
        {
            var result = await _trackerRepository.GetTaskListAsync();
            return StatusCode(result.StatusCode, result);
        }

        #endregion Tasks


        #region Organization

        [HttpPost("CreateOrg")]
        public async Task<IActionResult> CreateOrganization(Organization organization)
        {
            if(string.IsNullOrEmpty(organization.OrganizationName) || organization.CreatedBy == null)
            {
                return BadRequest(new ApiResponse<object>(false,400,"org name & created by required",null));
            }

            var result = await _trackerRepository.CreateUpdateOrganizationAsync(organization);

            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("UpdateOrg")]
        public async Task<IActionResult> UpdateOrganization(Organization organization)
        {
            if (string.IsNullOrEmpty(organization.OrganizationName))
            {
                return BadRequest(new ApiResponse<object>(false, 400, "org name & id required", null));
            }

            var result = await _trackerRepository.CreateUpdateOrganizationAsync(organization);

            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetOrganization")]
        public async Task<IActionResult> GetOrg(string? orgId)
        {
            var result = await _trackerRepository.GetOrganization(orgId);
            return StatusCode(result.StatusCode, result);
        }

        #endregion Organization

    }
}
