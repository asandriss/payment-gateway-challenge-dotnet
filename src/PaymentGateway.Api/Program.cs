using PaymentGateway.Abstraction;
using PaymentGateway.Api.Middleware;
using PaymentGateway.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<SimulatorBank>();
builder.Services.AddSingleton<IApiKeysRepository, ApiKeysRepository>();
builder.Services.AddSingleton<ICurrencyProvider, CurrencyProvider>();
builder.Services.AddSingleton<IPaymentsRepository, PaymentsRepository>();

builder.Services.AddSingleton<IPaymentProcessor, PaymentProcessorService>();
builder.Services.AddSingleton<IBank, SimulatorBank>();


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File("logs/app-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File("logs/app-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<ApiKeyValidationMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
