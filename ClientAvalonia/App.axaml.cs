using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using System.Net.Http;
using Avalonia.Markup.Xaml;
using ClientAvalonia.Services;
using ClientAvalonia.ViewModels;
using ClientAvalonia.Views;
using Splat;

namespace ClientAvalonia;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        ServiceRegistration();
    }

    private void ServiceRegistration()
    {
        Locator.CurrentMutable.Register(() => new HttpClient(), typeof(HttpClient));
        
        HttpClient client = Locator.Current.GetService<HttpClient>() ?? throw new InvalidOperationException();
        string apiUrl = "http://localhost:5149";
        
        Locator.CurrentMutable.Register(() => new UserService(client,apiUrl), typeof(UserService));
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            desktop.MainWindow = new AuthenticationWindowView()
            {
                DataContext = new AuthenticationWindowViewModel(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}