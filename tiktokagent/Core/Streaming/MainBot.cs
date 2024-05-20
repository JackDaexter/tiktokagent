using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Remote;
using tiktokagent.Domain;
using WebDriverManager;
using OpenQA.Selenium.Appium.Service;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using SeleniumUndetectedChromeDriver;
using Selenium.Extensions;
using OpenQA.Selenium.Appium.Enums;
using System.Threading.Channels;
using System.Collections.Concurrent;
using tiktokagent.Messaging;
using CommunityToolkit.Mvvm.ComponentModel;
namespace tiktokagent.Core.Streaming;

public enum BotStatus
{
    Running,
    Stopped,
    Inactive
}

public partial class MainBot : ObservableObject
{

    [ObservableProperty]
    public BotStatus _botStatus;

    [ObservableProperty]
    public Account _account;

    public ChromeDriver _webDriver;
    public ConcurrentQueue<InterThreadEvents> _channel;
    public AppiumDriver _appiumDriver;
    public WebDriverWait wait;

    public MainBot(Account account, ConcurrentQueue<InterThreadEvents> channel)
    {
        _account = account;
        this._channel = channel;
        this.BotStatus = BotStatus.Inactive;
    }


    public static bool ConnexionPageIsPresent(ChromeDriver _webDriver)
    {
        try
        {
            _webDriver.FindElement(By.XPath("//*[@id=\"loginContainer\"]/div[2]/div"));
            return true;
        }
        catch (NoSuchElementException e)
        {
            return false;
        }
    }

    public static bool WeAreOnPhoneAccountCreation(ChromeDriver _webDriver)
    {
        try
        {
            _webDriver.FindElement(By.XPath("//*[@id=\"loginContainer\"]/div[2]/form/div[1]/a"));
            return true;
        }
        catch (NoSuchElementException e)
        {
            return false;
        }
    }

    public static bool CaptchaIsAsked(ChromeDriver _webDriver)
    {
        try
        {
            _webDriver.FindElement(By.XPath("//*[@id=\"captcha_container\"]/div"));
            return true;
        }
        catch (NoSuchElementException e)
        {
            return false;
        }
    }
    public static bool SubscribeLinkIsPresent(ChromeDriver _webDriver)
    {
        try
        {
            _webDriver.FindElement(By.XPath("//*[@id=\"login-modal\"]/div[1]/div[3]/a"));
            return true;
        }
        catch (NoSuchElementException e)
        {
            return false;
        }
    }

    

    public void FillConnexionField(ChromeDriver _webDriver)
    {
        var wasBlocked = false;
        _webDriver
            .FindElement(By.XPath("//*[@id=\"loginContainer\"]/div[2]/form/div[1]/input"))
            .SendKeys(Account.Email); // Button to switch from email to telephone

        _webDriver
            .FindElement(By.XPath("//*[@id=\"loginContainer\"]/div[2]/form/div[2]/div/input"))
            .SendKeys(Account.Password); // Button to switch from email to telephone
        _webDriver.FindElement(By.XPath("//*[@id=\"loginContainer\"]/div[2]/form/button")).Click(); // Button to switch from email to telephone
        WaitWhileCaptchaPresent(ref wasBlocked);
        if (wasBlocked)
        {
           _webDriver.FindElement(By.XPath("//*[@id=\"loginContainer\"]/div[2]/form/button")).Click(); // Button to switch from email to telephone
        }
    }

    public void FillBirthDate(ChromeDriver _webDriver)
    {
        var monthOfBirth = new Random().Next(1, 12);
        var dayOfBirth = new Random().Next(1, 30);
        var yearOfBirth = new Random().Next(21, 45);
        
        _webDriver.FindElement(By.XPath("//*[@id=\"loginContainer\"]/div[2]/form/div[2]/div[1]")).Click(); //Month
        _webDriver.FindElement(By.XPath($"//*[@id=\"Month-options-item-{monthOfBirth}\"]")).Click(); //Month

        Thread.Sleep(1000);
        _webDriver.FindElement(By.XPath("//*[@id=\"loginContainer\"]/div[2]/form/div[2]/div[2]")).Click(); //Day
        _webDriver.FindElement(By.XPath($"//*[@id=\"Day-options-item-15\"]")).Click(); //Day


        _webDriver.FindElement(By.XPath($"//*[@id=\"loginContainer\"]/div[2]/form/div[2]/div[3]")).Click(); //Year
        _webDriver.FindElement(By.XPath($"//*[@id=\"Year-options-item-{yearOfBirth}\"]")).Click(); //Year

    }
 
    private async void BrowserSetUp()
    {
        List<string> ls = new List<string>();
        ls.Add("enable-automation");
        ls.Add("excludeSwitches");
        ls.Add("enable-logging");
        ls.Add("disable-popup-blocking");
        ChromeOptions options = new ChromeOptions();
        if (_account != null && _account.Proxy != null)
        {
            options.AddArgument($"--proxy-server={_account.Proxy.Ip}:{_account.Proxy.Port}");
        }
        options.AddArgument("--disable-popup-blocking");
        options.AddArgument("--ignore-certificate-errors");
        options.AddAdditionalOption("useAutomationExtension", false);
        options.AddAdditionalOption("androidPackage", "com.android.chrome");
        // options.AddArgument("--incognito");
        options.AddArgument("--disable-blink-features=AutomationControlled");
        //options.AddExcludedArguments(ls);
        //options.AddArgument("--user-agent=Mozilla/5.0 (iPhone; CPU iPhone OS 10_3 like Mac OS X) AppleWebKit/602.1.50 (KHTML, like Gecko) CriOS/56.0.2924.75 Mobile/14E5239e Safari/602.1");
        options.AddArgument("user-agent=Mozilla/5.0 (Linux; Android 10; K) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.0.0 Mobile Safari/537.36");

     
        _webDriver = new ChromeDriver();
        wait = new WebDriverWait(_webDriver,TimeSpan.FromSeconds(50));
        _webDriver.ExecuteScript("Object.defineProperty(navigator, 'webdriver', {get: () => undefined})");

    }


    private async  void FieldMensonge()
    {
        Uri uri = new Uri("https://www.microsoft.com");
        BrowserLaunchOptions options = new BrowserLaunchOptions()
        {
            LaunchMode = BrowserLaunchMode.SystemPreferred,
            TitleMode = BrowserTitleMode.Show,
            PreferredToolbarColor = Colors.Violet,
            PreferredControlColor = Colors.SandyBrown
        };

        await Browser.Default.OpenAsync(uri, options);
    }


    public void Subscription()
    {

        //BrowserSetUp();
        //MobileSetUp();
        FieldMensonge();
        /*_webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

        _webDriver.Navigate().GoToUrl("https://www.tiktok.com/discover");

        Actions builder = new Actions(_webDriver); // Action method in interactions Lib use for DoubleClick()
        builder.SendKeys(Keys.Escape).Perform();*/


        Thread.Sleep(20000);
        /*_webDriver.FindElement(By.XPath("//*[@id=\"header-login-button\"]")).Click();

        if (SubscribeLinkIsPresent(_webDriver))
        {
            _webDriver.FindElement(By.XPath("//*[@id=\"login-modal\"]/div[1]/div[3]/a")).Click();
        }

        _webDriver.FindElement(By.XPath("//*[@id=\"loginContainer\"]/div/div/div/div[2]/div[1]/div[2]")).Click();

        _webDriver.FindElement(By.XPath("//*[@id=\"loginContainer\"]/div/form/div[4]/a")).Click(); // Subscribe with email
        FillBirthDate(_webDriver);
        if (_account != null)
        {
            _webDriver
            .FindElement(By.XPath("//*[@id=\"loginContainer\"]/div[2]/form/div[5]/div/input"))
            .SendKeys(_account.Email);

            _webDriver
                .FindElement(By.XPath("//*[@id=\"loginContainer\"]/div[2]/form/div[6]/div/input"))
                .SendKeys(_account.Password);
        }
        else
        {
            _webDriver
                .FindElement(By.XPath("//*[@id=\"loginContainer\"]/div[2]/form/div[5]/div/input"))
                .SendKeys(_accountProxied.Email);

            _webDriver
                .FindElement(By.XPath("//*[@id=\"loginContainer\"]/div[2]/form/div[6]/div/input"))
                .SendKeys(_accountProxied.Password);
        }

        var element = _webDriver.FindElement(By.XPath("//*[@id=\"login-modal\"]/div[1]"));

        _webDriver.ExecuteScript("arguments[0].scrollIntoView(true);", element);
        _webDriver.FindElement(By.XPath("//*[@id=\"loginContainer\"]/div[2]/form/div[7]/div/button")).Click(); //Envoyer le code

        Thread.Sleep(1500);

        _webDriver.FindElement(By.XPath("//*[@id=\"loginContainer\"]/div[2]/form/div[7]/div/button")).Click(); //Envoyer le code

        if (_account != null)
        {
            _account.Status = Status.Captcha;
        }
        else
        {
            AccountProxied.Status = Status.Captcha;
        }*/
    }

    public void Start()
    {
        Account.Status = Status.Running;
        BrowserSetUp();

        _webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

        _webDriver.Navigate().GoToUrl("https://www.tiktok.com");
        Thread.Sleep(2000);

        Actions builder = new Actions(_webDriver); // Action method in interactions Lib use for DoubleClick()
        builder.SendKeys(Keys.Escape).Perform();
        _webDriver.FindElement(By.XPath("//*[@id=\"header-login-button\"]")).Click();

        wait.Until(_webDriver => _webDriver.FindElement(By.XPath("//*[@id=\"loginContainer\"]"))); // Wait for login container to appear
        _webDriver.FindElement(By.XPath("//*[@id=\"loginContainer\"]/div/div/div/div[2]")).Click();
        wait.Until(_webDriver =>
            _webDriver.FindElement(By.XPath("//*[@id=\"loginContainer\"]/div/form/div[1]/a"))
        );

        if (MainBot.ConnexionPageIsPresent(_webDriver))
        {
            FillConnexionField(_webDriver);
        }
        else
        {
            _webDriver
                .FindElement(By.XPath("//*[@id=\"loginContainer\"]/div/form/div[1]/a"))
                .Click(); // Button to swwitch from connexion to inscription

            if (MainBot.WeAreOnPhoneAccountCreation(_webDriver))
            {
                _webDriver
                    .FindElement(By.XPath("//*[@id=\"loginContainer\"]/div[2]/form/div[1]/a"))
                    .Click(); // Button to switch from Telephone to username
            }

            FillConnexionField(_webDriver);

        }
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    private void WaitWhileCaptchaPresent(ref bool wasBlocked)
    {
        while (CaptchaIsAsked(_webDriver))
        {
            Account.Status = Account.Status == Status.Captcha ? Account.Status : Status.Captcha ;
            Thread.Sleep(10000);
            wasBlocked = true;

        }
        Account.Status = Status.Running;
    }
}
