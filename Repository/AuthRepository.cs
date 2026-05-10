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
            string query = "SELECT Id,Email,PasswordHash,PasswordSalt,Role,FirstName,IsActive FROM Users WHERE Email = @Email";

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
            bool isActive = (bool)userRow["IsActive"];

            if (!isActive)
            {
                return new ApiResponse<object>(false, 401, "user is inactive");
            }

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

        public async Task<ApiResponse<object>> ResetPasswordAsync(ResetPassword resetPassword)
        {
            try
            {
                // Verify user exists with matching Id and Email
                string verifyQuery = "SELECT Id FROM Users WHERE Id = @Id AND Email = @Email";

                SqlParameter[] verifyParameters = new SqlParameter[]
                {
                    new SqlParameter("@Id", resetPassword.Id),
                    new SqlParameter("@Email", resetPassword.Email)
                };

                DataTable verifyDt = await _sqlHelper.ExecuteQueryAsyc(verifyQuery, verifyParameters);

                if (verifyDt.Rows.Count == 0)
                {
                    return new ApiResponse<object>(false, 401, "User not found or invalid credentials");
                }

                // Create new password hash
                var newPasswordResult = PasswordHelper.CreatePasswordHashWithSalt(resetPassword.Password);

                // Update password
                string updateQuery = @"
                    UPDATE Users 
                    SET PasswordHash = @PasswordHash, PasswordSalt = @PasswordSalt 
                    WHERE Id = @Id AND Email = @Email
                ";

                SqlParameter[] updateParameters = new SqlParameter[]
                {
                    new SqlParameter("@Id", resetPassword.Id),
                    new SqlParameter("@Email", resetPassword.Email),
                    new SqlParameter("@PasswordHash", newPasswordResult.PasswordHash),
                    new SqlParameter("@PasswordSalt", newPasswordResult.PasswordSalt)
                };

                var result = await _sqlHelper.ExecuteCommandAsync(updateQuery, updateParameters);

                if (result > 0)
                {
                    return new ApiResponse<object>(true, 200, "Password reset successfully");
                }
                else
                {
                    return new ApiResponse<object>(false, 400, "Failed to reset password");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "ResetPasswordAsync FN");
                return new ApiResponse<object>(false, 500, $"An error occurred: {ex.Message}");
            }
        }

        // Remove User - Soft Delete
        public async Task<ApiResponse<object>> RemoveUserAsync(int userId)
        {
            try
            {
                // Verify user exists
                string verifyQuery = "SELECT Id FROM Users WHERE Id = @Id";

                SqlParameter[] verifyParameters = new SqlParameter[]
                {
                    new SqlParameter("@Id", userId)
                };

                DataTable verifyDt = await _sqlHelper.ExecuteQueryAsyc(verifyQuery, verifyParameters);

                if (verifyDt.Rows.Count == 0)
                {
                    return new ApiResponse<object>(false, 404, "User not found");
                }

                // Soft delete - mark user as inactive instead of deleting
                string updateQuery = "UPDATE Users SET IsActive = 0 WHERE Id = @Id";

                SqlParameter[] updateParameters = new SqlParameter[]
                {
                    new SqlParameter("@Id", userId)
                };

                var result = await _sqlHelper.ExecuteCommandAsync(updateQuery, updateParameters);

                if (result > 0)
                {
                    return new ApiResponse<object>(true, 200, "User deactivated successfully");
                }
                else
                {
                    return new ApiResponse<object>(false, 400, "Failed to deactivate user");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "RemoveUserAsync FN");
                return new ApiResponse<object>(false, 500, $"An error occurred: {ex.Message}");
            }
        }

        // Update User Info
        public async Task<ApiResponse<object>> UpdateUserAsync(UpdateUser updateUser)
        {
            try
            {
                // Verify user exists
                string verifyQuery = "SELECT Id FROM Users WHERE Id = @Id";

                SqlParameter[] verifyParameters = new SqlParameter[]
                {
                    new SqlParameter("@Id", updateUser.Id)
                };

                DataTable verifyDt = await _sqlHelper.ExecuteQueryAsyc(verifyQuery, verifyParameters);

                if (verifyDt.Rows.Count == 0)
                {
                    return new ApiResponse<object>(false, 404, "User not found");
                }

                // Update user info
                string updateQuery = @"
                    UPDATE Users 
                    SET FirstName = @FirstName, 
                        LastName = @LastName, 
                        Department = @Department, 
                        PhoneNumber = @PhoneNumber, 
                        EmployeeId = @EmployeeId,
                        IsActive = @IsActive
                    WHERE Id = @Id
                ";

                SqlParameter[] updateParameters = new SqlParameter[]
                {
                    new SqlParameter("@Id", updateUser.Id),
                    new SqlParameter("@FirstName", updateUser.FirstName),
                    new SqlParameter("@LastName", updateUser.LastName ?? (object)DBNull.Value),
                    new SqlParameter("@Department", updateUser.Department ?? (object)DBNull.Value),
                    new SqlParameter("@PhoneNumber", updateUser.PhoneNumber ?? (object)DBNull.Value),
                    new SqlParameter("@EmployeeId", updateUser.EmployeeId ?? (object)DBNull.Value),
                    new SqlParameter("@IsActive", updateUser.IsActive ? 1 : 0)
                };

                var result = await _sqlHelper.ExecuteCommandAsync(updateQuery, updateParameters);

                if (result > 0)
                {
                    return new ApiResponse<object>(true, 200, "User information updated successfully");
                }
                else
                {
                    return new ApiResponse<object>(false, 400, "Failed to update user information");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "UpdateUserAsync FN");
                return new ApiResponse<object>(false, 500, $"An error occurred: {ex.Message}");
            }
        }
    }
}