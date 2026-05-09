using devops.Helpers;
using devops.Models;
using Serilog;
using System.Data;
using System.Data.SqlClient;

namespace devops.Repository
{
    public class AuthRepository : IAuthRepository
    {
        public readonly SqlHelper _sqlHelper;

        public AuthRepository(PasswordHelper passwordHelper, SqlHelper sqlHelper)
        {
            _sqlHelper = sqlHelper;
        }

        // User Registration 
        public async Task<ApiResponse<object>> RegisterUserAsync(Registration registration)
        {
            try
            {
                var passwordResult = PasswordHelper.CreatePasswordHashWithSalt(registration.Password);

                string query = @"
                        INSERT INTO Users (
                            FirstName, LastName, Email, PasswordHash, PasswordSalt, Role, Department, PhoneNumber, EmployeeId
                        ) 
                        VALUES (
                            @FirstName, @LastName, @Email, @PasswordHash, @PasswordSalt, @Role, @Department, @PhoneNumber, @EmployeeId
                        )
                    ";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@FirstName", registration.FirstName),
                    new SqlParameter("@LastName", registration.LastName ?? (object)DBNull.Value),
                    new SqlParameter("@Email", registration.Email),
                    new SqlParameter("@PasswordHash", passwordResult.PasswordHash),
                    new SqlParameter("@PasswordSalt", passwordResult.PasswordSalt),
                    new SqlParameter("@Role", registration.Role),
                    new SqlParameter("@Department", registration.Department ?? (object)DBNull.Value),
                    new SqlParameter("@PhoneNumber", registration.PhoneNumber ?? (object)DBNull.Value),
                    new SqlParameter("@EmployeeId", registration.EmployeeId ?? (object)DBNull.Value)
                };

                var result = await _sqlHelper.ExecuteCommandAsync(
                    query,
                    parameters  
                );

                if (result > 0)
                {
                    return new ApiResponse<object>(true, 200, "User registered successfully");
                }
                else
                {
                    return new ApiResponse<object>(false, 400, "Registration failed");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "RegisterUserAsync FN");
                return new ApiResponse<object>(false, 500, $"An error occurred: {ex.Message}");
            }
        }


        // Login Call 
        public async Task<ApiResponse<object>> LoginAsync(Login login)
        {
            string query = "SELECT Id,Email,PasswordHash,PasswordSalt,Role,FirstName FROM Users WHERE Email = @Email";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Email",login.Email)
            };

            DataTable dt = await _sqlHelper.ExecuteQueryAsyc(query, parameters);

            if(dt.Rows.Count == 0)
            {
                return new ApiResponse<object>(false, 401, "User not found");
            }

            var userRow = dt.Rows[0];
            string email = userRow["Email"].ToString();
            string passwordHash = userRow["PasswordHash"].ToString();
            string passwordSalt = userRow["PasswordSalt"].ToString();
            string role = userRow["Role"].ToString();
            int userId = (int)userRow["Id"];

            // Verify password
            bool isPasswordValid = PasswordHelper.VerifyPassword(login.Password,passwordHash, passwordSalt);

            if (!isPasswordValid)
            {
                return new ApiResponse<object>(false, 401, "Invalid password");
            }

            return new ApiResponse<object>(true, 200, "Login successful", new { UserId = userId, Email = email, Role = role });
        }


    }
}