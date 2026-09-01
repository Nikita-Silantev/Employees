using Grpc.Contracts;
using Grpc.Core;
using Server.Models;
using Server.Repositories;
using System.Globalization;
using Google.Protobuf.WellKnownTypes;

namespace Server.Services;

public class EmployeeService : Employees.EmployeesBase
{
    private readonly EmployeeRepository _repository;

    public EmployeeService(EmployeeRepository repository)
    {
        _repository = repository;
    }

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


        var createdDepartment = await _repository.CreateDepartment(department);

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
        var departments = await _repository.GetAllDepartment();

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

        var updated_department = await _repository.UpdateDepartment(depatrment);

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
        var stat = await _repository.DeleteDepartment(id);
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


        temp_response = await _repository.CreatePost(newPost);
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
        var temp_response = await _repository.GetAllPosts();

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
        var update_needPost = new Post
        {
            Id = request.Id,
            Name = request.Name,
            Salary = request.Salary
        };

        var updatedPost = await _repository.UpdatePost(update_needPost);

        if (updatedPost == null)
        {
            Console.WriteLine("Похоже в сервис пришло 0");
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
            var deletedPost = await _repository.DeletePost(needDeletePost.Id);
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

    #region задачи

    /// <summary>
    /// Создать задачу
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task<CreatedTask> CreateTask(NewTask request, ServerCallContext context)
    {
        var response = new CreatedTask();
        var tempTask = new EmpTask();
        tempTask.Id = 0;
        tempTask.Name = request.Name;
        tempTask.Date_Started = DateOnly.Parse(request.DateStarted);
        tempTask.Date_End = DateOnly.Parse(request.DateEnd);

        var createdTask = await _repository.CreateTask(tempTask);
        response.Id = createdTask.Id;
        response.Name = createdTask.Name;
        response.DateStarted = createdTask.Date_Started.ToString("yyyy-MM-dd");
        response.DateEnd = createdTask.Date_End.ToString("yyyy-MM-dd");
        return response;
    }

    /// <summary>
    /// выдать все задачи
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task<AllTasks> GetAllTasks(EmptyRequest request, ServerCallContext context)
    {
        var response = new AllTasks();
        var ListTasks = new List<EmpTask>();


        ListTasks = await _repository.GetAllTasks();

        foreach (EmpTask t in ListTasks)
        {
            response.AllTasks_.Add(new AllTaskItem
            {
                Id = t.Id,
                Name = t.Name,
                DateStarted = t.Date_Started.ToString("yyyy-MM-dd"),
                DateEnd = t.Date_End.ToString("yyyy-MM-dd")
            });
        }

        return response;
    }

    /// <summary>
    /// обносить задачу
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task<UpdatedTask> UpdateTask(NewTaskData request, ServerCallContext context)
    {
        var needUpdateTask = new EmpTask();
        needUpdateTask.Id = request.Id;
        needUpdateTask.Name = request.Name;
        needUpdateTask.Date_Started = DateOnly.Parse(request.DateStarted);
        needUpdateTask.Date_End = DateOnly.Parse(request.DateEnd);


        var updatedTask = await _repository.UpdateTask(needUpdateTask);

        var response = new UpdatedTask
        {
            Id = updatedTask.Id,
            Name = updatedTask.Name,
            DateStarted = updatedTask.Date_Started.ToString("yyyy-MM-dd"),
            DateEnd = updatedTask.Date_End.ToString("yyyy-MM-dd")
        };
        return response;
    }

    /// <summary>
    /// удалить задачу
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task<MessageDeleteTask> DeleteTask(DeletedTask request, ServerCallContext context)
    {
        var delTask = new EmpTask
        {
            Id = request.Id,
            Name = request.Name,
            Date_Started = DateOnly.Parse(request.DateStarted),
            Date_End = DateOnly.Parse(request.DateEnd)
        };

        await _repository.DeleteTask(delTask.Id);

        var response = new MessageDeleteTask();
        response.Message = "Succsessful deleted task!";
        return response;
    }

    #endregion

    #region сотрудники

    /// <summary>
    /// Создание сотрудника
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task<CreatedEmployee> CreateEmployee(NewEmployee request, ServerCallContext context)
    {
        var response = new CreatedEmployee();

        var tempEmployee = new Employee();
        tempEmployee.Id = 0;
        tempEmployee.FirstName = request.FName;
        tempEmployee.MiddleName = request.MName;
        tempEmployee.LastName = request.LName;
        tempEmployee.Date_Birth = DateOnly.Parse(request.DateBirth);
        tempEmployee.Id_Department = request.IdDepartment;
        tempEmployee.Id_Post = request.IdPost;
        tempEmployee.Id_Task = request.IdTask;
        tempEmployee.Rate = Decimal.Parse(request.Rate);


        var causeDepartment = _repository.CheckDepartment(request.IdDepartment);
        if (await causeDepartment == false)
        {
            response.Message = "Inputted department not exists!";
            return response;
        }

        var causePost = _repository.CheckPost(request.IdPost);
        if (await causePost == false)
        {
            response.Message = "Inputted post not exists!";
            return response;
        }

        var causeTask = _repository.CheckTask(request.IdTask);
        if (await causeTask == false)
        {
            response.Message = "Inputted task not exists!";
            return response;
        }

        DateOnly today = DateOnly.FromDateTime(DateTime.Today);
        var DateEmployee = DateOnly.Parse(request.DateBirth);
        if (DateEmployee > today.AddYears(-18))
        {
            response.Message = "Age little 18!";
            return response;
        }

        var createdEmployee = await _repository.CreateEmployee(tempEmployee);

        var tempResponseEmployee = new ItemCreatedEmployee();
        tempResponseEmployee.Id = createdEmployee.Id;
        tempResponseEmployee.FName = createdEmployee.FirstName;
        tempResponseEmployee.MName = createdEmployee.MiddleName;
        tempResponseEmployee.LName = createdEmployee.LastName;
        tempResponseEmployee.DateBirth = createdEmployee.Date_Birth.ToString("yyyy-MM-dd");
        tempResponseEmployee.IdDepartment = createdEmployee.Id_Department;
        tempResponseEmployee.IdPost = createdEmployee.Id_Post;
        tempResponseEmployee.IdTask = createdEmployee.Id_Task;
        tempResponseEmployee.Rate = createdEmployee.Rate.ToString("F2", CultureInfo.InvariantCulture);

        response.Message = "Succsessful created employee!";
        response.ItemEmployee.Add(tempResponseEmployee);
        return response;
    }

    /// <summary>
    /// Выдать всех сотрудников
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task<AllEmployees> GetAllEmployee(EmptyRequest request, ServerCallContext context)
    {
        var response = new AllEmployees();
        var allEmployees = new List<Employee>();

        allEmployees = await _repository.GetAllEmployees();

        foreach (Employee emp in allEmployees)
        {
            response.Employees.Add(new ItemEmployee
            {
                Id = emp.Id,
                FName = emp.FirstName,
                MName = emp.MiddleName,
                LName = emp.LastName,
                DateBirth = emp.Date_Birth.ToString("yyyy-MM-dd"),
                IdDepartment = emp.Id_Department,
                IdPost = emp.Id_Post,
                IdTask = emp.Id_Task,
                Rate = emp.Rate.ToString("F2", CultureInfo.InvariantCulture)
            });
        }

        return response;
    }

    /// <summary>
    /// Обновить сотрудника
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public async override Task<UpdatedEmployee> UpdateEmployee(NewEmployeeVersion request, ServerCallContext context)
    {
        var response = new UpdatedEmployee();
        
        var causeDepartment = _repository.CheckDepartment(request.IdDepartment);
        if (await causeDepartment == false)
        {
            response.Message = "Inputted department not exists!";
            return response;
        }

        var causePost = _repository.CheckPost(request.IdPost);
        if (await causePost == false)
        {
            response.Message = "Inputted post not exists!";
            return response;
        }

        var causeTask = _repository.CheckTask(request.IdTask);
        if (await causeTask == false)
        {
            response.Message = "Inputted task not exists!";
            return response;
        }
        
        DateOnly today = DateOnly.FromDateTime(DateTime.Today);
        var DateEmployee = DateOnly.Parse(request.DateBirth);
        if (DateEmployee > today.AddYears(-18))
        {
            response.Message = "Age little 18!";
            return response;
        }
        
        var newemp = new Employee();
        newemp.Id = request.Id;
        newemp.FirstName = request.FName;
        newemp.MiddleName = request.MName;
        newemp.LastName = request.LName;
        newemp.Date_Birth = DateOnly.Parse(request.DateBirth);
        newemp.Id_Department = request.IdDepartment;
        newemp.Id_Post = request.IdPost;
        newemp.Id_Task = request.IdTask;
        newemp.Rate = Decimal.Parse(request.Rate);

        var updatedEmployee = await _repository.UpdateEmployee(newemp);
        
        var tempResponseEmployee = new ItemUpdatedEmployee();
        tempResponseEmployee.Id = updatedEmployee.Id;
        tempResponseEmployee.FName = updatedEmployee.FirstName;
        tempResponseEmployee.MName = updatedEmployee.MiddleName;
        tempResponseEmployee.LName = updatedEmployee.LastName;
        tempResponseEmployee.DateBirth = updatedEmployee.Date_Birth.ToString("yyyy-MM-dd");
        tempResponseEmployee.IdDepartment = updatedEmployee.Id_Department;
        tempResponseEmployee.IdPost = updatedEmployee.Id_Post;
        tempResponseEmployee.IdTask = updatedEmployee.Id_Task;
        tempResponseEmployee.Rate = updatedEmployee.Rate.ToString("F2", CultureInfo.InvariantCulture);
        
        response.Message = "Succsessful update employee!";
        response.Employee.Add(tempResponseEmployee);
        return response;
    }

    /// <summary>
    /// удалить сотрудника
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public async override Task<DeleteMessage> DeleteEmployee(DeleteEmployeeData request, ServerCallContext context)
    {
        var response = new DeleteMessage();
        var message = await _repository.DeleteEmployee(request.Id);
        response.Message = message;
        return response;
    }

    #endregion
}