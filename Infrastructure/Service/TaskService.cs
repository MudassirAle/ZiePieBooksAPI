using Application.Interface;
using Core.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;

namespace Infrastructure.Service
{
    public class TaskService : ITaskService
    {
        private readonly string _sapConnectionString;
        private readonly string _sapDevConnectionString;
        private readonly ILogger<TaskService> _logger;

        public TaskService(IConfiguration configuration, ILogger<TaskService> logger)
        {
            _sapConnectionString = configuration.GetConnectionString("SAP") ?? throw new ArgumentNullException(nameof(configuration), "Configuration or SAP connection string is null");
            _sapDevConnectionString = configuration.GetConnectionString("SAPDev") ?? throw new ArgumentNullException(nameof(configuration), "Configuration or SAPDev connection string is null");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger is null");
        }

        //public async Task<ServiceResponse<Core.Model.Task>> GetByOnboardingId(int onboardingId)
        //{
        //    var response = new ServiceResponse<Core.Model.Task>();
        //    try
        //    {
        //        Core.Model.Task task = null;

        //        using (SqlConnection sapDevConnection = new SqlConnection(_sapDevConnectionString))
        //        {
        //            await sapDevConnection.OpenAsync();

        //            string getTaskSql = "SELECT * FROM zb.Task WHERE OnboardingId = @OnboardingId";

        //            using (SqlCommand command = new SqlCommand(getTaskSql, sapDevConnection))
        //            {
        //                command.Parameters.AddWithValue("@OnboardingId", onboardingId);

        //                using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
        //                {
        //                    if (await dataReader.ReadAsync())
        //                    {
        //                        task = new Core.Model.Task
        //                        {
        //                            ID = Convert.ToInt32(dataReader["ID"]),
        //                            OnboardingId = Convert.ToInt32(dataReader["OnboardingId"]),
        //                            PasswordWorking = Convert.ToBoolean(dataReader["PasswordWorking"]),
        //                            TicketStatus = false // Placeholder, to be updated below
        //                        };
        //                    }
        //                    else
        //                    {
        //                        response.ErrorMessage = "Task not found for the given OnboardingId.";
        //                        _logger.LogError("Task not found for OnboardingId {OnboardingId}", onboardingId);
        //                        return response;
        //                    }
        //                }
        //            }

        //            string getTicketSql = "SELECT Ticket FROM zb.QBFileSync WHERE OnboardingId = @OnboardingId";

        //            string ticket;
        //            using (SqlCommand command = new SqlCommand(getTicketSql, sapDevConnection))
        //            {
        //                command.Parameters.AddWithValue("@OnboardingId", onboardingId);
        //                ticket = (string?)await command.ExecuteScalarAsync() ?? throw new Exception("No ticket found for the given OnboardingId.");
        //            }

        //            using (SqlConnection sapConnection = new SqlConnection(_sapConnectionString))
        //            {
        //                await sapConnection.OpenAsync();

        //                string getStatusSql = "SELECT Status FROM dbo.WCOutbox WHERE Ticket = @Ticket";

        //                using (SqlCommand command = new SqlCommand(getStatusSql, sapConnection))
        //                {
        //                    command.Parameters.AddWithValue("@Ticket", ticket);

        //                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
        //                    {
        //                        if (await reader.ReadAsync())
        //                        {
        //                            task.TicketStatus = reader.GetBoolean(reader.GetOrdinal("Status"));
        //                        }
        //                        else
        //                        {
        //                            _logger.LogError("No status found for ticket {Ticket}", ticket);
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //        response.Data = task;
        //        response.IsSuccess = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        response.IsSuccess = false;
        //        _logger.LogError(ex, "An error occurred while retrieving task by OnboardingId.");
        //        response.ErrorMessage = ex.Message;
        //    }

        //    return response;
        //}

        public async Task<ServiceResponse<Core.Model.Task>> GetByOnboardingId(int onboardingId)
        {
            var response = new ServiceResponse<Core.Model.Task>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_sapDevConnectionString))
                {
                    await connection.OpenAsync();

                    string sql = "SELECT * FROM zb.Task WHERE OnboardingId = @OnboardingId";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@OnboardingId", onboardingId);

                        using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
                        {
                            if (await dataReader.ReadAsync())
                            {
                                Core.Model.Task task = new Core.Model.Task
                                {
                                    ID = Convert.ToInt32(dataReader["ID"]),
                                    OnboardingId = Convert.ToInt32(dataReader["OnboardingId"]),
                                    PasswordWorking = Convert.ToBoolean(dataReader["PasswordWorking"]),
                                    TicketStatus = dataReader["TicketStatus"].ToString() ?? string.Empty
                                };
                                response.Data = task;
                                response.IsSuccess = true;
                            }
                            else
                            {
                                response.ErrorMessage = "Task not found.";
                                _logger.LogError("Task not found for OnboardingId {OnboardingId}", onboardingId);
                            }
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                response.IsSuccess = false;
                _logger.LogError(sqlEx, "SQL Error while retrieving task: {Message}", sqlEx.Message);
                response.ErrorMessage = sqlEx.Message;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                _logger.LogError(ex, "An error occurred while retrieving task: {Message}", ex.Message);
                response.ErrorMessage = ex.Message;
            }

            return response;
        }

        public async Task<ServiceResponse<int?>> ValidatePassword(ValidatePasswordDTO validatePasswordDTO)
        {
            var response = new ServiceResponse<int?>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_sapDevConnectionString))
                {
                    await connection.OpenAsync();

                    string sql = "INSERT INTO zb.Task (OnboardingId, PasswordWorking) " +
                                 "VALUES (@OnboardingId, @PasswordWorking); " +
                                 "SELECT SCOPE_IDENTITY();";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@OnboardingId", validatePasswordDTO.OnboardingId);
                        command.Parameters.AddWithValue("@PasswordWorking", validatePasswordDTO.PasswordWorking);

                        int? taskId = Convert.ToInt32(await command.ExecuteScalarAsync());

                        if (taskId.HasValue)
                        {
                            response.Data = taskId.Value;
                            response.IsSuccess = true;
                        }
                        else
                        {
                            response.IsSuccess = false;
                            response.ErrorMessage = "Failed to create task.";
                            _logger.LogError("Failed to create task for OnboardingId {OnboardingId}", validatePasswordDTO.OnboardingId);
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                response.IsSuccess = false;
                _logger.LogError(sqlEx, "SQL Error while creating task: {Message}", sqlEx.Message);
                response.ErrorMessage = sqlEx.Message;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                _logger.LogError(ex, "An error occurred while creating task: {Message}", ex.Message);
                response.ErrorMessage = ex.Message;
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> EnqueueTicket(string ticket)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_sapConnectionString))
                {
                    await connection.OpenAsync();

                    string sql = "UPDATE dbo.WCOutbox SET Status = 'Queued' WHERE Ticket = @Ticket";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Ticket", ticket);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        response.IsSuccess = rowsAffected > 0;
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                response.IsSuccess = false;
                _logger.LogError(sqlEx, "SQL Error while enqueueing ticket: {Message}", sqlEx.Message);
                response.ErrorMessage = sqlEx.Message;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                _logger.LogError(ex, "An error occurred while enqueueing ticket: {Message}", ex.Message);
                response.ErrorMessage = ex.Message;
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> UpdateStatus(int onboardingId)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_sapDevConnectionString))
                {
                    await connection.OpenAsync();

                    string sql = "UPDATE zb.Task SET TicketStatus = @TicketStatus WHERE OnboardingId = @OnboardingId";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@TicketStatus", "Processed");
                        command.Parameters.AddWithValue("@OnboardingId", onboardingId);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            response.Data = true;
                            response.IsSuccess = true;
                        }
                        else
                        {
                            response.Data = false;
                            response.IsSuccess = false;
                            response.ErrorMessage = $"No task found with OnboardingId {onboardingId} to update.";
                            _logger.LogError("Failed to update status for OnboardingId {OnboardingId}", onboardingId);
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                response.IsSuccess = false;
                _logger.LogError(sqlEx, "SQL Error while updating status: {Message}", sqlEx.Message);
                response.ErrorMessage = sqlEx.Message;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                _logger.LogError(ex, "An error occurred while updating status: {Message}", ex.Message);
                response.ErrorMessage = ex.Message;
            }

            return response;
        }
    }
}