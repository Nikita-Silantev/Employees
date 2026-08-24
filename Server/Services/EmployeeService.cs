using Grpc.Contracts;
using Grpc.Core;
using Server.Models;
using Server.Repositories;

namespace Server.Services;

public class EmployeeService : Employees.EmployeesBase
{
    #region отделы

    /// <summary>
    /// Создание отдела
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
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

    /// <summary>
    /// выдать все отделы
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task<AllDepartment> GetAllDepartments(EmptyRequest request, ServerCallContext context)
    {
        var response = new AllDepartment();
        var rep = new EmployeeRepository();
        var departments = await rep.GetAllDepartment();

        foreach (Department d in departments)
        {
            response.Departments.Add(new DepartmentItem
            {
                Id = d.Id,
                Name = d.Name
            });
        }

        return response;
    }

    /// <summary>
    /// обновить отдел по id 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task<UpdatetDepartment> UpdateDepartment(ForUpdateDataDepartment request,
        ServerCallContext context)
    {
        var response = new UpdatetDepartment();
        var depatrment = new Department();
        depatrment.Id = request.Id;
        depatrment.Name = request.Name;

        var repos = new EmployeeRepository();
        var updated_department = await repos.UpdateDepartment(depatrment);

        response.Id = updated_department.Id;
        response.Name = updated_department.Name;

        return response;
    }

    /// <summary>
    /// удалить отдел по Id
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task<StatusDelete> DeleteDepartment(ForDeleteDepartment request, ServerCallContext context)
    {
        var response = new StatusDelete();
        int id = request.Id;
        var repos = new EmployeeRepository();
        var stat = await repos.DeleteDepartment(id);
        if (stat == "OK")
        {
            response.Status = "Succsessful Delete Department";
        }
        else
        {
            response.Status = "Failed Delete Department";
        }

        return response;
    }

    #endregion
}