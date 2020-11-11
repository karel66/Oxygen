# Oxygen Flow
**Functional style C# Selenium wrapper for automated UI testing.**

- Write web UI test as sequence of piped steps - a flow. 
- Full functional composability - repeating sequence of steps can be combined to a new flow step, thus creating components of arbitrary complexity while preserving basic flow step interface.
- Full trace - failing step and context in plain sight, no digging in deep call stacks.

Actual heavy lifting is done by CSS selectors, so one should master the language to take full advantage of Oxygen Flow.

Basic flow steps example:
```csharp
using Oxygen;

[TestClass]
public class Demo : Oxygen.Flow
{
    [TestMethod]
    public void SearchTestChrome()
    {
        var result =
            CreateContext(BrowserBrand.Chrome, new Uri("https://www.google.com/"), 30, true, @"C:\Selenium")
            // Agree to Google terms if presented
            | IfExists("iframe",
                (Context context) => context | SwitchToFrame("iframe") | Find("div#introAgreeButton") | Click | SwitchToDefault)
            
            | SetText("input[name=q]", "OxygenFlow")
            | Click("input[type=submit]");
            
        Assert.IsFalse(result.HasProblem, result);
    }
}
```
Trace produced by the test above:
```
22:19:10 Exists: iframe : True
22:19:10 Element: iframe [0]
22:19:10 SwitchToFrame
22:19:11 Element: div#introAgreeButton [0]
22:19:11 Click
22:19:11 SwitchToDefault
22:19:11 Element: input[name=q] [0]
22:19:11 SetText 'OxygenFlow'
22:19:11 Element: input[type=submit] [0]
22:19:11 Click
22:19:11 Click: moveToElement failed: javascript error: Failed to execute 'elementsFromPoint' on 'Document': The provided double value is non-finite.
  (Session info: chrome=86.0.4240.193)
```
