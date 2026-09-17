using Grpc.Contracts;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using ClientEmp.Models;

namespace ClientEmp.Services;

public class DepartmentService
{
    private readonly GrpcChannel _channel;
    private readonly Employees.EmployeesClient _client;

    public DepartmentService(IConfiguration configuration)
    {
        var channel = GrpcChannel.ForAddress(configuration["GrpcServiceUrl"]);
        _client = new Employees.EmployeesClient(channel);
    }

    public async Task<ResponceDepartment> CreateDepartment(RequestData request)
    {
        
        var response = await _client.CreateDepartmentAsync(request);
        return response;
    }
}