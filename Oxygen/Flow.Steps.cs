/*
Oxygen Flow library
*/

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Remote;

namespace Oxygen
{
    public partial class Flow
    {

        /// <summary>
        /// Provides the step result context for action.
        /// </summary>
        public static FlowStep Use(FlowStep step, Action<Context> action) => (Context context) =>
        {
            if (context.IsProblem) return context;

            if (action == null) return context.Problem("NULL argument in Use(step, action).");

            return step(context).Use(action);
        };

        public static FlowStep Use(FlowStep step, Action<RemoteWebElement> action) => (Context context) =>
        {
            if (context.IsProblem) return context;

            if (action == null) return context.Problem("NULL argument in Use(step, action).");
            try
            {
                var result = step(context);

                if (!result.IsProblem)
                {
                    action(result.Element);
                }

                return result;
            }
            catch (Exception x)
            {
                return context.Problem(x);
            }
        };

        /// <summary>
        /// Provides current context for action.
        /// </summary>
        public static FlowStep Use(Action<Context> action) => (Context context) =>
        {
            if (context.IsProblem) return context;

            if (action == null) return context.Problem("NULL argument in GetContext(action).");

            return context.Use(action);
        };

        /// <summary>
        /// Provides current context element for the action.
        /// </summary>
        public static FlowStep UseElement(Action<RemoteWebElement> action) => (Context context) =>
        {
            if (context.IsProblem) return context;

            if (action == null) return context.Problem("NULL argument in GetElement(action).");

            try
            {
                action(context.Element);
            }
            catch (Exception x)
            {
                return context.Problem(x);
            }

            return context;
        };

        static FlowStep CollectionFilter(Func<ReadOnlyCollection<IWebElement>, IWebElement> filter) => (Context context) =>
        {
            if (context.Collection == null) return context.Problem("Missing collection");
            if (context.Collection.Count == 0) return context.Problem("Empty collection");

            IWebElement child = null;

            string errorMsg = TryAndWait10Times(() =>
            {
                child = filter(context.Collection);
                return child != null;
            });

            if (child == null)
            {
                return context.Problem(LogError(errorMsg, null));
            }
            return context.FromElement(child as RemoteWebElement);
        };

        /// <summary>
        /// Returns element from the context Collection
        /// </summary>
        public static FlowStep FirstContainingText(string text) => (Context context) =>
        {
            var msg = O($"FirstContainingText: '{text}'");

            return CollectionFilter(collection =>
                    collection.Where(e => e.Text.Contains(text))
                    .FirstOrDefault())(context);
        };

        /// <summary>
        /// Returns element from context Collection
        /// </summary>
        public static FlowStep LastContainingText(string text) => (Context context) =>
        {
            var msg = O($"LastContainingText: '{text}'");

            return CollectionFilter(collection =>
                    collection.Where(e => e.Text.Contains(text))
                    .LastOrDefault())(context);
        };

        /// <summary>
        /// Mouse click on the context Element
        /// </summary>
        public static Context Click(Context context)
        {
            var msg = O("Click");

            var c = context.Element;

            if (c == null)
            {
                return context.Problem(LogError("Missing context Element!", null));
            }

            try
            {
                new Actions(context.Driver).MoveToElement(c).Perform();
            }
            catch (Exception x)
            {
                O("MoveToElement failed: " + x.Message);
            }

            Exception clickError = null;
            var result = TryAndWait10Times(() =>
            {
                try
                {
                    if (c.TagName == HtmlTag.li.ToString())
                    {
                        new Actions(context.Driver).Click(c).Perform();
                    }
                    else
                    {
                        c.Click();
                    }

                    return true;
                }
                catch (Exception x)
                {
                    clickError = x;
                    return false;
                }
            });

            if (string.IsNullOrEmpty(result))
            {
                return context;
            }
            else
            {
                return context.Problem(LogError(msg, clickError));
            }
        }

        /// <summary>
        /// Mouse click on page element retuned by given flow step
        /// </summary>

        public static FlowStep Click(FlowStep step) => (Context context) => step(context) | Click;

        /// <summary>
        /// Mouse click on page element with given id
        /// </summary>
        public static FlowStep Click(string elementId) => (Context context) => Element(elementId)(context) | Click;

        public static Context DblClick(Context context)
        {
            var msg = O("DblClick");

            Click(context);

            try
            {
                new Actions(context.Driver).DoubleClick(context.Element).Perform();

                Thread.Sleep(10);

                return context;
            }
            catch (Exception x)
            {
                return context.Problem(LogError(msg, x));
            }
        }


        /// <summary>
        /// Sets text box, text area and combo text on page
        /// </summary>
        public static FlowStep SetText(string cssSelector, string text, int index = 0) => (Context context) =>
            Element(cssSelector)(context) | SetText(text);

        /// <summary>
        /// Sets current context element text if text box, text area or dropdown list.
        /// </summary>
        public static FlowStep SetText(string value) => (Context context) =>
          {
              var msg = O($"SetText: '{value}'");

              if (context.Element == null)
              {
                  return context.Problem("Missing context Element");
              }

              var element = context.Element;

              TryAndWait10Times(() => element.Displayed);

              try
              {
                  if (element.TagName == HtmlTag.input.ToString())
                  {
                      if (!string.IsNullOrEmpty(element.GetAttribute("value")))
                      {
                          element.SendKeys(" ");
                          element.Clear();
                      }
                      element.SendKeys(value);
                  }
                  else if (element.TagName == HtmlTag.textarea.ToString())
                  {
                      element.Clear();
                      element.SendKeys(value);
                  }
                  else if (element.TagName == HtmlTag.select.ToString())
                  {
                      SetComboText(context.FromElement(element), value);
                  }
                  else
                  {
                      return context.Problem($"Unexpected tag <{element.TagName}> in SetText");
                  }
              }
              catch (Exception x)
              {
                  return context.Problem(LogError(msg, x));
              }

              return context;
          };



        public static Context PressEnter(Context context)
        {
            var c = context.Element;

            if (c == null)
            {
                return context.Problem("PressEnter: Missing context element");
            }

            var msg = O($"PressEnter: <{c.TagName}>{c.Text}</{c.TagName}>");

            try
            {
                new Actions(context.Driver).SendKeys(c, Keys.Enter).Perform();
            }
            catch (Exception x)
            {
                return context.Problem(LogError(msg, x));
            }

            return context;
        }

        /// <summary>
        /// Select display text in combobox
        /// </summary>
        public static Context SetComboText(Context context, string value)
        {
            var c = context.Element;

            if (c == null)
            {
                return context.Problem("SetComboText: Missing context element");
            }

            var msg = O($"SetComboText: {value}");

            try
            {
                if (c.Text != value)
                {
                    var item = c.FindElementsByTagName(HtmlTag.option.ToString()).Where(opt => opt.Text == value).FirstOrDefault();

                    if (item == null)
                    {
                        return context.Problem($"Can't find combo text '{value}'");
                    }

                    item.Click();
                }

                return context.FromElement(c);
            }

            catch (Exception x)
            {
                return context.Problem(LogError(msg, x));
            }

        }

        /// <summary>
        /// Clicks teh hyperlink and checks target window title.
        /// </summary>
        /// <param name="linkID"></param>
        /// <param name="targetTitle"></param>
        /// <returns></returns>
        public static FlowStep FollowLink(string linkID, string targetTitle) => (Context context) =>
          {
              TryAndWait10Times(() =>
              {
                  Element(linkID)(context).Bind(Click);
                  return context.Title == targetTitle;
              });

              if (context.Title != targetTitle)
              {
                  return context.Problem($"Expected title '{targetTitle}', actual '{context.Title}'");
              }

              return context;
          };

        public static FlowStep AssertAttributeValue(string attributeName, string expected) => (Flow.Context context) =>
        {
            if (!context.HasElement)
            {
                return context.Problem($"AssertAttributeValue {attributeName}='{expected}': Missing context element.");
            }

            var actual = context.Element.GetAttribute(attributeName);

            return actual == expected ?
                context : context.Problem($"Expected {attributeName}='{expected}', actual {attributeName}='{actual}'");
        };

        public static FlowStep Assertion(Predicate<Context> predicate, string errorMessage) => (Flow.Context context) =>
            predicate(context) ? context : context.Problem(errorMessage);



    }
}
