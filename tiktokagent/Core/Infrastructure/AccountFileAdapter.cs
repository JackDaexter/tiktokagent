using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using tiktokagent.Core.Domain;
using tiktokagent.Core.Usecases;
using tiktokagent.Domain;
using JsonSerializer = System.Text.Json.JsonSerializer;

// ReSharper disable All

namespace tiktokagent.Core.Infrastructure;

public class AccountFileAdapter : IObtainAccounts
{
    private string _path;
    
    public AccountFileAdapter(string path)
    {
        _path = path;
    }

    public async Task<List<SimpleProxy>> LoadAllProxyAsync()
    {
        List<SimpleProxy> multipleProxy = new List<SimpleProxy>();
        try
        {
            
            string fileNameFromStorage = await SecureStorage.Default.GetAsync("ProxyFileName");
            string filePathFromStorage = await SecureStorage.Default.GetAsync("ProxyFilePath");

            string docPath = filePathFromStorage ?? Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            string fileName = fileNameFromStorage ?? "proxy.txt";
            
            string[] lines = File.ReadAllLines(Path.Combine(docPath, fileName));

          
            foreach(string line in lines)
            {
                string[] items = line.Split(':');
                int myInteger = int.Parse(items[1]);   // Here's your integer.

                multipleProxy.Add(new SimpleProxy(items[0], myInteger));
            }
            
            
            return multipleProxy;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error : " + ex.Message);
        }

        return multipleProxy;
    }

    public async Task<List<Account>> LoadAllAccountsAsync()
    {
        List<Account> accounts = new List<Account>();
        try
        {
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
            var accountMapperDeserialized = JsonSerializer.Deserialize<List<AccountMapper>>(jsonContent);
            accountMapperDeserialized.ForEach(x =>
            {
                var account = new Account(x.Email, x.Username, x.Password);
                accounts.Add(account);
            });
            return accounts;
        }catch (Exception ex)
        {
            Console.WriteLine("Error : " + ex.Message);
        }

        return accounts;
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