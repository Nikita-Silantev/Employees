using Server.Repositories;
using Server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddScoped<EmployeeRepository>();

var app = builder.Build();

app.MapGrpcService<EmployeeService>();

app.Run();