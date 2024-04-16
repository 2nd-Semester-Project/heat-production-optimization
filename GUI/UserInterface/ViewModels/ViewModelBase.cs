using System.Collections.ObjectModel;
using ReactiveUI;
using System.Reactive;

namespace UserInterface.ViewModels;

public class ViewModelBase : ReactiveObject
{
        public ObservableCollection<NewAsset> Assets {get;} = new();
}
