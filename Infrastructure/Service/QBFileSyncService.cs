using Application.Interface;
using Core.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;

namespace Infrastructure.Service
{
    public class QBFileSyncService : IQBFileSyncService
    {
        private readonly string _connectionString;
        private readonly ILogger<QBFileSyncService> _logger;

        public QBFileSyncService(
            IConfiguration configuration,
            ILogger<QBFileSyncService> logger
        )
        {
            _connectionString = configuration.GetConnectionString("SAPDev") ?? throw new ArgumentNullException(nameof(configuration), "Configuration or SAP connection string is null");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger is null");
        }

        public async Task<ServiceResponse<int?>> Post(QBFileReceiveLog qBFileReceiveLog)
        {
            var response = new ServiceResponse<int?>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "INSERT INTO zb.QBFileSync (BusinessId, Ticket, RequestType, BlobUri, Password, ReceivedAt, Status) " +
                                 "VALUES (@BusinessId, @Ticket, @RequestType, @BlobUri, @Password, @ReceivedAt, @Status); " +
                                 "SELECT SCOPE_IDENTITY();";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@BusinessId", qBFileReceiveLog.BusinessId);
                        command.Parameters.AddWithValue("@Ticket", qBFileReceiveLog.Ticket);
                        command.Parameters.AddWithValue("@RequestType", qBFileReceiveLog.RequestType);
                        command.Parameters.AddWithValue("@BlobUri", qBFileReceiveLog.BlobUri);
                        command.Parameters.AddWithValue("@Password", string.IsNullOrEmpty(qBFileReceiveLog.Password) ? (object)DBNull.Value : qBFileReceiveLog.Password);
                        command.Parameters.AddWithValue("@ReceivedAt", qBFileReceiveLog.ReceivedAt);
                        command.Parameters.AddWithValue("@Status", qBFileReceiveLog.Status);

                        int? lastInsertedId = Convert.ToInt32(await command.ExecuteScalarAsync());

                        if (lastInsertedId != null)
                        {
                            response.Data = lastInsertedId.Value;
                            response.IsSuccess = true;
                        }
                        else
                        {
                            response.IsSuccess = false;
                            response.ErrorMessage = "Failed to insert file receive request.";
                            _logger.LogError("Failed to insert file receive request.");
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                response.IsSuccess = false;
                _logger.LogError($"SQL Error: {sqlEx.Message}");
                response.ErrorMessage = $"SQL Error: {sqlEx.Message}";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                _logger.LogError($"An error occurred: {ex.Message}");
                response.ErrorMessage = $"An error occurred: {ex.Message}";
            }
            return response;
        }

        public async Task<ServiceResponse<bool>> UpdateStatus(QBFileSyncUpdate qBFileSyncUpdate)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "UPDATE zb.QBFileSync SET FileAddress = @FileAddress, ProcessedAt = @ProcessedAt, Status = @Status, Error = @Error WHERE Ticket = @Ticket";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@FileAddress", qBFileSyncUpdate.FileAddress);
                        command.Parameters.AddWithValue("@ProcessedAt", qBFileSyncUpdate.ProcessedAt);
                        command.Parameters.AddWithValue("@Status", qBFileSyncUpdate.Status);
                        command.Parameters.AddWithValue("@Error", qBFileSyncUpdate.Error ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Ticket", qBFileSyncUpdate.Ticket);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        response.Data = rowsAffected > 0;
                        response.IsSuccess = true;
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                response.IsSuccess = false;
                _logger.LogError($"SQL Error: {sqlEx.Message}");
                response.ErrorMessage = $"SQL Error: {sqlEx.Message}";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                _logger.LogError($"An error occurred: {ex.Message}");
                response.ErrorMessage = $"An error occurred: {ex.Message}";
            }
            return response;
        }

        public async Task<ServiceResponse<bool>> Delete(int id)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "DELETE FROM zb.QBFileSync WHERE ID = @ID";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ID", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        response.Data = rowsAffected > 0;
                        response.IsSuccess = true;
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                response.IsSuccess = false;
                _logger.LogError($"SQL Error: {sqlEx.Message}");
                response.ErrorMessage = $"SQL Error: {sqlEx.Message}";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                _logger.LogError($"An error occurred: {ex.Message}");
                response.ErrorMessage = $"An error occurred: {ex.Message}";
            }
            return response;
        }
    }
}