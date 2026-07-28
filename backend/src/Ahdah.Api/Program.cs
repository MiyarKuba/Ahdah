using Ahdah.Api.Authentication;
using Ahdah.Api.ErrorHandling;
using Ahdah.Infrastructure;
using Ahdah.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.AddOpenApi();
builder.Services.AddAhdahPersistence(builder.Configuration);
builder.Services.AddAhdahIdentity();
builder.Services.AddAhdahAccess(builder.Configuration);
builder.Services.AddAhdahAuthentication(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
    app.MapOpenApi();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program;
