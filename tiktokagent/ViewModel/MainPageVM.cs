using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenQA.Selenium.Chrome;
using tiktokagent.Core.Usecases;
using tiktokagent.Domain;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager;
using OpenQA.Selenium;
using WebDriverManager.Helpers;

namespace tiktokagent.ViewModel;

public partial class MainPageVm: ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Account> _accounts;

    [ObservableProperty]
    private Account _selectedAccount;


    [ObservableProperty]
    private string _numberOfAccount;
    
    [ObservableProperty]
    private string _textOnStartButton;
    private readonly IObtainAccounts _accountRepository;
    
    public MainPageVm()
    {
        //_accountRepository = accountRepository;
        _accounts = new ObservableCollection<Account>();
        _accounts.Add(new Account("test@gmail.com", "test", "test", Status.Active));
        _accounts.Add(new Account("test@gmail.com", "test", "test", Status.Active));
        TextOnStartButton = "Commencer la génération de compte";
        
    }
  
    [RelayCommand]
    private void StartBotting()
    {
        _ = InitializeAccountsListAsync();
        Start();

    }

   
    private Task InitializeAccountsListAsync()
    {

        var isAValidNumber = CheckIfNumberOfAccountContainValidNumber();
        if (!isAValidNumber)
        {
            return null;
        }
        
        var numberOfAccounts = int.Parse(NumberOfAccount);
       
        var accountGenerator = new AccountManager(_accountRepository);
        accountGenerator.GenerateAccounts(numberOfAccounts).ForEach(account => Accounts.Add(account));
        
        
        return null;

    }
    
    
    private bool CheckIfNumberOfAccountContainValidNumber()
    {
        var number = int.TryParse(NumberOfAccount, out _);
        return number;
    }

    private void Start()
    {
        new DriverManager().SetUpDriver(
            "https://chromedriver.storage.googleapis.com/114.0.5735.16/chromedriver_win32.zip",
            Path.Combine(Directory.GetCurrentDirectory(), "chromedrivser.exe"),
            "chromedriver.exe");
        var _webDriver = new ChromeDriver("C:\\Users\\franc\\RiderProjects\\tiktokagent\\tiktokagent\\Core");
        _webDriver.Navigate().GoToUrl("https://www.tiktok.com");

         
    }

}