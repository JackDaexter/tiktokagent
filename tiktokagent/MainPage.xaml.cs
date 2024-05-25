using CommunityToolkit.Mvvm.Messaging;
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

        WeakReferenceMessenger.Default.Register<AppEvents>(this,  (m, response) =>
        {
            
            if (response.Status.Equals(ApplicationEvents.AccountLoaded))
            {
                 DisplayAlert("Validation", "Chargement des comptes réussi", "OK");
            }
            /*else  if (response.Status.Equals(ApplicationEvents.AccountSaved))
            {
                 DisplayAlert("Validation", "Comptes sauvegardé", "OK");
            }
            else if (response.Status.Equals(ApplicationEvents.AccountRemoved))
            {
                 DisplayAlert("Suppression effectué", "Compte supprimé", "OK");
            }
            else if (response.Status.Equals(ApplicationEvents.SelectAccountToRemove))
            {
                 DisplayAlert("Selectionner un compte", "Veuillez sélectionner un compte a supprimer", "OK");
            }
            else if (response.Status.Equals(ApplicationEvents.AccountAlreadyExist))
            {
                 DisplayAlert("Compte existant", "Ce compte existe déjà", "OK");
            }     
            else if (response.Status.Equals(ApplicationEvents.FilePathError))
            {
                 DisplayAlert("Chemin inexistant", "Problème avec le chemin du fichier", "OK");
            }
            else if (response.Status.Equals(ApplicationEvents.AccountAdded))
            {
                 DisplayAlert("Compte Ajouté", "Le compte a bien été ajouté", "OK");
            }*/
            
            
        });
    }

  /*  private async void OnStart(object sender, EventArgs e)
    {
        DisplayAlert("Title", "Adjoinrefdfd", "Ok");
    }*/

    private async void OnAddNewAccountWindow(object sender, EventArgs e)
    {
        Window window = new Window(new AddAccountPage(VM));
        window.MaximumHeight = 400;
        window.MaximumWidth = 580;
        window.MinimumHeight = 400;
        window.MinimumWidth = 580;
        
        Application.Current.OpenWindow(window);
    }

}