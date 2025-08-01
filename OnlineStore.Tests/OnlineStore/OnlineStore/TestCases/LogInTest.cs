using NUnit.Framework;
using OnlineStore.PageObjects;
using OnlineStore.WrapperFactory;
using OpenQA.Selenium;
using System.Configuration;


namespace OnlineStore.TestCases
{
    public class LogInTest
    {
        [Test]
        public void Test()
        {

            BrowserFactory.InitBrowser("Firefox");
            BrowserFactory.LoadApplication(ConfigurationManager.AppSettings["URL"]);

            Page.Home.ClickOnMyAccount();
            Page.Login.LoginToApplication("LogInTest");

            BrowserFactory.CloseAllDrivers();
        }

    }
}
