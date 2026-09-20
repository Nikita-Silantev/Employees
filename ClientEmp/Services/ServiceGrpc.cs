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
        _client = client;
    }

    /// <summary>
    /// запрос на все отделы
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// запрос на создание отдела
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public async Task<Department> CreateDepartment(string name)
    {
        try
        {
            var request = new RequestData();
            request.Name = name;
            var responce = await _client.CreateDepartmentAsync(request);
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

    /// <summary>
    /// запрос на удаление отдела
    /// </summary>
    /// <param name="department"></param>
    /// <returns></returns>
    public async Task<StatusDelete> DeleteDepartment(Department department)
    {
        try
        {
            var responce = new StatusDelete();
            var request = new ForDeleteDepartment();
            request.Id = department.Id;
            responce = await _client.DeleteDepartmentAsync(request);
            return responce;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    /// <summary>
    /// запрос на обновление одела
    /// </summary>
    /// <param name="department"></param>
    /// <returns></returns>
    public async Task<UpdatetDepartment> UpdateDepartment(Department department)
    {
        try
        {
            var request = new ForUpdateDataDepartment();
            request.Id = department.Id;
            request.Name = department.Name;
            UpdatetDepartment responce = await _client.UpdateDepartmentAsync(request);
            return responce;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}