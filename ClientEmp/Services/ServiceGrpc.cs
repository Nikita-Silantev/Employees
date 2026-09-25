using System;
using System.Collections.Generic;
using System.Globalization;
using Grpc.Contracts;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using ClientEmp.Models;
using ClientEmp.Models;

namespace ClientEmp.Services;

public class ServiceGrpc
{
    private readonly Employees.EmployeesClient _client;

    public ServiceGrpc(Employees.EmployeesClient client)
    {
        _client = client;
    }

    #region Departments

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

    #endregion

    #region Tasks

    /// <summary>
    /// Создание задачи в gprc на сервере
    /// </summary>
    /// <param name="name"></param>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <returns></returns>
    public async Task<EmpTask> CreateTask(string name, DateTime startDate, DateTime endDate)
    {
        var request = new NewTask();
        request.Name = name;
        request.DateStarted = startDate.ToString("yyyy-MM-dd");
        request.DateEnd = endDate.ToString("yyyy-MM-dd");
        var responce = await _client.CreateTaskAsync(request);
        var created = new EmpTask();
        created.Id = responce.Id;
        created.Name = responce.Name;
        created.Date_Started = DateTime.Parse(responce.DateStarted);
        created.Date_End = DateTime.Parse(responce.DateEnd);
        return created;
    }

    /// <summary>
    /// Gprc запрос на все задачи
    /// </summary>
    /// <returns></returns>
    public async Task<List<EmpTask>> GetAllTasks()
    {
        var responce = await _client.GetAllTasksAsync(new EmptyRequest());
        var Tasks = new List<EmpTask>();
        foreach (var t in responce.AllTasks_)
        {
            Tasks.Add(new EmpTask()
            {
                Id = t.Id,
                Name = t.Name,
                Date_Started = DateTime.Parse(t.DateStarted),
                Date_End = DateTime.Parse(t.DateEnd)
            });
        }

        return Tasks;
    }

    /// <summary>
    /// Обновить по ID задачу
    /// </summary>
    /// <param name="task"></param>
    /// <returns></returns>
    public async Task<EmpTask> UpdateTask(EmpTask task)
    {
        var request = new NewTaskData();
        request.Id = task.Id;
        request.Name = task.Name;
        request.DateStarted = task.Date_Started?.ToString("yyyy-MM-dd");
        request.DateEnd = task.Date_End?.ToString("yyyy-MM-dd");
        var responce = await _client.UpdateTaskAsync(request);
        var updated = new EmpTask();
        updated.Id = responce.Id;
        updated.Name = responce.Name;
        updated.Date_Started = DateTime.Parse(responce.DateStarted);
        updated.Date_End = DateTime.Parse(responce.DateEnd);
        return updated;
    }

    /// <summary>
    /// Удаление задачи
    /// </summary>
    /// <param name="task"></param>
    /// <returns></returns>
    public async Task<string> DeleteTask(EmpTask task)
    {
        var request = new DeletedTask()
        {
            Id = task.Id,
            Name = task.Name,
            DateStarted = task.Date_Started?.ToString("yyyy-MM-dd"),
            DateEnd = task.Date_End?.ToString("yyyy-MM-dd")
        };
        var responce = await _client.DeleteTaskAsync(request);
        string message = responce.Message;
        return message;
    }

    #endregion

    #region Post

    /// <summary>
    /// Создание должности
    /// </summary>
    /// <param name="name"></param>
    /// <param name="salary"></param>
    public async Task CreatePost(string name, int salary)
    {
        var request = new NewPost()
        {
            Name = name,
            Salary = salary
        };
        var responce = await _client.CreatePostAsync(request);
    }

    /// <summary>
    /// Запрос на все отделы
    /// </summary>
    /// <returns></returns>
    public async Task<List<Post>> GetAllPosts()
    {
        var request = new EmptyRequest();
        var responce = await _client.GetAllPostsAsync(request);
        var result = new List<Post>();
        foreach (var i in responce.Posts)
        {
            result.Add(new Post()
            {
                Id = i.Id,
                Name = i.Name,
                Salary = i.Salary
            });
        }

        return result;
    }

    /// <summary>
    /// Обновление должности
    /// </summary>
    /// <param name="post"></param>
    /// <returns></returns>
    public async Task<Post> UpdatePost(Post post)
    {
        var request = new NewVersionPost()
        {
            Id = post.Id,
            Name = post.Name,
            Salary = post.Salary
        };

        var responce = await _client.UpdatePostAsync(request);
        var result = new Post();
        result.Id = responce.Id;
        result.Name = responce.Name;
        result.Salary = responce.Salary;
        return result;
    }

    public async Task<string> DeletePost(Post post)
    {
        var request = new DeletedPost()
        {
            Id = post.Id,
            Name = post.Name,
            Salary = post.Salary
        };
        var responce = await _client.DeletePostAsync(request);
        var result = responce.Message;
        return result;
    }
    #endregion
}