using Infrastructure.Contexts;
using Infrastructure.Entities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((context, services) =>
    {
    services.AddApplicationInsightsTelemetryWorkerService();
    services.ConfigureFunctionsApplicationInsights();

    services.AddDbContext<DataContext>(x => x.UseSqlServer(context.Configuration.GetConnectionString("AccountDatabase")));

    services.AddDefaultIdentity<UserEntity>(x =>
    {
        x.SignIn.RequireConfirmedAccount = true;
        x.User.RequireUniqueEmail = true;
        x.Password.RequiredLength = 8;

    }).AddEntityFrameworkStores<DataContext>();

        services.AddAuthentication();
        services.AddAuthorization();
    })
    .Build();

host.Run();
