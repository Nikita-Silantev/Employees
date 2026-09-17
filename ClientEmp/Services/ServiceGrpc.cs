using System.Collections.Generic;
using Grpc.Contracts;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using ClientEmp.Models;

namespace ClientEmp.Services;

public class ServiceGrpc
{
    private readonly Employees.EmployeesClient _client;

    public ServiceGrpc()
    {
        var channel = GrpcChannel.ForAddress("https://localhost:7077");
        _client = new Employees.EmployeesClient(channel);
    }

    public async Task<List<Department>> GetAllDepartment()
    {
        var responce = await _client.GetAllDepartmentsAsync(new EmptyRequest());
        var allDepartments = new List<Department>();
        foreach (var i in responce.Departments)
        {
            allDepartments.Add(new Department()
            {
                Id = i.Id,
                Name = i.Name
            });
        }
        return allDepartments;
    }
}