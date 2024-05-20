using CommunityToolkit.Mvvm.ComponentModel;

namespace tiktokagent.Domain;

public enum Status
{
    Active,
    Inactive,
    Shadowbanned,
    Suspended,
    Testing,
    Captcha,
    Running
}

public enum Compte
{
    Subscribe,
    Unsubscribe,
}

public record Proxy(string Ip, int Port, string Username, string Password);


public partial class Account : ObservableObject
{
    [ObservableProperty]
    public string email;

    [ObservableProperty]
    public string username;

    [ObservableProperty]
    public string password;

    [ObservableProperty]
    public Status status;

    [ObservableProperty]
    public Compte compte;

    [ObservableProperty]
    public Proxy proxy;

    public Account(string email, string username, string password, Status status, Compte compte, Proxy proxy)
    {
        Email = email;
        Username = username;
        Password = password;
        Status = status;
        Compte = compte;
        Proxy = proxy;
    }

    
}
