using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Channels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using tiktokagent.Core.Domain;
using tiktokagent.Core.Infrastructure;
using tiktokagent.Core.Streaming;
using tiktokagent.Core.Usecases;
using tiktokagent.Domain;
using tiktokagent.Messaging;

namespace tiktokagent.ViewModel;

public partial class MainPageVm : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Account> _accounts;

    [ObservableProperty]
    private ObservableCollection<SimpleProxy> _proxies;

    [ObservableProperty]
    private ObservableCollection<MainBot> _bottingInstances;

    private Collection<Task> _threads;
    private Channel<InterThreadEvents> channel = Channel.CreateUnbounded<InterThreadEvents>();

    [ObservableProperty]
    private MainBot _selectedAccount;


    public bool SelectedAccountIsSelected;

    [ObservableProperty]
    private bool _loading;

    [ObservableProperty]
    private string _numberOfAccount;

    [ObservableProperty]
    private string _textOnStartButton;

    private readonly IObtainAccounts _accountRepository;

    public MainPageVm()
    {
        _accountRepository = new AccountFileAdapter("C:\\Users\\franc\\RiderProjects\\tiktokagent\\tiktokagent\\Core\\accounts.json");
        _accounts = new ObservableCollection<Account>();
        _threads = new ObservableCollection<Task>();
        _bottingInstances = new ObservableCollection<MainBot>();
        _selectedAccount = null;
        _proxies = new ObservableCollection<SimpleProxy>();
        SelectedAccountIsSelected = SelectedAccount == null;
        TextOnStartButton = "Commencer le streaming";
        LoadAccountFromRepository();
    }
   
    [RelayCommand]
    private void RemoveSelectedAccount()
    {
        if(SelectedAccount == null)
        {
            WeakReferenceMessenger.Default.Send(new AppEvents(ApplicationEvents.AccountRemoved));

            return;
        }
        else
        {
            var itemToRemove = BottingInstances.FirstOrDefault(i => i.Account.Email == SelectedAccount.Account.Email);

            if (itemToRemove != null)
            {
                BottingInstances.Remove(itemToRemove);
                var accounts = BottingInstances.Select(elem => elem.Account).ToList();
                Accounts = new ObservableCollection<Account>(accounts);
                _accountRepository.SaveMultipleAccounts(accounts);
            }
            WeakReferenceMessenger.Default.Send(new AppEvents(ApplicationEvents.AccountRemoved));

        }

    }

    [RelayCommand]
    private void SaveAccounts()
    {
        _accountRepository.SaveMultipleAccounts(Accounts.ToList());
        WeakReferenceMessenger.Default.Send(new AppEvents(ApplicationEvents.AccountLoaded)); ;
    }


    [RelayCommand]
    private void StartBotting()
    {
        foreach (var bot in BottingInstances)
        {
            if (!bot.BotStatus.Equals(BotStatus.Running))
            {
                _threads.Add(Task.Run(() =>
                {
                    try
                    {
                        bot.BotStatus = BotStatus.Running;
                        bot.Start();
                    }
                    catch (Exception)
                    {
                        bot.BotStatus = BotStatus.Stopped;
                    }
                }));

            }
            
        }
      
    }

    private async Task LoadAccountFromRepository()
    {
        await LoadProxyFromRepository();

        var accounts = await _accountRepository.LoadAllAccountsAsync();

        accounts.ForEach(account => {
            var isElementAlreadyPresent = Accounts.Where(e => e.Email.Equals(account.Email));
            if (!isElementAlreadyPresent.Any())
            {
                Accounts.Add(account);
            }
        });
        LoadAccountInstances();
        WeakReferenceMessenger.Default.Send(new AppEvents(ApplicationEvents.AccountLoaded)); ;

    }
    
    private async Task LoadProxyFromRepository()
    {
        var proxies = await _accountRepository.LoadAllProxyAsync();

        proxies.ForEach(proxy => {
            Proxies.Add(proxy);

            var isElementAlreadyPresent = Proxies.Where(e => e.Ip.Equals(proxy.Ip));
            if (!isElementAlreadyPresent.Any())
            {
                Proxies.Add(proxy);
            }
        });
    }

    [RelayCommand]
    private async Task LoadSavedAccounts()
    {
        Loading = true;
        var accounts = await _accountRepository.LoadAllAccountsAsync();
        Accounts = new ObservableCollection<Account>(accounts);
        Loading = false;
    }
    
    [RelayCommand]
    private async Task LoadProxyFromFile()
    {
        try
        {
            var result = await FilePicker.Default.PickAsync();
            if (result != null)
            {
                if (result.FileName.EndsWith("txt", StringComparison.OrdinalIgnoreCase))
                {
                    var docPath = Path.GetDirectoryName(result.FullPath);

                    if(docPath == null)
                    {
                        WeakReferenceMessenger.Default.Send(new AppEvents(ApplicationEvents.FilePathError));
                        return;
                    }
                    
                    await ParseAndExtractDataFromTxtFile(result, docPath);

                }
            }
        }
        catch (Exception)
        {
            
        }
    }


    [RelayCommand]
    private async Task LoadAccountFromAnotherFile()
    {
        Loading = true;

        try
        {
            var result = await FilePicker.Default.PickAsync();
            if (result != null)
            {
                if (result.FileName.EndsWith("json", StringComparison.OrdinalIgnoreCase))
                {
                    var docPath = Path.GetDirectoryName(result.FullPath);

                    if(docPath == null)
                    {
                        WeakReferenceMessenger.Default.Send(new AppEvents(ApplicationEvents.FilePathError));
                        return;
                    }
                    
                    var jsonContent = await ParseAndExtractDataFromJsonFile(result, docPath);

                    var accounts = System.Text.Json.JsonSerializer.Deserialize<List<Account>>(jsonContent);

                    Accounts = new ObservableCollection<Account>(accounts);
                    BottingInstances.Clear();
                    foreach (var account in Accounts)
                    {
                        BottingInstances.Add(new MainBot(account, null));
                    }
                }
            }

        }
        catch (Exception ex)
        {
            // The user canceled or something went wrong
        }

        Loading = false;
    }

    private static async Task<string> ParseAndExtractDataFromJsonFile(FileResult result, string docPath)
    {
        await SecureStorage.Default.SetAsync("FileName", result.FileName);
        await SecureStorage.Default.SetAsync("FilePath", docPath);

        var jsonContent = await File.ReadAllTextAsync(Path.Combine(docPath, result.FileName));
        return jsonContent;
    }
    
    private async Task ParseAndExtractDataFromTxtFile(FileResult result, string docPath)
    {
        await SecureStorage.Default.SetAsync("ProxyFileName", result.FileName);
        await SecureStorage.Default.SetAsync("ProxyFilePath", docPath);

        string[] lines = File.ReadAllLines(Path.Combine(docPath, result.FileName));

        if(lines.Length == 0)
        {
            _proxies.Clear();
            WeakReferenceMessenger.Default.Send(new AppEvents(ApplicationEvents.ErrorNumberOfAccounts));
            return;
        }
        foreach(string line in lines)
        {
            string[] items = line.Split(':');
            int myInteger = int.Parse(items[1]);   // Here's your integer.

            _proxies.Add(new SimpleProxy(items[0], myInteger));
        }
    }

    [RelayCommand]
    private async Task RefreshDataWithAccountFile()
    {
        Loading = true;

        try
        {
            string fileNameFromStorage = await SecureStorage.Default.GetAsync("FileName");
            string filePathFromStorage = await SecureStorage.Default.GetAsync("FilePath");
            string docPath = filePathFromStorage ?? Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fileName = fileNameFromStorage ?? "accounts.json";

            var jsonContent = File.ReadAllText(Path.Combine(docPath, fileName));

            using StreamReader reader = new(Path.Combine(docPath, fileName));

            var accounts = System.Text.Json.JsonSerializer.Deserialize<List<Account>>(jsonContent);
            accounts.ForEach(account => Accounts.Add(account));

            Accounts = new ObservableCollection<Account>(accounts);

        }
        catch (Exception ex)
        {
            // The user canceled or something went wrong
        }

        Loading = false;
    }


    private bool CheckIfNumberOfAccountContainValidNumber()
    {
        var number = int.TryParse(NumberOfAccount, out _);
        return number;
    }


    private void LoadAccountInstances()
    {
        Random rnd = new Random();
        int proxySelected = rnd.Next(0, Proxies.Count);
        foreach (var account in Accounts)
        {
            if (Proxies.Count == 0)
            { 
                var mainBot = new MainBot(account, null);
                BottingInstances.Add(mainBot);

            }
            else
            {
                var proxy = Proxies[proxySelected];
                var mainBot = new MainBot(account, proxy);
                BottingInstances.Add(mainBot);

            }
        }
        WeakReferenceMessenger.Default.Send(new AppEvents(ApplicationEvents.AccountLoaded)); ;

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

    public void AddNewAccount(Account account)
    {

        var existingAccount = BottingInstances.FirstOrDefault(i => i.Account.Email == account.Email);
        if (existingAccount != null) return;
        Accounts.Add(account);
        var mainBot = new MainBot(account, null);
        BottingInstances.Add(mainBot);

        _accountRepository.SaveMultipleAccounts(Accounts.ToList());
        WeakReferenceMessenger.Default.Send(new AppEvents(ApplicationEvents.AccountSaved));
    }
}

