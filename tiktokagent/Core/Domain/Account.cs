namespace tiktokagent.Domain;

public enum Status
{
    Active,
    Inactive,
    Shadowbanned,
    Suspended,
    Testing,
    Captcha,
}

public enum Compte
{
    Subscribe,
    Unsubscribe,
}

public record Account
{
    public Account(string email, string username, string password, Status status, Compte compte=Compte.Unsubscribe)
    {
        Email = email;
        Username = username;
        Password = password;
        Status = status;
        Compte = compte;
    }

    public string Email { get; set; }
    public string Username { get; init; }
    public string Password { get; init; }
    public Status Status { get; set; }
    public Compte Compte { get; set; }
}
