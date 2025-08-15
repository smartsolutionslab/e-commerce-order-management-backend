using E_Commerce.Common.Api.Extensions;
using E_Commerce.OrderManagement.Api.Endpoints;
using E_Commerce.OrderManagement.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Common services
builder.Services.AddCommonServices();
builder.Services.AddMultiTenancy();
builder.Services.AddApiVersioning();

// Order management specific services
builder.Services.AddOrderManagementInfrastructure(builder.Configuration);

// Health checks
builder.Services.AddHealthChecks();

// API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Add order endpoints
app.MapOrderEndpoints();

// Health check
app.MapHealthChecks("/health");

app.Run();
