using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkedinSmokeTest.Tests
{
    public class BasicMethods
    {
        public void ClickElement(IWebDriver driver, By element)
        {
            driver.FindElement(element).Click();
        }

        public void SendKeys(IWebDriver driver, By element, string keys)
        {
            driver.FindElement(element).SendKeys(keys);
        }
    }
}
