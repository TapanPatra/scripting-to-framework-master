# Set Up Project for Selenium Automation Framework in CSharp
This repository contains the final project framework built by the course author as part of the "Set Up Project for Selenium Automation Framework in CSharp" automation course.  

I followed along with the course to understand how to build a scalable and modular test automation framework using C#.

## Purpose
This project showcases key concepts in automation engineering including:
- Page Object Model
- Data-driven testing
- Web Element Extension Methods

## Course Followed
**Course:** Selenium C Sharp Tutorial  
**Author:** Lakshay Sharma    
**Platform:** Tools QA  

## Installation
```
  <package id="Dapper" version="1.42" targetFramework="net472" />
  <package id="NUnit" version="2.6.4" targetFramework="net472" />
  <package id="NUnitTestAdapter" version="2.0.0" targetFramework="net472" />
  <package id="Selenium.Support" version="2.47.0" targetFramework="net472" />
  <package id="Selenium.WebDriver" version="2.47.0" targetFramework="net472" />
  <package id="System.Configuration.ConfigurationManager" version="4.7.0" targetFramework="net472" />
  <package id="System.Security.AccessControl" version="4.7.0" targetFramework="net472" />
  <package id="System.Security.Permissions" version="4.7.0" targetFramework="net472" />
  <package id="System.Security.Principal.Windows" version="4.7.0" targetFramework="net472" />
```

## Page Factory in C#
```csharp
class HomePage
{
    private IWebDriver driver;

    [FindsBy(How = How.Id, Using = "account")]
    public IWebElement MyAccount { get; set; }
}
```

## PageFactory CacheLookup
```csharp
class HomePage
{
    private IWebDriver driver;

    [FindsBy(How = How.Id, Using = "account")][CacheLookup]
    public IWebElement MyAccount { get; set; }
}
```

## Optimizing Page Object Model
```csharp
public LoginPage(IWebDriver driver)
{
    this.driver = driver;
    PageFactory.InitElements(driver, this);
}
```

## Encapsulate Selenium Page Objects
```csharp
var homePage = new HomePage(driver); 
homePage.ClickOnMyAccount();
```

## Manage And Read Configurations using ConfigurationManager in C#
**App.config.**  
```csharp
using System.Configuration;
var dbString = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;

var url = ConfigurationManager.AppSettings["URL"];

var folderPath = ConfigurationManager.AppSettings["FolderPath"];
```

**How to Read Configurations from External Config file in C# using ConfigurationManager**

```
<!-- App.config File -->
<configuration>
    <connectionStrings configSource="DBConnectionStrings.config" />        
</configuration>

<!-- DBConnectionString.config File -->
<connectionStrings>
	<add name="DataBaseName" connectionString="Data Source=(LocalDB)\v11.0;Initial Catalog=WingtipToys;Integrated Security=True;Pooling=False/>
</connectionStrings>

//To read the connection string from the above config file, use the below code:
var connString= ConfigurationManager.ConnectionStrings["DataBaseName"].ConnectionString;
```

## Data Driven Testing
- Read and Write Excel Using OLEDB
- Read as Objects

```csharp
public static string TestDataFileConnection()
{
    var fileName = ConfigurationManager.AppSettings["TestDataSheetPath"];          
    var con = string.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source = {0}; Extended Properties=Excel 12.0;", fileName);
    return con;
}

public static UserData GetTestData(string keyName)
{
    using (var connection = new OleDbConnection(TestDataFileConnection()))
    {
        connection.Open();
        var query = string.Format("select * from [DataSet$] where key='{0}'", keyName);
        var value = connection.Query<UserData>(query).FirstOrDefault();
        connection.Close();
        return value;
    }
}

```
## Browser Factory or WebDriver Factory
```csharp
BrowserFactory.InitBrowser("Firefox");
BrowserFactory.LoadApplication(ConfigurationManager.AppSettings["URL"]); 
```
## Page Generator
```csharp
Page.Home.ClickOnMyAccount();
``

## WebElement Extensions Method
```csharp
public static void EnterText(IWebElement element, string text)
{
    element.Clear();
    Log.Info("Clear action is performed on an xyz element");
    element.Click();
    Log.Info("Click action is performed on an xyz element");
}
```
## Final Test
```csharp
public class LogInTest
{
    [Test]
    public void Test() {          

        BrowserFactory.InitBrowser("Firefox");
        BrowserFactory.LoadApplication(ConfigurationManager.AppSettings["URL"]);         

        Page.Home.ClickOnMyAccount();
        Page.Login.LoginToApplication("LogInTest");

        BrowserFactory.CloseAllDrivers();
    }

}
```