using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using tiktokagent.Domain;

namespace tiktokagent.Core.Streaming;

public class MainBot
{
    public Account _account;
    //public AccountProxied _accountProxied;

    public MainBot(Account account)
    {
        _account = account;
    }

    public static bool ConnexionPageIsPresent(ChromeDriver webDriver)
    {
        try
        {
            webDriver.FindElement(By.XPath("//*[@id=\"loginContainer\"]/div[2]/div"));
            return true;
        }
        catch (NoSuchElementException e)
        {
            return false;
        }
    }

    public static bool WeAreOnPhoneAccountCreation(ChromeDriver webDriver)
    {
        try
        {
            webDriver.FindElement(By.XPath("//*[@id=\"loginContainer\"]/div[2]/form/div[1]/a"));
            return true;
        }
        catch (NoSuchElementException e)
        {
            return false;
        }
    }

    public static bool CaptchaIsAsked(ChromeDriver webDriver)
    {
        try
        {
            webDriver.FindElement(By.XPath("//*[@id=\"captcha_container\"]/div"));
            return true;
        }
        catch (NoSuchElementException e)
        {
            return false;
        }
    }
    public static bool SubscribeLinkIsPresent(ChromeDriver webDriver)
    {
        try
        {
            webDriver.FindElement(By.XPath("//*[@id=\"login-modal\"]/div[1]/div[3]/a"));
            return true;
        }
        catch (NoSuchElementException e)
        {
            return false;
        }
    }

    public void FillConnexionField(ChromeDriver webdriver)
    {
        webdriver
            .FindElement(By.XPath("//*[@id=\"loginContainer\"]/div[2]/form/div[1]/input"))
            .SendKeys("salut@gmail.com"); // Button to switch from email to telephone

        webdriver
            .FindElement(By.XPath("//*[@id=\"loginContainer\"]/div[2]/form/div[2]/div/input"))
            .SendKeys("84ze9gf6sd15fd"); // Button to switch from email to telephone
        webdriver.FindElement(By.XPath("//*[@id=\"loginContainer\"]/div[2]/form/button")).Click(); // Button to switch from email to telephone
    }

    public void FillBirthDate(ChromeDriver webDriver)
    {
        var monthOfBirth = new Random().Next(1, 12);
        var dayOfBirth = new Random().Next(1, 30);
        var yearOfBirth = new Random().Next(21, 45);
        
        webDriver.FindElement(By.XPath("//*[@id=\"loginContainer\"]/div[2]/form/div[2]/div[1]")).Click(); //Month
        webDriver.FindElement(By.XPath($"//*[@id=\"Month-options-item-{monthOfBirth}\"]")).Click(); //Month

        Thread.Sleep(1000);
        webDriver.FindElement(By.XPath("//*[@id=\"loginContainer\"]/div[2]/form/div[2]/div[2]")).Click(); //Day
        webDriver.FindElement(By.XPath($"//*[@id=\"Day-options-item-15\"]")).Click(); //Day


        webDriver.FindElement(By.XPath($"//*[@id=\"loginContainer\"]/div[2]/form/div[2]/div[3]")).Click(); //Year
        webDriver.FindElement(By.XPath($"//*[@id=\"Year-options-item-{yearOfBirth}\"]")).Click(); //Year

    }

    public void Subscription(ChromeDriver webDriver)
    {
        if(SubscribeLinkIsPresent(webDriver))
        {
            webDriver.FindElement(By.XPath("//*[@id=\"login-modal\"]/div[1]/div[3]/a")).Click();
        }


        webDriver.FindElement(By.XPath("//*[@id=\"loginContainer\"]/div/div/div/div[2]/div[1]/div[2]")).Click();
       
        webDriver.FindElement(By.XPath("//*[@id=\"loginContainer\"]/div/form/div[4]/a")).Click(); // Subscribe with email
        FillBirthDate(webDriver);
        webDriver
            .FindElement(By.XPath("//*[@id=\"loginContainer\"]/div[2]/form/div[5]/div/input"))
            .SendKeys(_account.Email);   
        
        webDriver
            .FindElement(By.XPath("//*[@id=\"loginContainer\"]/div[2]/form/div[6]/div/input"))
            .SendKeys(_account.Password);

        var element = webDriver.FindElement(By.XPath("//*[@id=\"login-modal\"]/div[1]"));

        webDriver.ExecuteScript("arguments[0].scrollIntoView(true);", element);
        webDriver.FindElement(By.XPath("//*[@id=\"loginContainer\"]/div[2]/form/div[7]/div/button")).Click(); //Envoyer le code

        Thread.Sleep(1500);
        
        webDriver.FindElement(By.XPath("//*[@id=\"loginContainer\"]/div[2]/form/div[7]/div/button")).Click(); //Envoyer le code

        _account.Status = Status.Captcha;
    }

    public void Start()
    {
        var _webDriver = new ChromeDriver(
            "C:\\Users\\franc\\RiderProjects\\tiktokagent\\tiktokagent\\Core"
        );
        _webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        WebDriverWait wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(30));

        _webDriver.Navigate().GoToUrl("https://www.tiktok.com");
        Thread.Sleep(2000);


        _webDriver.FindElement(By.XPath("//*[@id=\"header-login-button\"]")).Click();
        Subscription(_webDriver);

        /*wait.Until(_webDriver => _webDriver.FindElement(By.XPath("//*[@id=\"loginContainer\"]"))); // Wait for login container to appear
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
            if (CaptchaIsAsked(_webDriver)) {
                _account.Status = Status.Suspended;
            }
        }*/
    }
}
