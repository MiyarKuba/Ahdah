using Ahdah.Api.Authentication;
using Ahdah.Api.ErrorHandling;
using Ahdah.Infrastructure;
using Ahdah.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddPolicy(AhdahCorsPolicies.FlutterClient, policy =>
    {
        var allowedOrigins = builder.Configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>() ?? [];

        if (allowedOrigins.Length > 0)
        {
            policy
                .WithOrigins(allowedOrigins)
                .WithMethods("GET", "POST", "PATCH", "PUT", "OPTIONS")
                .WithHeaders("Authorization", "Content-Type", "Accept", "Idempotency-Key");
        }
    });
});
builder.Services.AddAhdahPersistence(builder.Configuration);
builder.Services.AddAhdahIdentity();
builder.Services.AddAhdahAccess(builder.Configuration);
builder.Services.AddAhdahCompanyStructure();
builder.Services.AddAhdahAdvances();
builder.Services.AddAhdahExpenses();
builder.Services.AddAhdahSuppliers();
builder.Services.AddAhdahAuthentication(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
    app.MapOpenApi();
}

app.UseCors(AhdahCorsPolicies.FlutterClient);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program;

public static class AhdahCorsPolicies
{
    public const string FlutterClient = "FlutterClient";
}
