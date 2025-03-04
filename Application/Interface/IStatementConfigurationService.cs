using Core.Model;

namespace Application.Interface
{
    public interface IStatementConfigurationService
    {
        Task<ServiceResponse<StatementConfiguration>> GetByPlatformAccountId(int platformAccountId);
        Task<ServiceResponse<int?>> PostStatementConfiguration(StatementConfigurationDTO statementConfigurationDTO);
        Task<ServiceResponse<int?>> PostColumnConfiguration(ColumnConfigurationDTO columnConfigurationDTO);
    }
}