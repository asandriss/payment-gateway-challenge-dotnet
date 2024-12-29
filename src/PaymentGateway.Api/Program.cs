using PaymentGateway.Abstraction;
using PaymentGateway.Api.Middleware;
using PaymentGateway.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IApiKeysRepository, ApiKeysRepository>();
builder.Services.AddSingleton<ICurrencyProvider, CurrencyProvider>();
builder.Services.AddSingleton<IPaymentsRepository, PaymentsRepository>();

builder.Services.AddTransient<IPaymentProcessor, PaymentProcessorService>();
builder.Services.AddTransient<IBank, SimulatorBank>();
builder.Services.AddHttpClient<SimulatorBank>();

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
