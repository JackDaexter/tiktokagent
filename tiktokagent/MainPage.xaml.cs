using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls;
using tiktokagent.Messaging;
using tiktokagent.ViewModel;

namespace tiktokagent;


public partial class MainPage : ContentPage
{
    private MainPageVm VM;
    public MainPage(MainPageVm vm)
    {
        InitializeComponent();
        InitializeAppListener();
        InitializeAppElementListener();
 
        BindingContext = vm;
        VM = vm;
    }

    private void InitializeAppListener()
    {

        WeakReferenceMessenger.Default.Register<AppStatus>(this, async (m, response) =>
        {

            if (response.Status == ApplicationStatus.Error)
            {
                DisplayAlert("Erreur", response.Status.ToString(), "OK");
            }
        });
    } 
    
    private void InitializeAppElementListener()
    {

        WeakReferenceMessenger.Default.Register<AppEvents>(this, async (m, response) =>
        {
            Color color = new();
            
            if (response.Status.Equals(ApplicationEvents.AccountLoaded))
            {
                DisplayAlert("Validation", "Comptes sauvegardé", "OK");
            }
        });
    }

  /*  private async void OnStart(object sender, EventArgs e)
    {
        DisplayAlert("Title", "Adjoinrefdfd", "Ok");
    }*/

    private async void OnAddNewAccountWindow(object sender, EventArgs e)
    {
        Window window = new Window(new NewPage1(VM));
        window.MaximumHeight = 600;
        window.MaximumWidth = 650;
        window.MinimumHeight = 600;
        window.MinimumWidth = 650;
        Application.Current.OpenWindow(window);
    }

}