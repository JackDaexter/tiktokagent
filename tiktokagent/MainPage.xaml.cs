using CommunityToolkit.Mvvm.Messaging;
using tiktokagent.Messaging;
using tiktokagent.ViewModel;

namespace tiktokagent;


public partial class MainPage : ContentPage
{
    
    public MainPage(MainPageVm vm)
    {
        InitializeComponent();
        InitializeAppListener();
        InitializeAppElementListener();
 
        BindingContext = vm;
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
        WeakReferenceMessenger.Default.Register<AppElements>(this, async (m, response) =>
        {
            Color color = new();
            
            if (response.Status.Equals(InteractionStatus.ErrorNumberOfAccounts))
            {
                DisplayAlert("Erreur", "Le nombre de compte à générer n'est pas valide.\n Changer de nombre de compte", "OK");
            }
        });
    }

    private async void OnStart(object sender, EventArgs e)
    {
        	
    }


}