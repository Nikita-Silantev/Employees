using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClientEmp.Models;
using ClientEmp.Services;
using ReactiveUI.SourceGenerators;

namespace ClientEmp.ViewModels.UserControlVM;

public partial class DepartmentUCVM : ViewModelBase
{
    [Reactive] private List<Department> _departments = new();

    public DepartmentUCVM()
    {
        _ = LoadDepartmentsAsync();
    }

    private async Task LoadDepartmentsAsync()
    {
        try
        {
            var service = new ServiceGrpc();
            Departments = await service.GetAllDepartment();
        }
        catch (Exception ex)
        {
            // TODO: нормальное логирование/отображение ошибки пользователю
            Console.WriteLine(ex);
        }
    }
}