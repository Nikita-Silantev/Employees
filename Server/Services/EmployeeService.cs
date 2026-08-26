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

    #region должности

    /// <summary>
    /// Создание должности
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task<CreatedPost> CreatePost(NewPost request, ServerCallContext context)
    {
        var response = new CreatedPost();
        var temp_response = new Post();
        var newPost = new Post();

        newPost.Id = 0;
        newPost.Name = request.Name;
        newPost.Salary = request.Salary;

        var repos = new EmployeeRepository();

        temp_response = await repos.CreatePost(newPost);
        response.Id = temp_response.Id;
        response.Name = temp_response.Name;
        response.Salary = temp_response.Salary;
        return response;
    }

    /// <summary>
    /// Выдать все должности
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task<AllPosts> GetAllPosts(EmptyRequest request, ServerCallContext context)
    {
        var response = new AllPosts();
        var repos = new EmployeeRepository();
        var temp_response = await repos.GetAllPosts();

        foreach (Post p in temp_response)
        {
            response.Posts.Add(new Posts
            {
                Id = p.Id,
                Name = p.Name,
                Salary = p.Salary
            });
        }

        return response;
    }

    /// <summary>
    /// Обновить должность по Id
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public override async Task<UpdatedPost> UpdatePost(NewVersionPost request, ServerCallContext context)
    {
        var response = new UpdatedPost();
        var repos = new EmployeeRepository();
        Console.WriteLine($"raw request: Id={request.Id}, Name='{request.Name}', Salary={request.Salary}");
        var update_needPost = new Post
        {
            Id = request.Id,
            Name = request.Name,
            Salary = request.Salary
        };

        var updatedPost = await repos.UpdatePost(update_needPost);

        if (updatedPost == null)
        {
            throw new Exception("Post not found");
        }

        response.Id = updatedPost.Id;
        response.Name = updatedPost.Name;
        response.Salary = updatedPost.Salary;

        return response;
    }

    /// <summary>
    /// Удаление должности
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task<MessageDeletePost> DeletePost(DeletedPost request, ServerCallContext context)
    {
        var response = new MessageDeletePost();
        var needDeletePost = new Post();
        needDeletePost.Id = request.Id;
        needDeletePost.Name = request.Name;
        needDeletePost.Salary = request.Salary;

        try
        {
            var rep = new EmployeeRepository();

            var deletedPost = await rep.DeletePost(needDeletePost.Id);
            response.Message = "Succsessful!";
            return response;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    #endregion
}