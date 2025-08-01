using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using NUnit.Framework;

namespace LinkedinSmokeTest.Tests
{
    public class Validations
    {
        public void ValidateScreenByUrl(IWebDriver driver, string screnUrl)
        {
            string currentUrl = driver.Url;
            Assert.IsTrue(currentUrl.Equals(screnUrl));
        }

        public void ValidateElementIsPresent(IWebDriver driver, By element)
        {
            IWebElement findElement = driver.FindElement(element);
            Assert.IsTrue(findElement.Displayed);
        }

        public void ValidateTextInElement(IWebDriver driver, By element, string text)
        {
            string findElementText = driver.FindElement(element).Text;
            Assert.IsTrue(findElementText.Equals(text));
        }


    }
}
