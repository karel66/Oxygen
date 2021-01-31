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
18:26:57.089 AcceptGoogleTerms 
18:26:57.090 	IfExists "iframe", step=><>c.AcceptGoogleTerms(UnitTests.Demo+<>c, <>9__2_0=><>c.AcceptGoogleTerms)
18:26:57.107 		AcceptGoogleTerms UnitTests.Demo+<>c, <>9__2_0=><>c.AcceptGoogleTerms
18:26:57.107 			SwitchToFrame "iframe"
18:26:57.108 				Find "iframe", 0
18:26:57.121 				SwitchToFrame 
18:26:57.145 			Click "div#introAgreeButton"
18:26:57.145 				Find "div#introAgreeButton", 0
18:26:57.159 				Click 
18:26:57.841 			SwitchToDefault 
18:26:57.843 Find "input[name=q]", 0
18:26:57.862 SetText "OxygenFlow"
18:26:57.990 Click "input[type=submit]"
18:26:57.990 	Find "input[type=submit]", 0
18:26:58.002 	Click 
18:26:58.024 *** ERROR *** javascript error: Failed to execute 'elementsFromPoint' on 'Document': The provided double value is non-finite.
  (Session info: chrome=88.0.4324.104)
```

More in the Wiki https://github.com/karel66/OxygenFlow/wiki
