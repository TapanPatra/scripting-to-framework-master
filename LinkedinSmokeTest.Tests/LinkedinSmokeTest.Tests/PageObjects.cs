using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace LinkedinSmokeTest.Tests
{
    public class PageObjects
    {
        #region LogInScreen
        public static By loginEmailInput = By.Id("login-email");
        public static By loginPasswordInput = By.Id("login-password");
        public static By loginButton = By.Id("login-submit");
        #endregion


        public static By fullNameElement = By.ClassName("full-name");


        public static By ProfileLinkMenu = By.XPath(".//*[@id='main-sitenav']/ul/li[2]/a");
        public static By editProfileLink = By.XPath(".//*[@id='profile-subnav']/li[1]/a");
        public static By navProfilePhoto = By.ClassName("nav-profile-photo");
        public static By logoutButton = By.XPath(".//*[@id='account-subnav']/div/div[2]/ul/li[1]/div/span/span[3]/a");
    }
}
