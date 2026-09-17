using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ClientEmp.ViewModels;
using Microsoft.Extensions.Configuration;

namespace ClientEmp;

public partial class App : Application
{
    public IConfiguration Configuration { get; private set; } = null!;
    public IServiceProvider Services { get; private set; } = null!;

    public override void Initialize()
    {
        Configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        Services = ConfigureServices();
        AvaloniaXamlLoader.Load(this);
    }

    private IServiceProvider ConfigureServices()
    {
        var collection = new ServiceCollection();

        collection.AddSingleton(Configuration);

        collection.AddSingleton(_ =>
            GrpcChannel.ForAddress(Configuration["Grpc:ServerUrl"]!));

        collection.AddSingleton(sp =>
            new Employees.EmployeesClient(sp.GetRequiredService<GrpcChannel>()));

        collection.AddSingleton<DepartmentService>();
        // позже так же: collection.AddSingleton<TaskService>(); и т.д.

        collection.AddTransient<MainWindowVM>();
        collection.AddTransient<DepartmentUCVM>();
        // collection.AddTransient<TasksUCVM>();

        return collection.BuildServiceProvider();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = Services.GetRequiredService<MainWindowVM>(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}