using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ClientEmp.ViewModels.UserControlVM;

namespace ClientEmp.View.TabControls;

public partial class TasksUC : UserControl
{
    public TasksUC()
    {
        InitializeComponent();
        DataContext = new TasksUCVM();
    }
}