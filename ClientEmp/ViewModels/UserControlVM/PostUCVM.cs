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

public partial class PostUCVM : ViewModelBase
{
    public Interaction<string, RxVoid> ShowMessage { get; set; } = new();
    private readonly ServiceGrpc _service;

    #region private

    [Reactive] private string _name = String.Empty;
    [Reactive] private int _salary = 0;

    #endregion

    #region ReactiveCommands

    public ReactiveCommand<RxVoid, RxVoid> AddPostCommand { get; }
    public ReactiveCommand<Post, RxVoid> UpdatePostCommand { get; }
    public ReactiveCommand<Post, RxVoid> DeletePostCommand { get; }

    #endregion

    #region ObsCollections

    [Reactive] private ObservableCollection<Post> _allPosts = new();

    #endregion

    public PostUCVM(ServiceGrpc service)
    {
        _service = service;
        _ = LoadPosts();
        AddPostCommand = ReactiveCommand.CreateFromTask(CreatePost);
        UpdatePostCommand = ReactiveCommand.CreateFromTask<Post>(UpdatePost);
        DeletePostCommand = ReactiveCommand.CreateFromTask<Post>(DeletePost);
    }

    /// <summary>
    /// Создать отдел
    /// </summary>
    public async Task CreatePost()
    {
        if (_name == String.Empty)
        {
            await ShowMessage.Handle("Имя не может бысть пустым");
            return;
        }

        if (_salary < 27093)
        {
            await ShowMessage.Handle(
                "Из расчета на 2026 год, минимальный прожиточный минимум в России не может быть менее 27093 рублей");
            return;
        }

        try
        {
            await _service.CreatePost(_name, _salary);
            Name = String.Empty;
            Salary = 0;
            LoadPosts();
        }
        catch
        {
            await ShowMessage.Handle("Не удалось добавить должность");
        }
    }

    /// <summary>
    /// Прочитать все должности
    /// </summary>
    public async Task LoadPosts()
    {
        _allPosts.Clear();
        var newAllPosts = await _service.GetAllPosts();
        foreach (var post in newAllPosts)
        {
            _allPosts.Add(post);
        }
    }

    /// <summary>
    /// Обновление должности
    /// </summary>
    /// <param name="post"></param>
    public async Task UpdatePost(Post post)
    {
        if (post.Name == String.Empty)
        {
            await ShowMessage.Handle("Имя не может бысть пустым");
            return;
        }

        if (post.Salary < 27093)
        {
            await ShowMessage.Handle(
                "Из расчета на 2026 год, минимальный прожиточный минимум в России не может быть менее 27093 рублей");
            return;
        }

        try
        {
            await _service.UpdatePost(post);
            _allPosts.Clear();
            await LoadPosts();
        }
        catch
        {
            await ShowMessage.Handle("Не удалось обновить должность");
        }
    }

    /// <summary>
    /// Удаление должности
    /// </summary>
    /// <param name="post"></param>
    public async Task DeletePost(Post post)
    {
        try
        {
            var responce = await _service.DeletePost(post);
            _allPosts.Remove(post);
        }
        catch
        {
            await ShowMessage.Handle("Не удалось удалить должность");
        }
    }
}