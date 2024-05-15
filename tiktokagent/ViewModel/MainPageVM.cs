using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using tiktokagent.Core.Infrastructure;
using tiktokagent.Core.Streaming;
using tiktokagent.Core.Usecases;
using tiktokagent.Domain;
using tiktokagent.Messaging;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager.Helpers;
using Status = tiktokagent.Domain.Status;

namespace tiktokagent.ViewModel;

public partial class MainPageVm : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Account> _accounts;

    [ObservableProperty]
    private ObservableCollection<MainBot> _bottingInstances;

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
        _accountRepository = new AccountFileAdapter("C:\\Users\\franc\\RiderProjects\\tiktokagent\\tiktokagent\\Core\\accounts.json");
        _accounts = new ObservableCollection<Account>();
        _bottingInstances = new ObservableCollection<MainBot>();
        TextOnStartButton = "Commencer le streaming";
        _accountRepository.LoadAllAccounts().ForEach(account => Accounts.Add(account));
    }

    [RelayCommand]
    private void StartBotting()
    {
        Start();
    }
    
    [RelayCommand]
    private void CreateRandomAccounts()
    {
        Loading = true;
        var status = InitializeAccountsListAsync();
        Loading = false;

        if (!status) return;
    }

    [RelayCommand]
    private void LoadSavedAccounts()
    {
        Loading = true;
        var accounts = _accountRepository.LoadAllAccounts();
        accounts.ForEach(account => Accounts.Add(account));

    }

    [RelayCommand]
    private void CreateTestsAccounts()
    {
        Loading = true;
        var status = InitializeAccountsListAsync();
        Loading = false;

        if (!status) return;
    }

    private bool InitializeAccountsListAsync()
    {
        var isAValidNumber = CheckIfNumberOfAccountContainValidNumber();
        if (!isAValidNumber)
        {
            WeakReferenceMessenger.Default.Send(new AppElements(InteractionStatus.ErrorNumberOfAccounts));
            return false;
        }

        var numberOfAccounts = int.Parse(NumberOfAccount);

        var accountGenerator = new AccountManager(_accountRepository);
        accountGenerator
            .GenerateAccounts(numberOfAccounts)
            .ForEach(account => Accounts.Add(account));

        return true;
    }

    private bool CheckIfNumberOfAccountContainValidNumber()
    {
        var number = int.TryParse(NumberOfAccount, out _);
        return number;
    }

    private void Start()
    {
        
        for(var i = 0; i < _accounts.Count; i++)
        {
            var mainBot = new MainBot(_accounts[i]);
            BottingInstances.Add(mainBot);
        }
        foreach (var bottingInstance in BottingInstances)
        {
            bottingInstance.Start();
        }

    }
}
