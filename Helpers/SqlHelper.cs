using System.Data;
using System.Data.SqlClient;
using Serilog;

namespace devops.Helpers
{
    public class SqlHelper
    {
        private readonly string _connectionString;

        public SqlHelper(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Default");
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException("Connection string 'Default' not found or is empty in configuration.");
            }
        }


        // GENERAL FUNCTION (INSERT,UPDATE,DELETE)
        public async Task<int> ExecuteCommandAsync(string query, SqlParameter[] parameters = null)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        if (parameters != null && parameters.Length > 0)
                        {
                            command.Parameters.AddRange(parameters);
                        }

                        return await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "ExecuteCommandAsync query execution error");
                throw;
            }
        }


        // Execute Query Return DataTable 
        public async Task<DataTable> ExecuteQueryAsyc(string query, SqlParameter[] parameters = null)
        {
            DataTable dataTable = new DataTable();

            try
            {
                using(SqlConnection con = new SqlConnection(_connectionString))
                {
                    await con.OpenAsync();
                    using(SqlCommand cmd = new SqlCommand(query, con))
                    {
                        if(parameters != null && parameters.Length > 0)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }

                        using(SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dataTable);
                        }

                    }
                }

            }
            catch(Exception ex)
            {
                Log.Error(ex, "ExecuteQueryAsyc query execution error");
                throw;
            }

            return dataTable;
        }

    }
}