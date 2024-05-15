using Microsoft.Maui.Controls.Shapes;
using tiktokagent.Core.Usecases;
using tiktokagent.Domain;
using System;
using System.IO;
using Path = System.IO.Path;
using System.Text.Json;
using Newtonsoft.Json;

namespace tiktokagent.Core.Infrastructure;

public class AccountFileAdapter : IObtainAccounts
{
    private string _path;
    
    public AccountFileAdapter(string path)
    {
        _path = path;
    }

    public List<Account> LoadAllAccounts()
    {
        string docPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        if(!File.Exists(Path.Combine(docPath, "accounts.json")))
        {
            File.Create(Path.Combine(docPath, "accounts.json"));
            return new List<Account>();
        }
        using StreamReader reader = new(Path.Combine(docPath, "accounts.json"));

        var jsonContent = File.ReadAllText(Path.Combine(docPath, "accounts.json"));
        return System.Text.Json.JsonSerializer.Deserialize<List<Account>>(jsonContent);
    }

    public List<AccountProxied> GetAccountsProxieds()
    {
        string docPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        var json = File.ReadAllText(Path.Combine(docPath, "accounts.json"));
        return System.Text.Json.JsonSerializer.Deserialize<List<AccountProxied>>(json);
    }

    public void SaveAccount(Account accountProxied)
    {
        string docPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string json = JsonConvert.SerializeObject(accountProxied);

        File.WriteAllText(Path.Combine(docPath, "accounts.json"), json);
    }

    public void SaveMultipleAccounts(List<Account> account)
    {
        var jsson = JsonConvert.SerializeObject(account);

        string docPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        File.WriteAllText(Path.Combine(docPath, "accounts.json"), jsson);
    }
}