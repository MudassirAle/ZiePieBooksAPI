using Application.Interface.QBDesktop;
using Application.Interface;
using Infrastructure.Service.QBDesktop;
using Infrastructure.Service;

namespace ZiePieBooksAPI.Helper
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Scoped services
            services.AddScoped<IAppUserService, AppUserService>();
            services.AddScoped<IEmailService, EmailService>();

            // Singleton services
            services.AddSingleton<ITokenService, TokenService>();
            services.AddSingleton<IAdminService, AdminService>();
            services.AddSingleton<ISubAdminService, SubAdminService>();
            services.AddSingleton<ITenantService, TenantService>();
            services.AddSingleton<ISubTenantService, SubTenantService>();
            services.AddSingleton<ICustomerService, CustomerService>();
            services.AddSingleton<IBusinessService, BusinessService>();
            services.AddSingleton<IPlaidAccountService, PlaidAccountService>();
            services.AddSingleton<IPlaidTransactionService, PlaidTransactionService>();
            services.AddSingleton<IPlaidStatementService, PlaidStatementService>();
            services.AddSingleton<IPlaidInstitutionService, PlaidInstitutionService>();
            services.AddSingleton<IAccountService, AccountService>();
            services.AddSingleton<IOnboardingService, OnboardingService>();
            services.AddSingleton<IServicesService, ServicesService>();
            services.AddSingleton<IStatementConfigurationService, StatementConfigurationService>();
            services.AddSingleton<IQBFileSyncService, QBFileSyncService>();
            services.AddSingleton<IWCOutboxService, WCOutboxService>();
            services.AddSingleton<IQBManualAccountService, QBManualAccountService>();
            services.AddSingleton<IQBAccountService, QBAccountService>();
            services.AddSingleton<IQBContactService, QBContactService>();
            services.AddSingleton<IQBTransactionService, QBTransactionService>();
            services.AddSingleton<IQBTransactionReportService, QBTransactionReportService>();
            services.AddSingleton<IPlaidSyncService, PlaidSyncService>();
            services.AddSingleton<IPreOrderService, PreOrderService>();
            services.AddSingleton<ITaskService, TaskService>();

            return services;
        }
    }
}
