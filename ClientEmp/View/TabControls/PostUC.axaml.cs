using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ClientEmp.ViewModels;
using ClientEmp.ViewModels.UserControlVM;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using ReactiveUI.Primitives;

namespace ClientEmp.View.TabControls;

public partial class PostUC : UserControl
{
    public PostUC()
    {
        InitializeComponent();
        var app = (App)Application.Current!;
        var vm = app.Services.GetRequiredService<PostUCVM>();
        DataContext = vm;

        Func<IInteractionContext<string, RxVoid>, Task> handler = async interaction =>
        {
            var owner = TopLevel.GetTopLevel(this) as Window;
            var infoVm = new InformationwindowVM(interaction.Input);
            var infoWindow = new InformationWindow(infoVm);
            await infoWindow.ShowDialog(owner!);
            interaction.SetOutput(RxVoid.Default); // см. пункт 2
        };
        vm.ShowMessage.RegisterHandler(handler);
    }
}