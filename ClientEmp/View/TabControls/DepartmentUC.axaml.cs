using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ClientEmp.ViewModels.UserControlVM;
using Microsoft.Extensions.DependencyInjection;

namespace ClientEmp.View.TabControls;

public partial class DepartmentUC : UserControl
{
    public DepartmentUC()
    {
        InitializeComponent();
        var app =  (App)Application.Current!;
        DataContext = app.Services.GetRequiredService<DepartmentUCVM>();
    }
}