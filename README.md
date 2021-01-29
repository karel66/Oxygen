# Oxygen Flow
**Functional style C# Selenium wrapper for automated UI testing.**

- Write web UI test as sequence of piped steps - a flow. 
- Full functional composability - repeating sequence of steps can be combined to a new step, thus creating components of arbitrary complexity while preserving basic flow step interface. For example: in a flow like F => a|b|c|a|b|c|a|b|c the repating sequence a|b|c can be abstracted literally to D => a|b|c and rewritten as F=>D|D|D
- Full trace logging for troubleshooting, no digging in deep call stacks.

Actual heavy lifting is done by CSS selectors, so one should master the language to take full advantage of Oxygen Flow.

Basic flow steps example:
```csharp
using Oxygen;

[TestClass]
public class Demo : Oxygen.Flow
{
    /// <summary>
    /// Run Google search. Chrome browser must be present and matching webdriver in C:\Selenium folder.
    /// </summary>
    [TestMethod]
    public void SearchTestChrome()
    {
        const string googleUrl = "https://www.google.com/";
        const string googleSearchBox = "input[name=q]";
        const string googleSearchButton = "input[type=submit]";

        var result =
            CreateContext(BrowserBrand.Chrome, new Uri(googleUrl), 30, true, @"C:\Selenium")
            | AcceptGoogleTerms
            | Find(googleSearchBox)
            | SetText("OxygenFlow")
            | Click(googleSearchButton);

        Assert.IsFalse(result.HasProblem, result);
    }

    /// <summary>
    /// Agree to Google terms if presented in iframe
    /// </summary>
    static Context AcceptGoogleTerms(Context context) =>
        IfExists("iframe", _ => _ 
            | SwitchToFrame("iframe") 
            | Click("div#introAgreeButton") 
            | SwitchToDefault)            
        (context);
}
```
Trace produced by the test above:
```
21:59:54 AcceptGoogleTerms 
21:59:54 SwitchToFrame "iframe"
21:59:54 Find "iframe", 0
21:59:54 SwitchToFrame 
21:59:54 Click "div#introAgreeButton"
21:59:54 Find "div#introAgreeButton", 0
21:59:54 Click 
21:59:54 SwitchToDefault 
21:59:54 Find "input[name=q]", 0
21:59:54 SetText "OxygenFlow"
21:59:55 Click "input[type=submit]"
21:59:55 Find "input[type=submit]", 0
21:59:55 Click 
21:59:55 *************************************************************************
21:59:55 FAILED javascript error: Failed to execute 'elementsFromPoint' on 'Document': The provided double value is non-finite.
  (Session info: chrome=88.0.4324.104)
21:59:55 *************************************************************************```

More in the Wiki https://github.com/karel66/OxygenFlow/wiki
