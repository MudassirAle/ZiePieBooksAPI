using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface
{
    public interface IOnboardingService
    {
        Task<ServiceResponse<Onboarding>> GetByBusinessID(int businessId);
        Task<ServiceResponse<int?>> Post(OnboardingDTO onboardingDTO);
        Task<ServiceResponse<bool>> UpdateDataSource(DataSourceDTO dataSourceDTO);
        Task<ServiceResponse<List<OnboardingForAdmin>>> GetForAdmin();
        Task<ServiceResponse<bool>> UpdateAssignedToId(AssignmentDTO assignmentDTO);
        Task<ServiceResponse<List<OnboardingForSubAdmin>>> GetForSubAdmin(int assignedToId);

    }
}
