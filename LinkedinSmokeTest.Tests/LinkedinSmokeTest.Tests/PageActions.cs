using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace LinkedinSmokeTest.Tests
{
    public class PageActions
    {
        BasicMethods basicMethod = new BasicMethods();
        Validations validation = new Validations();

        public void NavigateToLinkedin(IWebDriver driver, string linkedinUrl)
        {
            driver.Navigate().GoToUrl(linkedinUrl);
            validation.ValidateScreenByUrl(driver, linkedinUrl);
        }

        public void Login(IWebDriver driver, string email, string password)
        {
            basicMethod.ClickElement(driver, PageObjects.loginEmailInput);
            basicMethod.SendKeys(driver, PageObjects.loginEmailInput, email);
            basicMethod.ClickElement(driver, PageObjects.loginPasswordInput);
            basicMethod.SendKeys(driver, PageObjects.loginPasswordInput, password);
            basicMethod.ClickElement(driver, PageObjects.loginButton);


            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);


        }

        public void NavigatethroughMenu(IWebDriver driver, By menu, By submenu)
        {
            Actions builder = new Actions(driver);

            builder.MoveToElement(driver.FindElement(menu)).Perform();
            builder.MoveToElement(driver.FindElement(submenu)).Click().Perform();
        }

        public void Logout(IWebDriver driver)
        {
            Actions builder = new Actions(driver);
            builder.MoveToElement(driver.FindElement(PageObjects.navProfilePhoto)).Perform();
            builder.MoveToElement(driver.FindElement(PageObjects.logoutButton)).Click().Perform();

        }
    }
}
