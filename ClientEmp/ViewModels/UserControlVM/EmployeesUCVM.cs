using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ReactiveUI.Primitives;
using ReactiveUI.SourceGenerators;
using System.Threading.Tasks;
using ClientEmp.Models;
using ClientEmp.Services;
using ReactiveUI;
using ReactiveUI.Primitives.Signals;

namespace ClientEmp.ViewModels.UserControlVM;

public partial class EmployeesUCVM : ViewModelBase
{
    private readonly ServiceGrpc _service;
    
    [Reactive] private string empFirstName = string.Empty;
    [Reactive] private string empLastName = string.Empty;
    [Reactive] private string empMiddleName = string.Empty;
    [Reactive] private DateTime? empDateBirth = DateTime.Now;
    [Reactive] private int idDepartment;
    [Reactive] private int idPost;
    [Reactive] private int idTask;
    [Reactive] private string rate = string.Empty;

    
    
    [Reactive] private ObservableCollection<EmployeeDictionaryItem> _employees = new();
    [Reactive] private ObservableCollection<Department> _departments = new();
    [Reactive] private ObservableCollection<EmpTask> _tasks = new();
    [Reactive] private ObservableCollection<Post> _posts = new();
    
    public Interaction<string, RxVoid> ShowMessage { get; set; } = new();
    
    public ReactiveCommand<RxVoid, RxVoid> AddEmployeeCommand { get; set; }
    public EmployeesUCVM(ServiceGrpc service)
    {
        _service =  service;
        AddEmployeeCommand = ReactiveCommand.CreateFromTask(CreateEmployee);
    }

    public async Task LoadAllInfoAboutEmployee()
    {
        _employees.Clear();
        _departments.Clear();
        _tasks.Clear();
        _posts.Clear();
        try
        {
            var departments = await _service.GetAllDepartment();
            var employeeDB = await _service.GetAllEmployees();
            var posts = await _service.GetAllPosts();
            var tasks = await _service.GetAllTasks();
            
            Dictionary<int, string> deppartmentsDictionary = new();
            Dictionary<int, string> tasksDictionary = new(); 
            Dictionary<int, string> postsDictionary = new();
            
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

            foreach (var department in departments)
            {
                Department departmentToUI = new()
                {
                    Id = department.Id,
                    Name = department.Name
                };
                _departments.Add(departmentToUI);
            }

            foreach (var task in tasks)
            {
                EmpTask empTaskToUI = new()
                {
                    Id = task.Id,
                    Name = task.Name,
                    Date_Started = task.Date_Started,
                    Date_End = task.Date_End
                };
                _tasks.Add(empTaskToUI);
            }

            foreach (var post in posts)
            {
                Post postToUI = new()
                {
                    Id = post.Id,
                    Name = post.Name,
                    Salary = post.Salary
                };
                _posts.Add(postToUI);
            }
        }
        catch
        {
            await ShowMessage.Handle("Не удалось загрузить информацию о сотрудниках");
        }
    }

    public async Task CreateEmployee()
    {
        //await ShowMessage.Handle($"Будет создан сотрудник с параметрами {EmpLastName}, {EmpFirstName}, {EmpMiddleName}, {EmpDateBirth}, {IdDepartment}, {IdPost}, {IdTask}, {rate}");
        try
        {
            if (EmpLastName == "")
            {
                await ShowMessage.Handle("Заполните фамилию!");
                return;
            }

            if (EmpFirstName == "")
            {
                await ShowMessage.Handle("Заполните имя!");
                return;
            }

            if (EmpMiddleName == "")
            {
                await ShowMessage.Handle("Заполните отчество!");
                return;
            }

            if (EmpDateBirth is null)
            {
                await ShowMessage.Handle("Заполните дату рождения!");
                return;
            }

            if (IdDepartment == 0)
            {
                await ShowMessage.Handle("Выберите отдел!");
                return;
            }

            if (IdPost == 0)
            {
                await ShowMessage.Handle("Выберите должность!");
                return;
            }

            if (IdTask == 0)
            {
                await ShowMessage.Handle("Выберите задачу!");
                return;
            }

            if (string.IsNullOrWhiteSpace(rate))
            {
                await ShowMessage.Handle("Заполните ставку!");
                return;
            }

            // Заменяем запятую на точку, чтобы строка ВСЕГДА была в формате "1.0"
            string normalizedRate = rate.Replace(',', '.');

            // Парсим строго по правилам InvariantCulture (где разделитель — ТОЛЬКО точка)
            if (!decimal.TryParse(normalizedRate, System.Globalization.NumberStyles.Any, 
                    System.Globalization.CultureInfo.InvariantCulture, out decimal parsedRate))
            {
                await ShowMessage.Handle("Ставка введена некорректно! Используйте число, например: 1.5 или 1,5");
                return;
            }

            string result = await _service.CreateEmployee(EmpLastName, EmpFirstName, EmpMiddleName, EmpDateBirth.Value, IdDepartment, IdPost, IdTask, rate);
            if (result == "Succsessful created employee!")
            {
                await ShowMessage.Handle("Сотрудник успешно создан!");
                EmpFirstName = "";
                EmpMiddleName = "";
                EmpLastName = "";
                EmpDateBirth = DateTime.Now;
                IdDepartment = 0;
                IdPost = 0;
                IdTask = 0;
                rate = "";
                LoadAllInfoAboutEmployee();
            }
            else
            {
                await ShowMessage.Handle($"{result}");
            }
        }
        catch
        {
            await ShowMessage.Handle("Не удалось создать сотрудника");
        }
    }
}