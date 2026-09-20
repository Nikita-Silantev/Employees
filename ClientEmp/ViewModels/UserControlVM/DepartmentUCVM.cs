using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia;
using ClientEmp.Models;
using ClientEmp.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ClientEmp.ViewModels.UserControlVM;

public partial class DepartmentUCVM : ViewModelBase
{
    private readonly ServiceGrpc _service;
    
    [Reactive] public ObservableCollection<Department> Departments { get; set; } = new();

    [Reactive] public string Name { get; set; } = String.Empty;
    public ReactiveCommand<Unit, Unit> AddDepartmentCommand { get; set; }
    public DepartmentUCVM(ServiceGrpc service)
    {
        AddDepartmentCommand = ReactiveCommand.CreateFromTask(AddDepartment);
        _service = service;
        _ = LoadDepartmentsAsync();
    }

    private async Task LoadDepartmentsAsync()
    {
        List<Department> deps;
        try
        {
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

    private async Task AddDepartment()
    {
        var department = new Department();
        department = await _service.CreateDepartment(Name);
        Departments.Add(department);
        
    }
}