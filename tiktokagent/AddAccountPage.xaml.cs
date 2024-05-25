using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.ApplicationModel.Communication;
using tiktokagent.Core.Domain;
using tiktokagent.Domain;
using tiktokagent.Messaging;
using tiktokagent.ViewModel;

namespace tiktokagent;

public partial class AddAccountPage : ContentPage
{
    private readonly MainPageVm _vm;

    public AddAccountPage(MainPageVm vm)
	{
		InitializeComponent();
        BindingContext = vm;
        _vm = vm;
    }

    public void OnAddNewAccount(object sender, EventArgs e)
    {
        Account account = null;
        if (Username.Text != null && Email.Text != null && Password.Text != null) 
        {
            var email = Email.Text;
            var username = Username.Text;
            var password = Password.Text;
            account = new Account(email, username, password);
            
            _vm.AddNewAccount(account);
            WeakReferenceMessenger.Default.Send(new AppEvents(ApplicationEvents.AccountAdded)); ;

                
            FieldReinitialisation();
        }
        else
        {
            DisplayAlert("Errur", "Veuillez absolument remplir le username; l'email et le mot de passe", "OK");
        }
    }

    private void FieldReinitialisation()
    {
        Email.Text = "";
        Username.Text = "";
        Password.Text = "";
    }

    public void OnClose(object sender, EventArgs e)
    {
        //Application.Current.Shu
    }
}