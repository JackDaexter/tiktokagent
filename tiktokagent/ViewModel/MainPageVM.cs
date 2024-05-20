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
using tiktokagent.Core.Infrastructure;
using tiktokagent.Core.Streaming;
using tiktokagent.Core.Usecases;
using tiktokagent.Domain;
using tiktokagent.Messaging;

using Status = tiktokagent.Domain.Status;

namespace tiktokagent.ViewModel;

public partial class MainPageVm : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Account> _accounts;

    [ObservableProperty]
    private ObservableCollection<MainBot> _bottingInstances;

    private Collection<Task> _threads;
    private Channel<InterThreadEvents> channel = Channel.CreateUnbounded<InterThreadEvents>();

    [ObservableProperty]
    private Account _selectedAccount;
    
    [ObservableProperty]
    private bool _isAdmin;

    [ObservableProperty]
    private bool _loading;

    [ObservableProperty]
    private string _numberOfAccount;

    [ObservableProperty]
    private string _textOnStartButton;

    private readonly IObtainAccounts _accountRepository;

    public MainPageVm()
    {
        WeakReferenceMessenger.Default.Send(new AppEvents(ApplicationEvents.AccountLoaded)); ;

        _accountRepository = new AccountFileAdapter("C:\\Users\\franc\\RiderProjects\\tiktokagent\\tiktokagent\\Core\\accounts.json");
        _accounts = new ObservableCollection<Account>();
        _threads = new ObservableCollection<Task>();
        _bottingInstances = new ObservableCollection<MainBot>();
        TextOnStartButton = "Commencer le streaming";
        LoadAccountFromRepository();
    }

    [RelayCommand]
    private void StartBotting()
    {
        foreach (var bot in BottingInstances)
        {
            if (bot.Account.Status.Equals(Status.Inactive))
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
        var accounts = await _accountRepository.LoadAllAccountsAsync();
        accounts.ForEach(account => {
            var isElementAlreadyPresent = Accounts.Where(e => e.Email.Equals(account.Email));
            if (isElementAlreadyPresent.Count() == 0 )
            {
                Accounts.Add(account);

            }
        });
        LoadAccountInstances();
        WeakReferenceMessenger.Default.Send(new AppEvents(ApplicationEvents.AccountLoaded)); ;

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
                    await SecureStorage.Default.SetAsync("FileName", result.FileName);
                    await SecureStorage.Default.SetAsync("FilePath", result.FullPath);

                    var docPath = result.FullPath;
                    var jsonContent = File.ReadAllText(Path.Combine(docPath, result.FileName));
                   
                    using StreamReader reader = new(Path.Combine(docPath, result.FileName));
                    using var stream = await result.OpenReadAsync();
                    var accounts = System.Text.Json.JsonSerializer.Deserialize<List<Account>>(jsonContent);

                    Accounts = new ObservableCollection<Account>(accounts);

                }
            }

        }
        catch (Exception ex)
        {
            // The user canceled or something went wrong
        }

        Loading = false;
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

        for (var i = 0; i < Accounts.Count; i++)
        {
            var mainBot = new MainBot(Accounts[i], null);
            BottingInstances.Add(mainBot);
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
        Accounts.Add(account);
    }
}

