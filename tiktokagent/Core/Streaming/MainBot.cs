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
using tiktokagent.Core.Domain;

namespace tiktokagent.Core.Streaming;

public enum BotStatus
{
    Running,
    Stopped,
    Inactive
}
public enum BrowserStatus
{
    Active,
    Inactive,
    Shadowbanned,
    Suspended,
    Testing,
    Captcha,
    Code,
    Running
}



public partial class MainBot : ObservableObject
{

    [ObservableProperty]
    public BotStatus _botStatus;

    [ObservableProperty]
    public Account _account;

    [ObservableProperty]
    public int _numberOfStream;
    
    [ObservableProperty]
    public BrowserStatus _browserStatus;

    public ChromeDriver _webDriver;
    public SimpleProxy _proxy;
    public AppiumDriver _appiumDriver;
    public WebDriverWait wait;

    List<string> tikTokTrends = new List<string>()
    {
        "Grocery List Duel",
        "String Magic Show",
        "Word Chain Addict",
        "Never List Challenge",
        "String Clue Hunt",
        "Empty Fridge Blues",
        "First Last Word Show",
        "Double Trouble List",
        "ABC Sort Race",
        "String Quiz Test",
        "Scrambled Fix",
        "Would You String?",
        "Rhyme Time List",
        "Mad Libs Twist",
        "Blindfold Word Hunt",
        "String Sort Race",
        "Story Time List",
        "Dance String List",
        "Don't Say String",
        "Reverse Charades",
        "Creative Skit",
        "Rap List Methods",
        "Comment Quiz List",
        "Duet Your Twist",
        "Best Trend Compil"
    };
    public MainBot(Account account, SimpleProxy proxy)
    {
        _account = account;
        this._proxy = proxy;
        this.BotStatus = BotStatus.Inactive;
        _numberOfStream = 0;
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
    public static bool VerificationCodeIsAsked(ChromeDriver _webDriver)
    {
        try
        {
            _webDriver.FindElement(By.XPath("/html/body/div[9]/div[2]"));
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
        WaitWhileCodeVerificationIsPresent(ref wasBlocked);
        /*if (wasBlocked)
        {
           _webDriver.FindElement(By.XPath("//*[@id=\"loginContainer\"]/div[2]/form/button")).Click(); // Button to switch from email to telephone
        }*/
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
        if (_proxy != null)
        {
            options.AddArgument($"--proxy-server={_proxy.Ip}:{_proxy.Port}");
        }
        options.AddArgument("--disable-popup-blocking");
        options.AddArgument("--ignore-certificate-errors");
        options.AddAdditionalOption("useAutomationExtension", false);
        //options.AddAdditionalOption("androidPackage", "com.android.chrome");
        // options.AddArgument("--incognito");
        options.AddArgument("--disable-blink-features=AutomationControlled");
        options.AddArgument("--mute-audio");
        //options.AddExcludedArguments(ls);
        //options.AddArgument("--user-agent=Mozilla/5.0 (iPhone; CPU iPhone OS 10_3 like Mac OS X) AppleWebKit/602.1.50 (KHTML, like Gecko) CriOS/56.0.2924.75 Mobile/14E5239e Safari/602.1");

     
        _webDriver = new ChromeDriver(options);
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


    public void Start()
    {
        _browserStatus = BrowserStatus.Running;
        BrowserSetUp();

        _webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(20);

        _webDriver.Navigate().GoToUrl("https://www.tiktok.com");
        Thread.Sleep(2000);

        if (_webDriver.Url.Contains("redirect_url"))
        {
            bool wasBlocked = false;
            _webDriver.FindElement(By.XPath("//*[@id=\"loginContainer\"]/div/div/div/div[3]/div[2]")).Click();
            _webDriver.FindElement(By.XPath("//*[@id=\"loginContainer\"]/div[1]/form/div[1]/a"))
                .Click(); // Button to switch from inscription  to connexion
            
            _webDriver
                .FindElement(By.XPath("//*[@id=\"loginContainer\"]/div[1]/form/div[1]/input"))
                .SendKeys(Account.Email); 
            
            _webDriver
                .FindElement(By.XPath("//*[@id=\"loginContainer\"]/div[1]/form/div[2]/div/input"))
                .SendKeys(Account.Password); 
            
            _webDriver.FindElement(By.XPath("//*[@id=\"loginContainer\"]/div[1]/form/button")).Click(); // Button to switch from email to telephone
            WaitWhileCaptchaPresent(ref wasBlocked);
            WaitWhileCodeVerificationIsPresent(ref wasBlocked);
            Thread.Sleep(2000);
            _webDriver.Navigate().GoToUrl("https://www.tiktok.com/search");
        }
        else
        {
            Actions builder = new Actions(_webDriver); // Action method in interactions Lib use for DoubleClick()
            builder.SendKeys(Keys.Escape).Perform();
            _webDriver.FindElement(By.XPath("//*[@id=\"header-login-button\"]")).Click();

            wait.Until(_webDriver => _webDriver.FindElement(By.XPath("//*[@id=\"loginContainer\"]"))); // Wait for login container to appear
            _webDriver.FindElement(By.XPath("//*[@id=\"loginContainer\"]/div/div/div/div[2]")).Click();
            wait.Until(_webDriver =>
                _webDriver.FindElement(By.XPath("//*[@id=\"loginContainer\"]/div/form/div[1]/a"))
            );
        }

        if (MainBot.ConnexionPageIsPresent(_webDriver))
        {
            FillConnexionField(_webDriver);
        }
        else
        {
            _webDriver
                .FindElement(By.XPath("//*[@id=\"loginContainer\"]/div/form/div[1]/a"))
                .Click(); // Button to switch from inscription  to connexion 

            if (MainBot.WeAreOnPhoneAccountCreation(_webDriver))
            {
                _webDriver
                    .FindElement(By.XPath("//*[@id=\"loginContainer\"]/div[2]/form/div[1]/a"))
                    .Click(); // Button to switch from Telephone to username
            }

            FillConnexionField(_webDriver);

        }
        StreamWhileNonStop();
    }

    private void StreamWhileNonStop()
    {
        Random randomNumber = new Random();
      
        while (true)
        {
            int trendToSearch = randomNumber.Next(0, tikTokTrends.Count - 1);
            int videoToSelect = randomNumber.Next(0, 6);
            int waitBetweenNextVideo = randomNumber.Next(0, 25);
            int numberVideoAfterNextSearch = randomNumber.Next(3, 25);

            _webDriver
                .FindElement(By.XPath("//*[@id=\"app-header\"]/div/div[2]/div/form/input"))
                .SendKeys(tikTokTrends[trendToSearch]);
            _webDriver
                 .FindElement(By.XPath($"//*[@id=\"app-header\"]/div/div[2]/div/form/button")).Click(); // click on search

            Thread.Sleep(5000);
            /*var link = _webDriver
                 .FindElement(By.XPath($"//*[@id=\"tabs-0-panel-search_top\"]/div/div/div[{videoToSelect}]/div[1]/div/div/a"))
                 .GetAttribute("href");*/
            _webDriver
                 .FindElement(By.XPath($"//*[@id=\"tabs-0-panel-search_top\"]/div/div/div[{videoToSelect}]/div[1]")).Click();

            //_webDriver.Navigate().GoToUrl(link);

            StreamVideo();
         
        }
    }

    private void StreamVideo()
    {
        Random randomNumber = new Random();
        //*[@id="main-content-video_detail"]/div/div[2]/div/div[1]/div[1]/div[3]/div[1]/button[2]
        int waitBetweenNextVideo = randomNumber.Next(15, 45);
        int numberVideoAfterNextSearch = randomNumber.Next(3, 25);
        int waitTime = randomNumber.Next(1, 5);
        for (int i = 0;  i < numberVideoAfterNextSearch; i++)
        {
            Thread.Sleep(TimeSpan.FromSeconds(waitBetweenNextVideo));
            _webDriver
                .FindElement(By.XPath($"//*[@id=\"tabs-0-panel-search_top\"]/div[3]/div/div[1]/button[3]")).Click(); // next video
           

            if(i % numberVideoAfterNextSearch == 0)
            {
                Thread.Sleep(TimeSpan.FromMinutes(waitTime));
            }
            NumberOfStream += 1;
        }
        _webDriver
               .FindElement(By.XPath($"//*[@id=\"tabs-0-panel-search_top\"]/div[3]/div/div[1]/button[1]")).Click(); // close button  
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    private void WaitWhileCaptchaPresent(ref bool wasBlocked)
    {
        while (CaptchaIsAsked(_webDriver))
        {
            BrowserStatus = BrowserStatus == BrowserStatus.Captcha ? BrowserStatus : BrowserStatus.Captcha ;
            Thread.Sleep(3000);
            wasBlocked = true;

        }
        BrowserStatus = BrowserStatus.Running;
    }    
    private void WaitWhileCodeVerificationIsPresent(ref bool wasBlocked)
    {
        while (VerificationCodeIsAsked(_webDriver))
        {
            BrowserStatus = BrowserStatus == BrowserStatus.Captcha ? BrowserStatus : BrowserStatus.Code ;
            Thread.Sleep(3000);
            wasBlocked = true;

        }
        BrowserStatus = BrowserStatus.Running;
    }

    public void CloseStreaming()
    {
        try
        { 
            BrowserStatus = Streaming.BrowserStatus.Inactive;
        _webDriver.Quit();
        }catch(Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
