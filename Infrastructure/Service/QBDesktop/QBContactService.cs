using Application.Interface.QBDesktop;
using Core.Model;
using Core.Model.QBDesktop;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;

namespace Infrastructure.Service.QBDesktop
{
    public class QBContactService : IQBContactService
    {
        private readonly string _connectionString;
        private readonly ILogger<QBContactService> _logger;

        public QBContactService(IConfiguration configuration, ILogger<QBContactService> logger)
        {
            _connectionString = configuration.GetConnectionString("SAP")
                ?? throw new ArgumentNullException(nameof(configuration), "Configuration or SAP connection string is null");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger is null");
        }

        public async Task<ServiceResponse<List<QBContact>>> GetByTicket(string ticket)
        {
            var response = new ServiceResponse<List<QBContact>>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = @"
                        SELECT Name, 'Customer' AS Type, IsActive FROM dbo.WCCustomer WHERE Ticket = @Ticket
                        UNION ALL
                        SELECT Name, 'Employee' AS Type, IsActive FROM dbo.WCEmployee WHERE Ticket = @Ticket
                        UNION ALL
                        SELECT Name, 'Vendor' AS Type, IsActive FROM dbo.WCVendor WHERE Ticket = @Ticket";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Ticket", ticket);

                        using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
                        {
                            List<QBContact> contacts = new List<QBContact>();

                            while (await dataReader.ReadAsync())
                            {
                                QBContact contact = new QBContact
                                {
                                    Name = dataReader["Name"]?.ToString() ?? string.Empty,
                                    Type = dataReader["Type"]?.ToString() ?? string.Empty,
                                    IsActive = dataReader["IsActive"]?.ToString() ?? string.Empty
                                };
                                contacts.Add(contact);
                            }

                            response.Data = contacts;
                            response.IsSuccess = true;
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
    }
}