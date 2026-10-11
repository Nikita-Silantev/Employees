using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
    #region Приватные поля

    private readonly ServiceGrpc _service;
    [Reactive] private string empFirstName = string.Empty;
    [Reactive] private string empLastName = string.Empty;
    [Reactive] private string empMiddleName = string.Empty;
    [Reactive] private DateTime? empDateBirth = DateTime.Now;
    [Reactive] private int idDepartment;
    [Reactive] private int idPost;
    [Reactive] private int idTask;
    [Reactive] private string rate = string.Empty;

    #endregion

    #region Коллекции

    [Reactive] private ObservableCollection<EmployeeDictionaryItem> _employees = new();
    [Reactive] private ObservableCollection<Department> _departments = new();
    [Reactive] private ObservableCollection<EmpTask> _tasks = new();
    [Reactive] private ObservableCollection<Post> _posts = new();

    #endregion

    public Interaction<string, RxVoid> ShowMessage { get; set; } = new();

    #region Команды

    public ReactiveCommand<RxVoid, RxVoid> AddEmployeeCommand { get; set; }
    public ReactiveCommand<EmployeeDictionaryItem, RxVoid> DeleteEmployeeCommand { get; set; }
    public ReactiveCommand<EmployeeDictionaryItem, RxVoid> EditEmployeeCommand { get; set; }
    #endregion

    public EmployeesUCVM(ServiceGrpc service)
    {
        _service = service;
        AddEmployeeCommand = ReactiveCommand.CreateFromTask(CreateEmployee);
        DeleteEmployeeCommand = ReactiveCommand.CreateFromTask<EmployeeDictionaryItem>(DeleteEmployee);
        EditEmployeeCommand = ReactiveCommand.CreateFromTask<EmployeeDictionaryItem>(UpdateEmployee);
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

            // Собираем НОВЫЕ коллекции и подменяем целиком в конце:
            // так ComboBox'ы в таблице не видят промежуточного "пустого" состояния списков
            var newDepartments = new ObservableCollection<Department>();
            var newTasks = new ObservableCollection<EmpTask>();
            var newPosts = new ObservableCollection<Post>();
            var newEmployees = new ObservableCollection<EmployeeDictionaryItem>();

            foreach (var department in departments)
            {
                Department departmentToUI = new()
                {
                    Id = department.Id,
                    Name = department.Name
                };
                newDepartments.Add(departmentToUI);
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
                newTasks.Add(empTaskToUI);
            }

            foreach (var post in posts)
            {
                Post postToUI = new()
                {
                    Id = post.Id,
                    Name = post.Name,
                    Salary = post.Salary
                };
                newPosts.Add(postToUI);
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
                    idDepartment = emp.Id_Department,
                    Post = postsDictionary[emp.Id_Post],
                    idPost = emp.Id_Post,
                    Task = tasksDictionary[emp.Id_Task],
                    idTask = emp.Id_Task,
                    Rate = emp.Rate,
                };

                newEmployees.Add(employeeToUI);
            }

            // Порядок важен: сначала справочники для ComboBox, потом сотрудники
            Departments = newDepartments;
            Posts = newPosts;
            Tasks = newTasks;
            Employees = newEmployees;
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

            string result = await _service.CreateEmployee(EmpLastName, EmpFirstName, EmpMiddleName, EmpDateBirth.Value,
                IdDepartment, IdPost, IdTask, normalizedRate);
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

    /// <summary>
    /// Удаление сотрудника
    /// </summary>
    /// <param name="employee"></param>
    public async Task DeleteEmployee(EmployeeDictionaryItem employee)
    {
        try
        {
            int deleteId = employee.Id;
            string result = await _service.DeleteEmployee(deleteId);
            if (result == "success")
            {
                LoadAllInfoAboutEmployee();
                return;
            }
            else
            {
                await ShowMessage.Handle("Ошибка удаления сотрудника");
            }
        }
        catch
        {
            await ShowMessage.Handle("Не удалось удалить сотрудника");
        }
    }

    public async Task UpdateEmployee(EmployeeDictionaryItem employee)
    {
        try
        {
            var request = new Employee()
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                MiddleName = employee.MiddleName,
                Date_Birth = employee.Date_Birth.HasValue
                    ? DateOnly.FromDateTime(employee.Date_Birth.Value)
                    : DateOnly.MinValue,
                Id_Department = employee.idDepartment,
                Id_Post = employee.idPost,
                Id_Task = employee.idTask,
                Rate = employee.Rate
            };
            string result = await _service.UpdateEmployee(request);
            if (result == "Succsessful update employee!")
            {
                await LoadAllInfoAboutEmployee();
                return;
            }

            // сервер отказал (возраст, несуществующий отдел и т.д.) - показываем его сообщение
            await ShowMessage.Handle(result);
            await LoadAllInfoAboutEmployee(); // вернуть строку к реальному состоянию из БД
        }
        catch (Exception ex)
        {
            await ShowMessage.Handle($"Не удалось обновить сотрудника: {ex.Message}");
        }
    }
}