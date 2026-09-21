using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ReactiveUI.Primitives;
using ReactiveUI.SourceGenerators;
using System.Threading.Tasks;
using Avalonia;
using ClientEmp.Models;
using ClientEmp.Services;
using ReactiveUI;

namespace ClientEmp.ViewModels.UserControlVM;

public partial class DepartmentUCVM : ViewModelBase
{
    private readonly ServiceGrpc _service;

    //private bool CanAddDepartment() => true;
    //TODO разобрать LINQ и сделать блокировку на кнопку добавленя
    [Reactive] private ObservableCollection<Department> _departments = new();

    [Reactive] private string _name = string.Empty;
    public ReactiveCommand<RxVoid, RxVoid> AddDepartmentCommand { get; set; }
    public ReactiveCommand<Department, RxVoid> DeleteDepartmentCommand { get; set; }
    public ReactiveCommand<Department, RxVoid> UpdateDepartmentCommand { get; set; }

    public DepartmentUCVM(ServiceGrpc service)
    {
        AddDepartmentCommand = ReactiveCommand.CreateFromTask(AddDepartment);
        DeleteDepartmentCommand = ReactiveCommand.CreateFromTask<Department>(DeleteDepartment);
        UpdateDepartmentCommand = ReactiveCommand.CreateFromTask<Department>(UpdateDepartment);
        _service = service;
        _ = LoadDepartmentsAsync();
    }

    /// <summary>
    /// получить все отделы
    /// </summary>
    private async Task LoadDepartmentsAsync()
    {
        List<Department> deps;
        try
        {
            Departments.Clear();
            deps = await _service.GetAllDepartment();
            foreach (var department in deps)
            {
                Departments.Add(department);
            }
        }
        catch (Exception ex)
        {
            // TODO: нормальное логирование/отображение ошибки пользователю
            Console.WriteLine(ex);
        }
    }

    /// <summary>
    /// Создать отдел
    /// </summary>
    private async Task AddDepartment()
    {
        if (_name != "")
        {
            var department = new Department();
            department = await _service.CreateDepartment(Name);
            Departments.Add(department);
            Name = "";
        }
        else
        {
            return;
        }
    }

    /// <summary>
    /// Удалить отдел
    /// </summary>
    /// <param name="department"></param>
    private async Task DeleteDepartment(Department department)
    {
        var result = await _service.DeleteDepartment(department);
        if (result.Status == "Succsessful Delete Department")
        {
            Departments.Remove(department);
        }
        else
        {
            return;
        }
    }

    /// <summary>
    /// обновить отдел
    /// </summary>
    /// <param name="department"></param>
    private async Task UpdateDepartment(Department department)
    {
        await _service.UpdateDepartment(department);
        await LoadDepartmentsAsync();
    }
}