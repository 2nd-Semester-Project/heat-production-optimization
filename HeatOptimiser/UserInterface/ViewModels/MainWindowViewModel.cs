using System.Reactive;
using System.Reflection;
using ReactiveUI;
using System.Collections.ObjectModel;


namespace UserInterface.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    //Colors and Backgrounds maybe store them here?
    public string paneBackground{get=>"#85182a";}
    public string background{get=>"#cccccc";}
    public string showButtons;

    public bool _paneStatus = true;
    public bool PaneStatus
    {
        get => _paneStatus;
        set     {
                this.RaiseAndSetIfChanged(ref _paneStatus, value);
                ButtonWidth = _paneStatus ? 200 : 50;
                ButtonTextOpacity = _paneStatus ? 1.0 : 0.0;
                }
        
        
    }
    private double _buttonWidth = 200;
    public double ButtonWidth
    {
        get => _buttonWidth;
        set => this.RaiseAndSetIfChanged(ref _buttonWidth, value);
    }
    private double _buttontextOpacity = 1.0;
    public double ButtonTextOpacity
    {
        get => _buttontextOpacity;
        set => this.RaiseAndSetIfChanged(ref _buttontextOpacity, value);
    }

    private ReactiveObject _currentView;
public ReactiveObject CurrentView {
    get =>_currentView;
    set => this.RaiseAndSetIfChanged(ref _currentView, value);
    }

public ReactiveCommand<Unit, bool> PaneCommand {get;}
public ReactiveCommand<Unit, ReactiveObject> OpenAssetManagerCommand {get;}
public ReactiveCommand<Unit, ReactiveObject> OpenSourceDataManagerCommand {get;}
public ReactiveCommand<Unit, ReactiveObject> OpenHomepageCommand {get;}
public ReactiveCommand<Unit, ReactiveObject> OpenOptimiserCommand {get;}
public ReactiveCommand<Unit, ReactiveObject> OpenResultsCommand {get;}

public MainWindowViewModel()
{   
    CurrentView=new HomepageViewModel();
    PaneCommand=ReactiveCommand.Create(()=> PaneStatus=!PaneStatus);
    OpenAssetManagerCommand=ReactiveCommand.Create(()=> CurrentView=new AssetManagerViewModel());
    OpenHomepageCommand=ReactiveCommand.Create(()=> CurrentView=new HomepageViewModel());
    OpenOptimiserCommand=ReactiveCommand.Create(()=> CurrentView= new OptimiserViewModel());
    OpenResultsCommand=ReactiveCommand.Create(()=> CurrentView= new ResultsViewModel());
}




}
