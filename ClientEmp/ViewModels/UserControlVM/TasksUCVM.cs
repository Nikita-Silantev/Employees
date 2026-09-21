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
using ClientEmp.Models;

namespace ClientEmp.ViewModels.UserControlVM;

public partial class TasksUCVM : ViewModelBase
{
    public Interaction<string, RxVoid> ShowMessage { get; set; } = new();
    private readonly ServiceGrpc _service;
    #region приватные свойства

    [Reactive] private string _name = string.Empty;
    [Reactive] private DateTime? _dateStart = DateTime.Now;
    [Reactive] private DateTime? _dateEnd = DateTime.Now;

    #endregion

    #region ReactiveCommands

    public ReactiveCommand<RxVoid, RxVoid> AddTaskCommand { get; set; }

    #endregion

    #region ObservableCollection

    [Reactive] private ObservableCollection<EmpTask> _tasksCollection = new();

    #endregion

    public TasksUCVM(ServiceGrpc service)
    {
        _service = service;
        _ = LoadAllTasks();
        AddTaskCommand = ReactiveCommand.CreateFromTask(AddTask);
    }

    /// <summary>
    /// Создание задачи
    /// </summary>
    public async Task AddTask()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            await ShowMessage.Handle("Имя не должно быть пустым");
            return;
        }

        if (DateStart is null)
        {
            await ShowMessage.Handle("Дата начала не должна быть пустой");
            return;
        }

        if (DateEnd is null)
        {
            await ShowMessage.Handle("Дата окончания не должна быть пустой");
            return;
        }

        if (DateStart > DateEnd)
        {
            await ShowMessage.Handle("Дата начала не может быть позже даты конца");
            return;
        }

        try
        {
            await _service.CreateTask(Name, DateStart.Value, DateEnd.Value);
            Name = string.Empty;
            DateStart = DateTime.Now;
            DateEnd = DateTime.Now;
        }
        catch
        {
            await ShowMessage.Handle("Не удалось добавить задачу");
        }
    }

    public async Task LoadAllTasks()
    {
        List<EmpTask> tasks;
        try
        {
            _tasksCollection.Clear();
            tasks = await _service.GetAllTasks();
            foreach (var task in tasks)
            {
                _tasksCollection.Add(task);
            }
        }
        catch
        {
            await ShowMessage.Handle("Не удалось прочитать задачи");
        }
    }
}