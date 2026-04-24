using Microsoft.UI.Xaml.Controls;

namespace Semcosm.HardwareConsole.App.Services;

public interface INavigationService
{
    void Initialize(Frame frame);
    bool Navigate(string route);
}
