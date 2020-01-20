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
  [TestMethod]
  public void GoogleSearchTest()
  {
      Flow.Context.Create(Browser.EDGE, "https://www.google.com/")
      // find and fill search box
      | AssureWindow("Google")
      | AllByTag(HtmlTags.input) | FirstByAttribute("title", "Search")
      | SetText("oxygen-flow")
      // find and press search button
      | AllByTag(HtmlTags.input) | FirstByAttribute("value", "Google Search")
      | Click
      // assertions ...
      | AssureWindow("oxygen-flow - Google Search")
      | ById("resultStats") | AssertTextContains("About ")
      // etc...
```

Composed flow steps example:
```csharp
using Oxygen;
using FlowStep Func<Oxygen.Flow.Context, Oxygen.Flow.Context>;

[TestClass]
public class SampleTest : Oxygen.Flow
{
  static FlowStep FillSearchBox(string searchTerm) => (Flow.Context context) =>
    AllByTag(HtmlTags.input)(context) | FirstByAttribute("title", "Search") | SetText(searchTerm);
    
  static Flow.Context ClickSearchButton(Flow.Context context) =>
    AllByTag(HtmlTags.input)(context) | FirstByAttribute("value", "Google Search") | Click;
  
  [TestMethod]
  public void GoogleSearchTest()
  {
      Flow.Context.Create(Browser.EDGE, "https://www.google.com/")
      | AssureWindow("Google")
      | FillSearchBox("oxygen-flow")
      | ClickSearchButton
      // assertions ...
      // etc...
```
