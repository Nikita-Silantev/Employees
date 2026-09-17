using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ClientEmp.ViewModels.UserControlVM;

namespace ClientEmp.View.TabControls;

public partial class DepartmentUC : UserControl
{
    public DepartmentUC()
    {
        InitializeComponent();
        DataContext = new DepartmentUCVM();
    }
}