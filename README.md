# Oxygen Flow
**Functional-style C# Selenium wrapper for automated UI testing.**

- Write web UI test as sequence of piped steps - a flow. 
- Full functional composability - group repeating sequences to new flow steps and use these like LEGO blocks.
- Full flow trace - failing step and context in plain sight, no digging in obscure VM call stacks.

Basic flow steps example:
```csharp
using Oxygen;

[TestClass]
public class SampleTest : Oxygen.Flow
{
  [TestClass]
    public class Demo : Oxygen.Flow
    {
        [TestMethod]
        public void GoogleSearchTest()
        {
           var result = 
                CreateContext(BrowserBrand.FireFox, new Uri("https://www.google.com/"))
                | Fill("input[name=q]","OxygenFlow")
                | Click("input[type=submit]")
                ;

            Assert.IsFalse(result.HasProblem, result.ProblemCause.ToString());
        }
    }
```

