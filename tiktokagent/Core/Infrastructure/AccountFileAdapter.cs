using Microsoft.Maui.Controls.Shapes;
using tiktokagent.Core.Usecases;
using tiktokagent.Domain;
using System;
using System.IO;
using Path = System.IO.Path;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace tiktokagent.Core.Infrastructure;

public class AccountFileAdapter : IObtainAccounts
{
    private string _path;
    
    public AccountFileAdapter(string path)
    {
        _path = path;
    }

    public async Task<List<Account>> LoadAllAccountsAsync()
    {
        string fileNameFromStorage = await SecureStorage.Default.GetAsync("FileName");
        string filePathFromStorage = await SecureStorage.Default.GetAsync("FilePath");

        string docPath = filePathFromStorage ?? Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        string fileName = fileNameFromStorage ?? "accounts.json";

        var jsonContent = File.ReadAllText(Path.Combine(docPath, fileName));

        if (!File.Exists(Path.Combine(docPath, fileName)) || !isContentJsonContent(jsonContent))
        {
            File.Create(Path.Combine(docPath, fileName));
            return new List<Account>();
        }
        using StreamReader reader = new(Path.Combine(docPath, fileName));

        return System.Text.Json.JsonSerializer.Deserialize<List<Account>>(jsonContent);
    }

    public async Task SaveAccount(Account accountProxied)
    {
        string fileNameFromStorage = await SecureStorage.Default.GetAsync("FileName");
        string filePathFromStorage = await SecureStorage.Default.GetAsync("FilePath");
        string docPath = filePathFromStorage ?? Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string fileName = fileNameFromStorage ?? "accounts.json";

        string json = JsonConvert.SerializeObject(accountProxied, Formatting.Indented);

        File.WriteAllText(Path.Combine(docPath, fileName), json);
    }

    public async Task SaveMultipleAccounts(List<Account> account)
    {
        string fileNameFromStorage = await SecureStorage.Default.GetAsync("FileName");
        string filePathFromStorage = await SecureStorage.Default.GetAsync("FilePath");

        string docPath = filePathFromStorage ?? Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string fileName = fileNameFromStorage ?? "accounts.json";

        var jsson = JsonConvert.SerializeObject(account, Formatting.Indented);

        File.WriteAllText(Path.Combine(docPath, fileName), jsson);
    }

    private Boolean isContentJsonContent(string jsonContent)
    {
        try
        {
            JContainer.Parse(jsonContent);
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }

  
}