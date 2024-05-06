using tiktokagent.ViewModel;

namespace tiktokagent;


public partial class MainPage : ContentPage
{
    
    public MainPage(MainPageVm vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    

    private async void OnStart(object sender, EventArgs e)
    {
        await DisplayAlert("Alert", "You have been alerted", "OK");
    }


}