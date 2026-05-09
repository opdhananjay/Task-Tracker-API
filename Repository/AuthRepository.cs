using devops.Helpers;
using devops.Models;
using Serilog;
using Microsoft.Data.SqlClient;

namespace devops.Repository
{
    public class AuthRepository : IAuthRepository
    {
        public readonly PasswordHelper _passwordHelper;
        public readonly SqlHelper _sqlHelper;
        public readonly string _connectionString;

        public AuthRepository(PasswordHelper passwordHelper, SqlHelper sqlHelper)
        {
            _passwordHelper = passwordHelper;
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
                    return new ApiResponse<object>(true, 201, "User registered successfully");
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
    }
}