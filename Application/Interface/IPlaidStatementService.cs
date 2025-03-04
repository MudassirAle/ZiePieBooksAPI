using Core.Model.Plaid;
using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface
{
    public interface IPlaidStatementService
    {
        Task<ServiceResponse<PlaidStatement>> GetByStatementId(string statementId);
        Task<ServiceResponse<List<PlaidStatement>>> GetByAccountId(int businessId);
        Task<ServiceResponse<int?>> Post(PlaidStatement plaidStatement);
        Task<ServiceResponse<bool>> Update(PlaidStatement plaidStatement);
        Task<ServiceResponse<bool>> Delete(int id);
    }
}
