using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using ReactiveUI.Primitives;
using ReactiveUI.SourceGenerators;
using System.Threading.Tasks;
using Avalonia;
using ClientEmp.Models;
using ClientEmp.Services;
using ReactiveUI;
using ReactiveUI.Primitives.Signals;
using ClientEmp.Models;

namespace ClientEmp.ViewModels.UserControlVM;

public partial class EmployeesUCVM : ViewModelBase
{
    private readonly ServiceGrpc _service;
    [Reactive] private string _word = "Hello";
    [Reactive] private ObservableCollection<EmployeeDictionaryItem> _employees = new();
    public Interaction<string, RxVoid> ShowMessage { get; set; } = new();
    
    public EmployeesUCVM(ServiceGrpc service)
    {
        _service =  service;
        _ = LoadAllInfoAboutEmployee();
        //TODO научиться по ID отдела задачи и должности показывать имена этих объектов, скорее всего будет делать LINQ
    }

    public async Task LoadAllInfoAboutEmployee()
    {
        try
        {
            var departments = await _service.GetAllDepartment();
            var employeeDB = await _service.GetAllEmployees();
            var posts = await _service.GetAllPosts();
            var tasks = await _service.GetAllTasks();
            Dictionary<int, string> deppartmentsDictionary = new();
            Dictionary<int, string> postsDictionary = new();
            Dictionary<int, string> tasksDictionary = new();
            
            foreach (var department in departments)
            {
                deppartmentsDictionary.Add(department.Id, department.Name);
            }

            foreach (var post in posts)
            {
                postsDictionary.Add(post.Id, post.Name);
            }

            foreach (var task in tasks)
            {
                tasksDictionary.Add(task.Id, task.Name);
            }

            foreach (var emp in employeeDB)
            {
                EmployeeDictionaryItem employeeToUI = new()
                {
                    Id = emp.Id,
                    FirstName = emp.FirstName,
                    LastName = emp.LastName,
                    MiddleName = emp.MiddleName,
                    Date_Birth = emp.Date_Birth.ToDateTime(TimeOnly.MinValue),
                    Department = deppartmentsDictionary[emp.Id_Department],
                    Post = postsDictionary[emp.Id_Post],
                    Task = tasksDictionary[emp.Id_Task],
                    Rate = emp.Rate,
                };
                
                _employees.Add(employeeToUI);
            }
        }
        catch
        {
            await ShowMessage.Handle("Не удалось загрузить информацию о сотрудниках");
        }
    }
}