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
    public ReactiveCommand<EmpTask, RxVoid> UpdateTaskCommand { get; set; }
    public ReactiveCommand<EmpTask, RxVoid> DeleteTaskCommand { get; set; }

    #endregion

    #region ObservableCollection

    [Reactive] private ObservableCollection<EmpTask> _tasksCollection = new();

    #endregion

    public TasksUCVM(ServiceGrpc service)
    {
        _service = service;
        _ = LoadAllTasks();
        AddTaskCommand = ReactiveCommand.CreateFromTask(AddTask);
        UpdateTaskCommand = ReactiveCommand.CreateFromTask<EmpTask>(UpdateTask);
        DeleteTaskCommand = ReactiveCommand.CreateFromTask<EmpTask>(DeleteTask);
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
            await LoadAllTasks();
        }
        catch
        {
            await ShowMessage.Handle("Не удалось добавить задачу");
        }
    }

    /// <summary>
    /// получить все задачи
    /// </summary>
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
    /// <summary>
    /// Обновление задачи
    /// </summary>
    /// <param name="task"></param>
    public async Task UpdateTask(EmpTask task)
    {
        try
        {
            if (task.Name == "")
            {
                await ShowMessage.Handle("Имя не должно быть пустым, обновить не удалось");
                return;
            }

            if (task.Date_Started is null)
            {
                await ShowMessage.Handle("Дата начала не должна быть пустой");
                return;
            }

            if (task.Date_End is null)
            {
                await ShowMessage.Handle("Дата окончания не должна быть пустой");
                return;
            }
            
            if (task.Date_Started > task.Date_End)
            {
                await ShowMessage.Handle("Дата начала не может быть позже даты конца, обновить не удалось");
                return;
            }
            await _service.UpdateTask(task);
            await LoadAllTasks();
        }
        catch
        {
            await ShowMessage.Handle("Не удалось обновить задачу");
        }
    }
    /// <summary>
    /// Удаление задачи
    /// </summary>
    /// <param name="task"></param>
    public async Task DeleteTask(EmpTask task)
    {
        try
        {
            var result = await _service.DeleteTask(task);
            if (result == "Succsessful deleted task!")
            {
                _tasksCollection.Remove(task);
            }
        }
        catch
        {
            await ShowMessage.Handle("Не удалось удалить");
        }
    }
}