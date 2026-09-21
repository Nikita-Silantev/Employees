using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ClientEmp.ViewModels;
using System;
using ReactiveUI.Primitives;

namespace ClientEmp.View;

public partial class InformationWindow : Window
{
    public InformationWindow() => InitializeComponent();
    public InformationWindow(InformationwindowVM vm) : this()
    {
        DataContext = vm;
        vm.CloseWindowCommand.Subscribe(new Action<RxVoid>(_ => Close()));
    }
}