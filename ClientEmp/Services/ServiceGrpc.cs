using System;
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

    public ServiceGrpc(Employees.EmployeesClient client)
    {
        _client =  client;
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

    public async Task<Department> CreateDepartment(string name)
    {
        try
        {
            var request = new RequestData();
            request.Name = name;
            var responce = _client.CreateDepartment(request);
            var createdDepartment = new Department();
            createdDepartment.Id = responce.Id;
            createdDepartment.Name = responce.Name;
            return createdDepartment;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}