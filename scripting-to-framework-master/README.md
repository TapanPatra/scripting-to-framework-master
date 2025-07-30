# scripting-to-framework
This repository contains the final project framework built by the course author as part of the "Scripting to Framework with C#" automation course.
I followed along with the course to understand how to build a scalable and modular test automation framework using C# and .NET Core with industry-standard tools and practices.

## Purpose
- This project showcases key concepts in automation engineering including:
- Scripting to framework evolution
- Design patterns (Page Object Model)
- Data-driven testing
- Logging, reporting, and utility handling

## Course Followed
**Course:** Scripting to Framework with C#  
**Author:** Carlos Kidman  
**Platform:** Test Automation University  
## Machine Setup
```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>netstandard2.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="DotNetSeleniumExtras.WaitHelpers" Version="3.11.0" />
    <PackageReference Include="Newtonsoft.Json" Version="12.0.2" />
    <PackageReference Include="NUnit" Version="3.12.0" />
    <PackageReference Include="RestSharp" Version="106.6.9" />
    <PackageReference Include="Selenium.Support" Version="3.141.0" />
    <PackageReference Include="Selenium.WebDriver" Version="3.141.0" />
    <PackageReference Include="Selenium.WebDriver.ChromeDriver" Version="125.0.6422.6000" />
  </ItemGroup>

</Project>
```
## Page Object Model
```csharp
public abstract class PageBase
{
    public readonly HeaderNav HeaderNav;

    public PageBase()
    {
        HeaderNav = new HeaderNav();
    }
}

public class CardsPage : PageBase
{
    public readonly CardsPageMap Map;

    public CardsPage()
    {
        Map = new CardsPageMap();
    }

    public CardsPage Goto()
    {
        HeaderNav.GotoCards();
        return this;
    }

    public IWebElement GetCardByName(string cardName)
    {
        var formattedName = cardName;

        if (cardName.Contains(" "))
        {
            formattedName = cardName.Replace(" ", "+");
        }

        return Map.Card(formattedName);
    }
}

public class CardsPageMap
{
    public Element Card(string name) => Driver.FindElement(By.CssSelector($"a[href*='{name}']"), $"Card: {name}");
}

```
### Page Factory
```csharp
public class Pages
{
    [ThreadStatic]
    public static CardsPage Cards;

    [ThreadStatic]
    public static CardDetailsPage CardDetails;

    [ThreadStatic]
    public static DeckBuilderPage DeckBuilder;

    [ThreadStatic]
    public static CopyDeckPage CopyDeck;

    public static void Init()
    {
        Cards = new CardsPage();
        CardDetails = new CardDetailsPage();
        DeckBuilder = new DeckBuilderPage();
        CopyDeck = new CopyDeckPage();
    }
}
```

## Models and Services
### Models 
Model can then be serialized to JSON.
```csharp
public class Card
{
    public virtual string Id { get; set; }

    public virtual string Name { get; set; }

    public virtual string Icon { get; set; }

    public virtual int Cost { get; set; }

    public virtual string Rarity { get; set; }

    public virtual string Type { get; set; }

    public virtual string Arena { get; set; }
}
public class IceSpritCard : Card
{
    public override string Name { get; set; } = "Ice Spirit";

    public override int Cost { get; set; } = 1;

    public override string Rarity { get; set; } = "Common";

    public override string Type { get; set; } = "Troop";

    public override string Arena { get; set; } = "Arena 8";
}
public class MirrorCard : Card
{
    public override string Name { get; set; } = "Mirror";

    public override int Cost { get; set; } = 1;

    public override string Type { get; set; } = "Spell";

    public override string Arena { get; set; } = "Arena 12";

    public override string Rarity { get; set; } = "Epic";
}

```
### Services
```csharp
public interface ICardService
{
    Card GetCardByName(string name);

    IList<Card> GetAllCards();
}

public class ApiCardService : ICardService
{
    public const string CARD_API = "https://statsroyale.com/api/cards";

    public IList<Card> GetAllCards()
    {
        var client = new RestClient(CARD_API);
        var request = new RestRequest
        {
            Method = Method.GET,
            RequestFormat = DataFormat.Json
        };

        var response = client.Execute(request);

        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            throw new Exception(
                $"/cards endpoint failed with {response.StatusCode.ToString()} -> {response.Content}");
        }

        return JsonConvert.DeserializeObject<IList<Card>>(response.Content);
    }

    public Card GetCardByName(string name)
    {
        var cards = GetAllCards();
        return cards.FirstOrDefault(card => card.Name == name);
    }
}
```

## Customizing Web driver
```csharp
public class Driver
{
    [ThreadStatic]
    static IWebDriver _driver;

    [ThreadStatic]
    public static Wait Wait;

    [ThreadStatic]
    public static WindowManager Window;

    public static void Init(int waitSeconds = 10)
    {
        _driver = DriverFactory.Build(FW.Config.Driver.Browser);
        Wait = new Wait(waitSeconds);
        Window = new WindowManager();
        Window.Maximize();
    }

    public static IWebDriver Current => _driver ?? throw new NullReferenceException("_driver is null. Call Driver.Init() first.");

    public static string Title => Current.Title;

    public static void Goto(string url)
    {
        if (!url.StartsWith("http"))
        {
            url = $"http://{url}";
        }

        FW.Log.Step($"Navigate to {url}");
        Current.Navigate().GoToUrl(url);
    }

    public static Element FindElement(By by, string elementName)
    {
        return new Element(Current.FindElement(by), elementName)
        {
            FoundBy = by
        };
    }

    public static Elements FindElements(By by)
    {
        return new Elements(Current.FindElements(by))
        {
            FoundBy = by
        };
    }

    public static void TakeScreenshot(string imageName)
    {
        var ss = ((ITakesScreenshot)Current).GetScreenshot();
        var ssFileName = Path.Combine(FW.CurrentTestDirectory.FullName, imageName);
        ss.SaveAsFile($"{ssFileName}.png", ScreenshotImageFormat.Png);
    }

    public static void Quit()
    {
        FW.Log.Info("Close browser");
        Current.Quit();
        Current.Dispose();
    }

}
```
### Driver Factory
```csharp
public static class DriverFactory
{
    public static IWebDriver Build(string browser)
    {
        FW.Log.Info("Open browser: " + browser);

        switch (browser)
        {
            case "chrome":
                return BuildChrome();

            case "firefox":
                return new FirefoxDriver();

            default:
                throw new System.ArgumentException("Cannot build unsupported browser: " + browser);
        }
    }

    private static ChromeDriver BuildChrome()
    {
        var options = new ChromeOptions();
        options.AddArgument("--start-maximized");
        return new ChromeDriver(options);
    }
}
```

## Test Data from API 
```csharp
static IList<Card> apiCards = new ApiCardService().GetAllCards();

[Test, Parallelizable(ParallelScope.Children)]
[TestCaseSource("apiCards")]
[Category("cards")]
public void Card_is_on_cards_page(Card apiCard)
{
    var card = Pages.Cards.Goto().GetCardByName(apiCard.Name);
    Assert.That(card.Displayed);
}
```
## Logging
```csharp
public class Logger
{
    private readonly string _filepath;

    public Logger(string testName, string filepath)
    {
        _filepath = filepath;

        using (var log = File.CreateText(_filepath))
        {
            log.WriteLine($"Starting timestamp: {DateTime.Now.ToLocalTime()}");
            log.WriteLine($"Test: {testName}");
        }
    }

    public void Info(string message)
    {
        WriteLine($"[INFO]: {message}");
    }

    public void Step(string message)
    {
        WriteLine($"    [STEP]: {message}");
    }

    public void Warning(string message)
    {
        WriteLine($"[WARNING]: {message}");
    }

    public void Error(string message)
    {
        WriteLine($"[ERROR]: {message}");
    }

    public void Fatal(string message)
    {
        WriteLine($"[FATAL]: {message}");
    }

    private void WriteLine(string text)
    {
        using (var log = File.AppendText(_filepath))
        {
            log.WriteLine(text);
        }
    }

    private void Write(string text)
    {
        using (var log = File.AppendText(_filepath))
        {
            log.Write(text);
        }
    }
}
```

## Elements
```csharp
public class Element : IWebElement
{
    private readonly IWebElement _element;

    public readonly string Name;

    public By FoundBy { get; set; }

    public Element(IWebElement element, string name)
    {
        _element = element;
        Name = name;
    }

    public IWebElement Current => _element ?? throw new NullReferenceException("Current IWebElement _element is null.");

    public string TagName => Current.TagName;

    public string Text => Current.Text;

    public bool Enabled => Current.Enabled;

    public bool Selected => Current.Selected;

    public Point Location => Current.Location;

    public Size Size => Current.Size;

    public bool Displayed => Current.Displayed;

    public void Clear()
    {
        Current.Clear();
    }

    public void Click()
    {
        FW.Log.Step($"Click {Name}");
        Current.Click();
    }

    public void Hover()
    {
        var actions = new Actions(Driver.Current);
        actions.MoveToElement(Current).Perform();
    }

    public IWebElement FindElement(By by)
    {
        return Current.FindElement(by);
    }

    public ReadOnlyCollection<IWebElement> FindElements(By by)
    {
        return Current.FindElements(by);
    }

    public string GetAttribute(string attributeName)
    {
        return Current.GetAttribute(attributeName);
    }

    public string GetCssValue(string propertyName)
    {
        return Current.GetCssValue(propertyName);
    }

    public string GetProperty(string propertyName)
    {
        return Current.GetProperty(propertyName);
    }

    public void SendKeys(string text)
    {
        Current.SendKeys(text);
    }

    public void Submit()
    {
        Current.Submit();
    }
}

public class Elements : ReadOnlyCollection<IWebElement>
{
    private readonly IList<IWebElement> _elements;

    public Elements(IList<IWebElement> list) : base(list)
    {
        _elements = list;
    }

    public By FoundBy { get; set; }

    public bool IsEmpty => Count == 0;
}
```
## Configuration
```csharp
public class FwConfig
{
    public DriverSettings Driver { get; set; }

    public TestSettings Test { get; set; }
}

public class DriverSettings
{
    public string Browser { get; set; }
}

public class TestSettings
{
    public string Url { get; set; }
}

public class FW
{
    public static string WORKSPACE_DIRECTORY = Path.GetFullPath(@"../../../../");

    public static FwConfig Config => _configuration ?? throw new NullReferenceException("Config is null. Call FW.SetConfig() first.");

    public static Logger Log => _logger ?? throw new NullReferenceException("Logger is null. Call FW.SetLogger() first.");

    [ThreadStatic]
    public static DirectoryInfo CurrentTestDirectory;

    [ThreadStatic]
    private static Logger _logger;

    private static FwConfig _configuration;

    public static DirectoryInfo CreateTestResultsDirectory()
    {
        var testResultsDirectory = WORKSPACE_DIRECTORY + "TestResults";

        if (Directory.Exists(testResultsDirectory))
        {
            Directory.Delete(testResultsDirectory, recursive: true);
        }

        return Directory.CreateDirectory(testResultsDirectory);
    }

    public static void SetConfig()
    {
        if (_configuration == null)
        {
            var jsonStr = File.ReadAllText(WORKSPACE_DIRECTORY + "/framework-config.json");
            _configuration = JsonConvert.DeserializeObject<FwConfig>(jsonStr);
        }
    }

    public static void SetLogger()
    {
        lock(_setLoggerLock)
        {
            var testResultsDirectory = WORKSPACE_DIRECTORY + "TestResults";
            var testName = TestContext.CurrentContext.Test.Name;
            var testPath = $"{testResultsDirectory}/{testName}";

            if (Directory.Exists(testPath))
            {
                CurrentTestDirectory = Directory.CreateDirectory(testPath + TestContext.CurrentContext.Test.ID);
            }
            else
            {
                CurrentTestDirectory = Directory.CreateDirectory(testPath);
            }

            _logger = new Logger(testName, CurrentTestDirectory.FullName + "/log.txt");
        }
    }

    private static object _setLoggerLock = new object();
}
```

## Test base and Outcomes
```csharp
public abstract class TestBase
{
    [OneTimeSetUp]
    public virtual void BeforeAll()
    {
        FW.SetConfig();
        FW.CreateTestResultsDirectory();
    }

    [SetUp]
    public virtual void BeforeEach()
    {
        FW.SetLogger();
        Driver.Init();
        Pages.Init();
        Driver.Goto(FW.Config.Test.Url);
    }

    [TearDown]
    public virtual void AfterEach()
    {
        var outcome = TestContext.CurrentContext.Result.Outcome.Status;

        if (outcome == TestStatus.Passed)
        {

        }
        else if (outcome == TestStatus.Failed)
        {
            Driver.TakeScreenshot("test_failed");
        }
        else
        {

        }

        FW.Log.Info("Outcome: " + outcome);

        Driver.Quit();
    }
}

[Parallelizable]
public class CardTests : TestBase
{
    static IList<Card> apiCards = new ApiCardService().GetAllCards();

    [Test, Parallelizable(ParallelScope.Children)]
    [TestCaseSource("apiCards")]
    [Category("cards")]
    public void Card_is_on_cards_page(Card apiCard)
    {
        var card = Pages.Cards.Goto().GetCardByName(apiCard.Name);
        Assert.That(card.Displayed);
    }
}
```
## Wait Conditions
```csharp
public class Wait
{
    private readonly WebDriverWait _wait;

    public Wait(int waitSeconds)
    {
        _wait = new WebDriverWait(Driver.Current, TimeSpan.FromSeconds(waitSeconds))
        {
            PollingInterval = TimeSpan.FromMilliseconds(500)
        };

        _wait.IgnoreExceptionTypes(
            typeof(NoSuchElementException),
            typeof(ElementNotVisibleException),
            typeof(StaleElementReferenceException)
        );
    }

    public bool Until(Func<IWebDriver, bool> condition)
    {
        return _wait.Until(condition);
    }

    public IWebElement Until(Func<IWebDriver, IWebElement> condition)
    {
        return _wait.Until(condition);
    }
}

public sealed class WaitConditions
{
    public static Func<IWebDriver, bool> ElementDisplayed(IWebElement element)
    {
        bool condition(IWebDriver driver)
        {
            return element.Displayed;
        }

        return condition;
    }

    /// <summary>
    /// Example of more complex condition that returns the element.
    /// </summary>
    public static Func<IWebDriver, IWebElement> ElementIsDisplayed(IWebElement element)
    {
        IWebElement condition(IWebDriver driver)
        {
            try
            {
                return element.Displayed ? element : null;
            }
            catch (NoSuchElementException)
            {
                return null;
            }
            catch (ElementNotVisibleException)
            {
                return null;
            }
        }

        return condition;
    }

    public static Func<IWebDriver, bool> ElementNotDisplayed(IWebElement element)
    {
        bool condition(IWebDriver driver)
        {
            try
            {
                return !element.Displayed;
            }
            catch (StaleElementReferenceException)
            {
                return true;
            }
        }

        return condition;
    }

    public static Func<IWebDriver, Elements> ElementsNotEmpty(Elements elements)
    {
        Elements condition(IWebDriver driver)
        {
            Elements _elements = Driver.FindElements(elements.FoundBy);
            return _elements.IsEmpty ? null : _elements;
        }

        return condition;
    }
}
```

