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




        #endregion Organization

    }
}
