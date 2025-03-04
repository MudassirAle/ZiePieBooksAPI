using Application.Interface;
using Core.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;

namespace Infrastructure.Service
{
    public class PreOrderService : IPreOrderService
    {
        private readonly string _connectionString;
        private readonly ILogger<PreOrderService> _logger;

        public PreOrderService(IConfiguration configuration, ILogger<PreOrderService> logger)
        {
            _connectionString = configuration.GetConnectionString("SAPDev") ?? throw new ArgumentNullException(nameof(configuration), "Configuration or SAP connection string is null");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger is null");
        }

        public async Task<ServiceResponse<PreOrder>> GetByBusinessId(int businessId)
        {
            var response = new ServiceResponse<PreOrder>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = @" SELECT TOP 1 * FROM zb.PreOrder WHERE BusinessId = @BusinessId AND IsActive = 1";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@BusinessId", businessId);

                        using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
                        {
                            if (await dataReader.ReadAsync())
                            {
                                PreOrder preOrder = new PreOrder
                                {
                                    ID = Convert.ToInt32(dataReader["ID"]),
                                    BusinessId = Convert.ToInt32(dataReader["BusinessId"]),
                                    Platform = dataReader["Platform"].ToString() ?? string.Empty,
                                    QBDesktopVersion = dataReader["QBDesktopVersion"] is DBNull ? null : dataReader["QBDesktopVersion"].ToString(),
                                    ExistingAccounting = Convert.ToBoolean(dataReader["ExistingAccounting"]),
                                    LatestAccountingDate = dataReader["LatestAccountingDate"] is DBNull ? default : Convert.ToDateTime(dataReader["LatestAccountingDate"]),
                                    NumBankAccounts = Convert.ToInt32(dataReader["NumBankAccounts"]),
                                    NumCreditCards = Convert.ToInt32(dataReader["NumCreditCards"]),
                                    IncludeChecks = Convert.ToBoolean(dataReader["IncludeChecks"]),
                                    ChecksPerMonth = Convert.ToInt32(dataReader["ChecksPerMonth"]),
                                    IsPaymentMade = Convert.ToBoolean(dataReader["IsPaymentMade"])
                                };

                                response.Data = preOrder;
                                response.IsSuccess = true;
                            }
                            else
                            {
                                response.ErrorMessage = "PreOrder record not found.";
                                _logger.LogError("PreOrder record not found.");
                            }
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

        public async Task<ServiceResponse<PreOrder>> GetById(int id)
        {
            var response = new ServiceResponse<PreOrder>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "SELECT * FROM zb.PreOrder WHERE ID = @ID";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ID", id);

                        using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
                        {
                            if (await dataReader.ReadAsync())
                            {
                                PreOrder preOrder = new PreOrder
                                {
                                    ID = Convert.ToInt32(dataReader["ID"]),
                                    BusinessId = Convert.ToInt32(dataReader["BusinessId"]),
                                    Platform = dataReader["Platform"].ToString() ?? string.Empty,
                                    QBDesktopVersion = dataReader["QBDesktopVersion"] is DBNull ? null : dataReader["QBDesktopVersion"].ToString(),
                                    ExistingAccounting = Convert.ToBoolean(dataReader["ExistingAccounting"]),
                                    LatestAccountingDate = dataReader["LatestAccountingDate"] is DBNull ? default : Convert.ToDateTime(dataReader["LatestAccountingDate"]),
                                    NumBankAccounts = Convert.ToInt32(dataReader["NumBankAccounts"]),
                                    NumCreditCards = Convert.ToInt32(dataReader["NumCreditCards"]),
                                    IncludeChecks = Convert.ToBoolean(dataReader["IncludeChecks"]),
                                    ChecksPerMonth = Convert.ToInt32(dataReader["ChecksPerMonth"]),
                                    IsPaymentMade = Convert.ToBoolean(dataReader["IsPaymentMade"])
                                };
                                response.Data = preOrder;
                                response.IsSuccess = true;
                            }
                            else
                            {
                                response.ErrorMessage = "PreOrder not found.";
                                _logger.LogError("PreOrder not found.");
                            }
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

        public async Task<ServiceResponse<int?>> Post(PreOrder preOrder)
        {
            var response = new ServiceResponse<int?>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = @"
                        INSERT INTO zb.PreOrder (BusinessId, Platform, QBDesktopVersion, ExistingAccounting, LatestAccountingDate, 
                                NumBankAccounts, NumCreditCards, IncludeChecks, ChecksPerMonth, IsPaymentMade)
                        VALUES (@BusinessId, @Platform, @QBDesktopVersion, @ExistingAccounting, @LatestAccountingDate, 
                                @NumBankAccounts, @NumCreditCards, @IncludeChecks, @ChecksPerMonth, @IsPaymentMade);
                        SELECT SCOPE_IDENTITY();";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@BusinessId", preOrder.BusinessId);
                        command.Parameters.AddWithValue("@Platform", preOrder.Platform);
                        command.Parameters.AddWithValue("@QBDesktopVersion", preOrder.QBDesktopVersion ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@ExistingAccounting", preOrder.ExistingAccounting);
                        command.Parameters.AddWithValue("@LatestAccountingDate", preOrder.LatestAccountingDate ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@NumBankAccounts", preOrder.NumBankAccounts);
                        command.Parameters.AddWithValue("@NumCreditCards", preOrder.NumCreditCards);
                        command.Parameters.AddWithValue("@IncludeChecks", preOrder.IncludeChecks);
                        command.Parameters.AddWithValue("@ChecksPerMonth", preOrder.ChecksPerMonth);
                        command.Parameters.AddWithValue("@IsPaymentMade", preOrder.IsPaymentMade);

                        int? lastInsertedId = Convert.ToInt32(await command.ExecuteScalarAsync());

                        if (lastInsertedId != null)
                        {
                            response.Data = lastInsertedId.Value;
                            response.IsSuccess = true;
                        }
                        else
                        {
                            response.IsSuccess = false;
                            response.ErrorMessage = "Failed to insert PreOrder.";
                            _logger.LogError("Failed to insert PreOrder.");
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

        public async Task<ServiceResponse<bool>> UpdatePayment(int preOrderId, PaymentDTO paymentDto)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = @"UPDATE zb.PreOrder SET IsPaymentMade = @IsPaymentMade, PaymentId = @PaymentId WHERE ID = @PreOrderId";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@IsPaymentMade", paymentDto.IsPaymentMade);
                        command.Parameters.AddWithValue("@PaymentId", paymentDto.PaymentId);
                        command.Parameters.AddWithValue("@PreOrderId", preOrderId);

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        response.IsSuccess = rowsAffected > 0;
                        response.Data = response.IsSuccess;
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

        public async Task<ServiceResponse<bool>> Update(PreOrder preOrder)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = @"
                        UPDATE zb.PreOrder SET 
                            BusinessId = @BusinessId, 
                            Platform = @Platform, 
                            QBDesktopVersion = @QBDesktopVersion, 
                            ExistingAccounting = @ExistingAccounting, 
                            LatestAccountingDate = @LatestAccountingDate, 
                            NumBankAccounts = @NumBankAccounts, 
                            NumCreditCards = @NumCreditCards, 
                            IncludeChecks = @IncludeChecks, 
                            ChecksPerMonth = @ChecksPerMonth, 
                            IsPaymentMade = @IsPaymentMade
                        WHERE ID = @ID";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@BusinessId", preOrder.BusinessId);
                        command.Parameters.AddWithValue("@Platform", preOrder.Platform);
                        command.Parameters.AddWithValue("@QBDesktopVersion", preOrder.QBDesktopVersion ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@ExistingAccounting", preOrder.ExistingAccounting);
                        command.Parameters.AddWithValue("@LatestAccountingDate", preOrder.LatestAccountingDate);
                        command.Parameters.AddWithValue("@NumBankAccounts", preOrder.NumBankAccounts);
                        command.Parameters.AddWithValue("@NumCreditCards", preOrder.NumCreditCards);
                        command.Parameters.AddWithValue("@IncludeChecks", preOrder.IncludeChecks);
                        command.Parameters.AddWithValue("@ChecksPerMonth", preOrder.ChecksPerMonth);
                        command.Parameters.AddWithValue("@IsPaymentMade", preOrder.IsPaymentMade);
                        command.Parameters.AddWithValue("@ID", preOrder.ID);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        response.IsSuccess = rowsAffected > 0;
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

                    string sql = "UPDATE zb.PreOrder SET IsActive = 0 WHERE ID = @ID";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ID", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        response.IsSuccess = rowsAffected > 0;
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