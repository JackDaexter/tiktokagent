using Microsoft.Maui.Handlers;
using tiktokagent.ViewModel;

namespace tiktokagent;

public partial class App : Application
{
    public App(MainPageVm mainPageVm)
    {
        InitializeComponent();

        MainPage = new MainPage(mainPageVm);
        Microsoft.Maui.Handlers.WindowHandler.Mapper.AppendToMapping(nameof(IWindow), (handler, view) =>
        {

        });
    }

    protected override Window CreateWindow(IActivationState activationState)
    {
        var window = base.CreateWindow(activationState);

        const int newWidth = 900;
        const int newHeight = 900;

        window.Width = newWidth;
        window.Height = newHeight;
        
        return window;
    }
}