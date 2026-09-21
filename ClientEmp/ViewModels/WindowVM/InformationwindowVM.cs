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

namespace ClientEmp.ViewModels;

public partial class InformationwindowVM : ViewModelBase
{
    [Reactive] private string _message = string.Empty;
    
    public ReactiveCommand<RxVoid, RxVoid> CloseWindowCommand { get; }

    public InformationwindowVM(string message)
    {
        Message = message;
        CloseWindowCommand = ReactiveCommand.Create(() => { });
    }
}