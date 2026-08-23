using Grpc.Contracts;
using Grpc.Core;
using Server.Models;
using Server.Repositories;

namespace Server.Services;

public class EmployeeService : Employees.EmployeesBase
{
    public override async Task<ResponceDepartment> CreateDepartment(RequestData request, ServerCallContext context)
    {
        var responce = new ResponceDepartment();

        var department = new Department
        {
            Id = 0,
            Name = request.Name
        };

        var repository = new EmployeeRepository();

        var createdDepartment = await repository.CreateDepartment(department);

        if (createdDepartment != null)
        {
            responce.Description = "Successfully Created Department";
        }
        else
        {
            responce.Description = "Failed Create Department";
        }

        return responce;
    }
}