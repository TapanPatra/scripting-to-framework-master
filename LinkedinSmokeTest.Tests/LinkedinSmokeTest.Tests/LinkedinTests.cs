using LinkedinSmokeTest.Tests;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkedinSmokeTest.Test
{
    [TestFixture]
    public class LinkedinTests
    {
        static IWebDriver driver;
        PageActions pageActions = new PageActions();
        Validations validations = new Validations();

        public string email = "firstname.lastname@example.com";
        public string password = "password";
        public string fullname = "Firstname Middlename Lastname";
        public string linkedinUrl = "https://www.linkedin.com/";

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);

        }

        [Test]
        public void SmokeTest()
        {
            pageActions.NavigateToLinkedin(driver, linkedinUrl);
            pageActions.Login(driver, email, password);
            pageActions.NavigatethroughMenu(driver, PageObjects.ProfileLinkMenu, PageObjects.editProfileLink);
            validations.ValidateTextInElement(driver, PageObjects.fullNameElement, fullname);


            pageActions.Logout(driver);

        }

        [Test]
        [Ignore("Ignore a Test")]
        public void RegressionTest()
        {

        }

        [TearDown]
        public void Teardown()
        {
            driver.Quit();
        }
    }
}
