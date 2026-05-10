using devops.Helpers;
using devops.Models;
using System.Data;
using System.Data.SqlClient;

namespace devops.Repository
{
    public class TrackerRepository : ITrackerRepository
    {
        private readonly SqlHelper _sqlHelper;
        public TrackerRepository(SqlHelper sqlHelper)
        {
            _sqlHelper = sqlHelper;
        }

        public async Task<ApiResponse<object>> CreateUpdateTaskAsyc(CreateTask task)
        {
            string query = "";

            if (task.Id == null)
            {
                query = @"INSERT INTO Tasks
                (
                    Title,
                    Description,
                    TaskType,
                    DeveloperId,
                    TesterId,
                    CreatedBy,
                    StartDateTime,
                    DueDateTime,
                    Priority,
                    Status,
                    UnitTestingStatus,
                    AcceptanceCriteria,
                    CreatedAt
                )
                VALUES
                (
                    @Title,
                    @Description,
                    @TaskType,
                    @DeveloperId,
                    @TesterId,
                    @CreatedBy,
                    @StartDateTime,
                    @DueDateTime,
                    @Priority,
                    @Status,
                    @UnitTestingStatus,
                    @AcceptanceCriteria,
                    @CreatedAt
                )";
            }
            else
            {
                query = @"UPDATE Tasks
                      SET
                        Title=@Title,
                        Description=@Description,
                        TaskType=@TaskType,
                        DeveloperId=@DeveloperId,
                        TesterId=@TesterId,
                        StartDateTime=@StartDateTime,
                        DueDateTime=@DueDateTime,
                        Priority=@Priority,
                        Status=@Status,
                        UnitTestingStatus=@UnitTestingStatus,
                        AcceptanceCriteria=@AcceptanceCriteria,
                        UpdatedAt=@UpdatedAt
                      WHERE Id=@Id";
            }

            SqlParameter[] parameters = new SqlParameter[]
            {   
                new SqlParameter("@Id", task.Id ?? (object)DBNull.Value),
                new SqlParameter("@Title", task.Title),
                new SqlParameter("@Description", task.Description ?? (object)DBNull.Value),
                new SqlParameter("@TaskType", task.TaskType),
                new SqlParameter("@DeveloperId", task.DeveloperId ?? (object)DBNull.Value),
                new SqlParameter("@TesterId", task.TesterId ?? (object)DBNull.Value),
                new SqlParameter("@CreatedBy", task.CreatedBy),
                new SqlParameter("@StartDateTime", task.StartDateTime ?? (object)DBNull.Value),
                new SqlParameter("@DueDateTime", task.DueDateTime ?? (object)DBNull.Value),
                new SqlParameter("@Priority", task.Priority),
                new SqlParameter("@Status", task.Status),
                new SqlParameter("@UnitTestingStatus", task.UnitTestingStatus ?? (object)DBNull.Value),
                new SqlParameter("@AcceptanceCriteria", task.AcceptanceCriteria ?? (object)DBNull.Value),
                new SqlParameter("@CreatedAt", DateTime.Now),
                new SqlParameter("@UpdatedAt", DateTime.Now)
            };

            var result = await _sqlHelper.ExecuteCommandAsync(query, parameters);

            if (result > 0)
            {
                return new ApiResponse<object>(true, 200, task.Id == null ? "Task created successfully" : "Task updated successfully", null);
            }

            return new ApiResponse<object>(false, 400, "Operation failed");
        }

        public async Task<ApiResponse<object>> GetTaskListAsync()
        {
            string query = @"SELECT * FROM Tasks";
            
            DataTable result = await _sqlHelper.ExecuteQueryAsyc(query);

            if(result.Rows.Count == 0)
            {
                return new ApiResponse<object>(false, 404, "No tasks found");
            }

            var jsonData = (from DataRow row in result.Rows
                            select new 
                            {
                                Id = row["Id"].ToString(),
                                Title = row["Title"].ToString(),
                                Description = row["Description"].ToString(),
                                TaskType = row["TaskType"].ToString(),
                                DeveloperId = row["DeveloperId"].ToString(),
                                TesterId = row["TesterId"].ToString(),
                                CreatedBy = row["CreatedBy"].ToString(),
                                StartDateTime = row["StartDateTime"].ToString(),
                                DueDateTime = row["DueDateTime"].ToString(),
                                Priority = row["Priority"].ToString(),
                                Status = row["Status"].ToString(),
                                UnitTestingStatus = row["UnitTestingStatus"].ToString(),
                                AcceptanceCriteria = row["AcceptanceCriteria"].ToString(),
                                CreatedAt = row["CreatedAt"].ToString(),
                                UpdatedAt = row["UpdatedAt"].ToString()

                            }).ToList();


            return new ApiResponse<object>(true, 200, "Task list retrieved successfully", jsonData);
        }
    }
}
