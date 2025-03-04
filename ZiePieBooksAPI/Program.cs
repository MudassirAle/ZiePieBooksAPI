using Azure.Identity;
using Core.Model;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using Quartz;
using ZiePieBooksAPI.Helper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(o =>
{
    o.DefaultAuthenticateScheme = "Bearer";
    o.DefaultChallengeScheme = "Bearer";
}).AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAdB2C"))
        .EnableTokenAcquisitionToCallDownstreamApi()
        .AddInMemoryTokenCaches()
        .AddMicrosoftGraph(x =>
        {
            string tenantId = builder.Configuration.GetValue<string>("GraphApi:TenantId")!;
            string clientId = builder.Configuration.GetValue<string>("GraphApi:ClientId")!;
            string ClientSecret = builder.Configuration.GetValue<string>("GraphApi:ClientSecret")!;
            ClientSecretCredential clientSecretCredential = new ClientSecretCredential(tenantId, clientId, ClientSecret);
            return new GraphServiceClient(clientSecretCredential);
        }, new string[] { ".default" });


//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("ReadPolicy", policy =>
//        policy.RequireClaim("scp", "Hajj.Read"));
//    options.AddPolicy("WritePolicy", policy =>
//        policy.RequireClaim("scp", "Hajj.Write"));
//});

//builder.Services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
//{
//    options.Events = new JwtBearerEvents
//    {
//        OnAuthenticationFailed = context =>
//        {
//            Console.WriteLine($"Authentication failed: {context.Exception.Message}");
//            return Task.CompletedTask;
//        },
//        OnTokenValidated = context =>
//        {
//            Console.WriteLine("Token validated successfully.");
//            return Task.CompletedTask;
//        }
//    };
//});

builder.Services.Configure<MailSetting>(builder.Configuration.GetSection("MailSetting"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("ZiePieBooksCorsPolicy", builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Add custom services using the extension method
builder.Services.AddApplicationServices();

builder.Services.AddQuartz(options =>
{
    // Register the job
    //options.UseMicrosoftDependencyInjectionJobFactory();

    var jobKey = JobKey.Create(nameof(PlaidSyncJob));

    //options
    //  .AddJob<PlaidSyncJob>(jobKey)
    //  .AddTrigger(trigger =>
    //    trigger
    //    .ForJob(jobKey)
    //    .WithCronSchedule("0 0 0 * * ?") // Cron expression: daily at midnight
    //  );

    options
      .AddJob<PlaidSyncJob>(jobKey)
      .AddTrigger(trigger =>
        trigger
          .ForJob(jobKey)
          .WithSimpleSchedule(schedule => schedule
            .WithIntervalInHours(24)
            .RepeatForever())
      );

    //options
    //  .AddJob<PlaidSyncJob>(jobKey)
    //  .AddTrigger(trigger =>
    //    trigger
    //      .ForJob(jobKey)
    //      .WithSimpleSchedule(schedule => schedule
    //        .WithIntervalInMinutes(30)
    //        .RepeatForever())
    //  );
});

builder.Services.AddQuartzHostedService(options =>
{
    options.WaitForJobsToComplete = true; // Graceful shutdown
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("ZiePieBooksCorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

//app.UseMiddleware<TokenValidationMiddleware>();

app.MapControllers();

app.Run();
