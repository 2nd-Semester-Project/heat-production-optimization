using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using UserInterface.ViewModels;
using UserInterface.Views;

namespace UserInterface;

public partial class App : Application
{
    // Initialises new avalonia app
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    // Sets up the main window of the application with the appropriate view model when the framework initialization is completed.
    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}