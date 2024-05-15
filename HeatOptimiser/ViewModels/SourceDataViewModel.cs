namespace UserInterface.ViewModels;
using ReactiveUI;
using System.Reactive;
using HeatOptimiser;
public class SourceDataViewModel : ViewModelBase
{
    private string _selectedFilePath;
    private int _selectedRow;
    private int _selectedColumn;
    public ReactiveCommand<Unit, Unit> SourceDataCommand { get; }
    public string SelectedFilePath
    {
        get => _selectedFilePath;
        set => this.RaiseAndSetIfChanged(ref _selectedFilePath, value);
    }

    public int SelectedRow
    {
        get => _selectedRow;
        set => this.RaiseAndSetIfChanged(ref _selectedRow, value);
    }

    public int SelectedColumn
    {
        get => _selectedColumn;
        set => this.RaiseAndSetIfChanged(ref _selectedColumn, value);
    }

    public SourceDataViewModel()
    {
        _selectedFilePath = string.Empty;
        SourceData sourceData = new SourceData(); 
        SourceDataCommand = ReactiveCommand.Create(() => sourceData.LoadSourceData(SelectedFilePath, SelectedRow, SelectedColumn));

    }
}