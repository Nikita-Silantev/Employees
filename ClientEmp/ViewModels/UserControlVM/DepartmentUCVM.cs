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
using ReactiveUI.Primitives.Signals;

namespace ClientEmp.ViewModels.UserControlVM;

public partial class DepartmentUCVM : ViewModelBase
{
    public Interaction<string, RxVoid> ShowMessage { get; set; } = new();
    
    private readonly ServiceGrpc _service;

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
        catch
        {
            await ShowMessage.Handle("Не удалось загрузить отделы");
        }
    }

    /// <summary>
    /// Создать отдел
    /// </summary>
    private async Task AddDepartment()
    {
        try
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
                await ShowMessage.Handle("Имя отдела не может быть пустым");
            }
        }
        catch
        {
            await ShowMessage.Handle("Не удалось добавить отдел");
        }
    }

    /// <summary>
    /// Удалить отдел
    /// </summary>
    /// <param name="department"></param>
    private async Task DeleteDepartment(Department department)
    {
        try
        {
            var result = await _service.DeleteDepartment(department);
            if (result.Status == "Succsessful Delete Department")
            {
                Departments.Remove(department);
            }
            else
            {
                await ShowMessage.Handle("Ошибка удаления отдела");
            }
        }
        catch
        {
            await ShowMessage.Handle("Не удалось удалить отдел");
        }
    }

    /// <summary>
    /// обновить отдел
    /// </summary>
    /// <param name="department"></param>
    private async Task UpdateDepartment(Department department)
    {
        try
        {
            await _service.UpdateDepartment(department);
            await LoadDepartmentsAsync();
        }
        catch
        {
            await ShowMessage.Handle("Не удалось обновить отдел");
        }
    }
}