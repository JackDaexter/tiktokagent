using Microsoft.Maui.Controls.Shapes;
using tiktokagent.Core.Usecases;
using tiktokagent.Domain;
using System;
using System.IO;
using Path = System.IO.Path;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using tiktokagent.Core.Domain;
// ReSharper disable All

namespace tiktokagent.Core.Infrastructure;

public class AccountFileAdapter : IObtainAccounts
{
    private string _path;
    
    public AccountFileAdapter(string path)
    {
        _path = path;
    }

    public async Task<List<Proxy>> LoadAllProxyAsync()
    {
        try
        {
            List<Proxy> multipleProxy = new List<Proxy>();
            string fileNameFromStorage = await SecureStorage.Default.GetAsync("ProxyFileName");
            string filePathFromStorage = await SecureStorage.Default.GetAsync("ProxyFilePath");

            string docPath = filePathFromStorage ?? Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            string fileName = fileNameFromStorage ?? "proxy.txt";

            var jsonContent = File.ReadAllText(Path.Combine(docPath, fileName));

            if (!File.Exists(Path.Combine(docPath, fileName)) || !IsContentJsonContent(jsonContent))
            {
                File.Create(Path.Combine(docPath, fileName));
                return new List<Proxy>();
            }
            using StreamReader reader = new(Path.Combine(docPath, fileName));
            var proxyMapperDeserialized = System.Text.Json.JsonSerializer.Deserialize<List<Proxy>>(jsonContent);
            proxyMapperDeserialized.ForEach(x =>
            {
                var proxy = new Proxy(x.Ip, x.Port);
                multipleProxy.Add(proxy);
            });
            return multipleProxy;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error : " + ex.Message);
            throw new Exception("Error");
        }
    }

    public async Task<List<Account>> LoadAllAccountsAsync()
    {
        try
        {
            List<Account> accounts = new List<Account>();
            string fileNameFromStorage = await SecureStorage.Default.GetAsync("FileName");
            string filePathFromStorage = await SecureStorage.Default.GetAsync("FilePath");

            string docPath = filePathFromStorage ?? Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            string fileName = fileNameFromStorage ?? "accounts.json";

            var jsonContent = File.ReadAllText(Path.Combine(docPath, fileName));

            if (!File.Exists(Path.Combine(docPath, fileName)) || !IsContentJsonContent(jsonContent))
            {
                File.Create(Path.Combine(docPath, fileName));
                return new List<Account>();
            }
            using StreamReader reader = new(Path.Combine(docPath, fileName));
            var accountMapperDeserialized = System.Text.Json.JsonSerializer.Deserialize<List<AccountMapper>>(jsonContent);
            accountMapperDeserialized.ForEach(x =>
            {
                var account = new Account(x.Email, x.Username, x.Password);
                accounts.Add(account);
            });
            return accounts;
        }catch (Exception ex)
        {
            Console.WriteLine("Error : " + ex.Message);
            throw new Exception("Error");
        }
        
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
        List<AccountMapper> accountsMapper = new List<AccountMapper>();
        string fileNameFromStorage = await SecureStorage.Default.GetAsync("FileName");
        string filePathFromStorage = await SecureStorage.Default.GetAsync("FilePath");

        string docPath = filePathFromStorage ?? Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string fileName = fileNameFromStorage ?? "accounts.json";

        account.ForEach(acc => {
            var accountMapper = new AccountMapper(acc.Email, acc.Username, acc.Password);
            accountsMapper.Add(accountMapper);
        });

        var accountMapperJsonFormat = JsonConvert.SerializeObject(accountsMapper, Formatting.Indented);

        File.WriteAllText(Path.Combine(docPath, fileName), accountMapperJsonFormat);
    }

    private Boolean IsContentJsonContent(string jsonContent)
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