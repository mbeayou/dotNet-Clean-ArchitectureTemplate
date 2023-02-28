using Anis.Template.Application;
using Anis.Template.Grpc.ExceptionHandler;
using Anis.Template.Grpc.Interceptors;
using Anis.Template.Grpc.Services;
using Anis.Template.Infra.Services.Logger;
using Anis.Template.Infrastructure;
using Anis.Template.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
Log.Logger = LoggerServiceBuilder.Build();

builder.Host.UseSerilog();
builder.Services.RegisterInfraService(builder.Configuration);
builder.Services.RegisterAppServices();

builder.Services.AddGrpc(option =>
{
    option.Interceptors.Add<ThreadCultureInterceptor>();
    option.EnableMessageValidation();
    option.Interceptors.Add<ExceptionHandlingInterceptor>();
});
builder.Services.AddGrpcValidators();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<AppDbContext>();

    context.Database.Migrate();
}
// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();


public partial class Program { }