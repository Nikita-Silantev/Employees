using System.Reactive;
using System.Threading.Tasks;
using ClientEmp.Models;
using ClientEmp.Services;
using Grpc.Contracts;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ClientEmp.ViewModels.UserControlVM;

public class DepartmentUCVM : ViewModelBase
{
    public string Word { get; set; } = "Hello department";
    private DepartmentService _departmentService = DepartmentService;
    [Reactive] public string Name { get; set; }
    public ReactiveCommand<Unit, Unit> AddDepartmentCommand { get; }

    public DepartmentUCVM()
    {
        var canAdd = this.WhenAnyValue(vm => vm.Name,
            name => !string.IsNullOrWhiteSpace(name));

        AddDepartmentCommand = ReactiveCommand.CreateFromTask(AddDepartment, canAdd);
    }

    private async Task AddDepartment()
    {
        var _newDepartment = new Department();
        _newDepartment.Name = Name;
        var _goDepartment = new RequestData();
        _goDepartment.Name = _newDepartment.Name;
        await _departmentService.CreateDepartment(_goDepartment);
        Name = string.Empty;
    }
}