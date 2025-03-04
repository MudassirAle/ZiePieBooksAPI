using Application.Interface;
using Core.Model;
using Core.Model.Plaid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;

namespace Infrastructure.Service
{
    public class PlaidInstitutionService : IPlaidInstitutionService
    {
        private readonly string _connectionString;
        private readonly ILogger<PlaidInstitutionService> _logger;

        public PlaidInstitutionService(IConfiguration configuration, ILogger<PlaidInstitutionService> logger)
        {
            _connectionString = configuration.GetConnectionString("SAPDev") ?? throw new ArgumentNullException(nameof(configuration), "Configuration or SAP connection string is null");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger is null");
        }

        public async Task<ServiceResponse<PlaidInstitution>> GetByInstitutionId(string institutionId)
        {
            var response = new ServiceResponse<PlaidInstitution>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "SELECT * FROM zb.PlaidInstitution WHERE InstitutionId = @InstitutionId";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@InstitutionId", institutionId);

                        using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
                        {
                            if (await dataReader.ReadAsync())
                            {
                                PlaidInstitution plaidInstitution = new PlaidInstitution
                                {
                                    InstitutionId = dataReader["InstitutionId"].ToString() ?? string.Empty,
                                    Name = dataReader["Name"].ToString() ?? string.Empty,
                                    Oauth = Convert.ToBoolean(dataReader["Oauth"]),
                                    CountryCodes = JsonConvert.DeserializeObject<List<string>>(dataReader["CountryCodes"].ToString() ?? "[]") ?? new List<string>(),
                                    DtcNumbers = JsonConvert.DeserializeObject<List<string>>(dataReader["DtcNumbers"].ToString() ?? "[]") ?? new List<string>(),
                                    Products = JsonConvert.DeserializeObject<List<string>>(dataReader["Products"].ToString() ?? "[]") ?? new List<string>(),
                                    RoutingNumbers = JsonConvert.DeserializeObject<List<string>>(dataReader["RoutingNumbers"].ToString() ?? "[]") ?? new List<string>()
                                };
                                response.Data = plaidInstitution;
                                response.IsSuccess = true;
                            }
                            else
                            {
                                response.ErrorMessage = "PlaidInstitution not found.";
                                _logger.LogError("PlaidInstitution not found.");
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

        //public async Task<ServiceResponse<int?>> Post(PlaidInstitution plaidInstitution)
        //{
        //    var response = new ServiceResponse<int?>();
        //    try
        //    {
        //        using (SqlConnection connection = new SqlConnection(_connectionString))
        //        {
        //            await connection.OpenAsync();

        //            string sql = "INSERT INTO zb.PlaidInstitution (InstitutionId, Name, Oauth, CountryCodes, DtcNumbers, Products, RoutingNumbers) " +
        //                         "VALUES (@InstitutionId, @Name, @Oauth, @CountryCodes, @DtcNumbers, @Products, @RoutingNumbers); " +
        //                         "SELECT SCOPE_IDENTITY();";

        //            using (SqlCommand command = new SqlCommand(sql, connection))
        //            {
        //                command.Parameters.AddWithValue("@InstitutionId", plaidInstitution.InstitutionId);
        //                command.Parameters.AddWithValue("@Name", plaidInstitution.Name);
        //                command.Parameters.AddWithValue("@Oauth", plaidInstitution.Oauth);
        //                command.Parameters.AddWithValue("@CountryCodes", JsonConvert.SerializeObject(plaidInstitution.CountryCodes));
        //                command.Parameters.AddWithValue("@DtcNumbers", JsonConvert.SerializeObject(plaidInstitution.DtcNumbers));
        //                command.Parameters.AddWithValue("@Products", JsonConvert.SerializeObject(plaidInstitution.Products));
        //                command.Parameters.AddWithValue("@RoutingNumbers", JsonConvert.SerializeObject(plaidInstitution.RoutingNumbers));

        //                int? lastInsertedId = Convert.ToInt32(await command.ExecuteScalarAsync());

        //                if (lastInsertedId != null)
        //                {
        //                    response.Data = lastInsertedId;
        //                    response.IsSuccess = true;
        //                }
        //                else
        //                {
        //                    response.ErrorMessage = "Failed to insert PlaidInstitution.";
        //                    _logger.LogError("Failed to insert PlaidInstitution.");
        //                }
        //            }
        //        }
        //    }
        //    catch (SqlException sqlEx)
        //    {
        //        response.IsSuccess = false;
        //        _logger.LogError($"SQL Error: {sqlEx.Message}");
        //        response.ErrorMessage = $"SQL Error: {sqlEx.Message}";
        //    }
        //    catch (Exception ex)
        //    {
        //        response.IsSuccess = false;
        //        _logger.LogError($"An error occurred: {ex.Message}");
        //        response.ErrorMessage = $"An error occurred: {ex.Message}";
        //    }
        //    return response;
        //}

        public async Task<ServiceResponse<bool>> PostBulk(List<PlaidInstitution> plaidInstitutions)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            using (var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
                            {
                                ConfigureBulkCopy(bulkCopy);

                                var table = CreateDataTable();
                                foreach (var institution in plaidInstitutions)
                                {
                                    AddInstitutionToTable(table, institution);
                                }

                                await bulkCopy.WriteToServerAsync(table);
                            }

                            transaction.Commit();
                            response.Data = true;
                            response.IsSuccess = true;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            _logger.LogError(ex, "Error occurred while saving plaid institutions");
                            response.Data = false;
                            response.IsSuccess = false;
                            response.ErrorMessage = ex.Message;
                        }
                    }
                }
            }
            catch (SqlException sqex)
            {
                _logger.LogError(sqex, "SQL error occurred while saving plaid institutions");
                response.Data = false;
                response.IsSuccess = false;
                response.ErrorMessage = sqex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while saving plaid institutions");
                response.Data = false;
                response.IsSuccess = false;
                response.ErrorMessage = ex.Message;
            }

            return response;
        }

        private void ConfigureBulkCopy(SqlBulkCopy bulkCopy)
        {
            bulkCopy.DestinationTableName = "zb.PlaidInstitution";
            bulkCopy.ColumnMappings.Add("InstitutionId", "InstitutionId");
            bulkCopy.ColumnMappings.Add("Name", "Name");
            bulkCopy.ColumnMappings.Add("Oauth", "Oauth");
            bulkCopy.ColumnMappings.Add("CountryCodes", "CountryCodes");
            bulkCopy.ColumnMappings.Add("DtcNumbers", "DtcNumbers");
            bulkCopy.ColumnMappings.Add("Products", "Products");
            bulkCopy.ColumnMappings.Add("RoutingNumbers", "RoutingNumbers");
        }

        private DataTable CreateDataTable()
        {
            var table = new DataTable();
            table.Columns.Add("InstitutionId", typeof(string));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Oauth", typeof(bool));
            table.Columns.Add("CountryCodes", typeof(string));
            table.Columns.Add("DtcNumbers", typeof(string));
            table.Columns.Add("Products", typeof(string));
            table.Columns.Add("RoutingNumbers", typeof(string));
            return table;
        }

        private void AddInstitutionToTable(DataTable table, PlaidInstitution institution)
        {
            var row = table.NewRow();
            row["InstitutionId"] = institution.InstitutionId;
            row["Name"] = institution.Name;
            row["Oauth"] = institution.Oauth;
            row["CountryCodes"] = JsonConvert.SerializeObject(institution.CountryCodes);
            row["DtcNumbers"] = JsonConvert.SerializeObject(institution.DtcNumbers);
            row["Products"] = JsonConvert.SerializeObject(institution.Products);
            row["RoutingNumbers"] = JsonConvert.SerializeObject(institution.RoutingNumbers);
            table.Rows.Add(row);
        }

    }
}