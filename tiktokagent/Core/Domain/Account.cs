using CommunityToolkit.Mvvm.ComponentModel;

namespace tiktokagent.Domain;


public enum Compte
{
    Subscribe,
    Unsubscribe,
}


public partial class Account : ObservableObject
{
    [ObservableProperty]
    public string email;

    [ObservableProperty]
    public string username;

    [ObservableProperty]
    public string password;
    

    
    public Account(string email, string username, string password)
    {
        Email = email;
        Username = username;
        Password = password;
    }

    
}
