# Oxygen Flow
**Functional-style C# Selenium wrapper for automated UI testing.**

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
    public void SearchTest()
    {
        var result =
             CreateContext(BrowserBrand.FireFox, new Uri("https://www.wikipedia.org/"))
             | Fill("#searchInput", "OxygenFlow")
             | Click("button[type=submit]")
             ;

        Assert.IsFalse(result.HasProblem, result.HasProblem ? result.ProblemCause.ToString() : null);
    }
}
```
Trace produced by the test above:
```
08:29:39 Element: #searchInput [0]
08:29:39 Fill 'OxygenFlow'
08:29:39 Element: button[type=submit] [0]
08:29:39 Click
```
