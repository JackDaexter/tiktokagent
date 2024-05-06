namespace tiktokagent.Domain;

public record Proxy(string Host, int Port, string Username, string Password);


public record AccountProxied(string Email, string Username, string Password, Status Status, Proxy Proxy);
// Path: tiktokagent/Domain/Account.cs