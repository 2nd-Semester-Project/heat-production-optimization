using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace UserInterface.ViewModels;

public class HomepageViewModel : ViewModelBase
{
    public int AssetCount
    {
        get => Assets.Count;
    }
    public HomepageViewModel()
    {
        
    }
}