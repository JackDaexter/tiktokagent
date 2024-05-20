using tiktokagent.Domain;
using tiktokagent.ViewModel;

namespace tiktokagent;

public partial class NewPage1 : ContentPage
{
    private MainPageVm VM;

    public NewPage1(MainPageVm vm)
	{
		InitializeComponent();
        BindingContext = vm;
        VM = vm;
    }

    public void OnAddNewAccount(object sender, EventArgs e)
    {
        Account account = null;
        Proxy proxy = null;

        if(Username.Text != null && Email.Text != null && Password.Text != null) 
        {
            var email = Email.Text;
            var username = Username.Text;
            var password = Password.Text;

            if(Ip.Text != null && Port.Text != null)
            {
                var port = Int32.Parse(Port.Text);
               
                proxy = new Proxy(Ip.Text, port,null,null);    
            }
            account = new Account(email, username, password, Status.Inactive, Compte.Unsubscribe, proxy);
            
            VM.AddNewAccount(account);
        }
    }

    public void OnClose(object sender, EventArgs e)
    {
        //Application.Current.Shu
    }
}