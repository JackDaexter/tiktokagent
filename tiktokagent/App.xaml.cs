using tiktokagent.ViewModel;

namespace tiktokagent;

public partial class App : Application
{
    public App(MainPageVm mainPageVm)
    {
        InitializeComponent();

        MainPage = new MainPage(mainPageVm);
    }

    
}