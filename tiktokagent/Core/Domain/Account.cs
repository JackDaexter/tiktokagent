namespace tiktokagent.Domain;

public enum Status
{
    Active,
    Shadowbanned,
    Suspended,
    Testing 
}


public record Account(string Email, string Username, string Password, Status Status);
