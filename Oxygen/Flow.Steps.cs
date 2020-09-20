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
            if (context.HasProblem) return context;

            if (action == null) return context.NewProblem($"{nameof(Use)}: NULL action argument.");

            return step(context).Use(action);
        };

        /// <summary>
        /// Provides the step result element for action.
        /// </summary>
        public static FlowStep Use(FlowStep step, Action<RemoteWebElement> action) => (Context context) =>
        {
            if (context.HasProblem) return context;

            if (action == null) return context.NewProblem($"{nameof(Use)}: NULL action argument.");

            try
            {
                var result = step(context);

                if (!result.HasProblem)
                {
                    action(result.Element);
                }

                return result;
            }
            catch (Exception x)
            {
                return context.NewProblem(x);
            }
        };

        /// <summary>
        /// Provides current context for action.
        /// </summary>
        public static FlowStep Use(Action<Context> action) => (Context context) =>
        {
            if (context.HasProblem) return context;

            if (action == null) return context.NewProblem($"{nameof(Use)}: NULL action argument.");

            return context.Use(action);
        };

        /// <summary>
        /// Provides current context element for the action.
        /// </summary>
        public static FlowStep UseElement(Action<RemoteWebElement> action) => (Context context) =>
        {
            if (context.HasProblem) return context;

            if (action == null) return context.NewProblem($"{nameof(UseElement)}: NULL action argument.");

            try
            {
                action(context.Element);
            }
            catch (Exception x)
            {
                return context.NewProblem(x);
            }

            return context;
        };

        static FlowStep CollectionFilter(Func<ReadOnlyCollection<IWebElement>, IWebElement> filter) => (Context context) =>
        {
            if (filter == null) return context.NewProblem($"{nameof(CollectionFilter)}: NULL filter");
            if (context.Collection == null) return context.NewProblem($"{nameof(CollectionFilter)}: missing collection");
            if (context.Collection.Count == 0) return context.NewProblem($"{nameof(CollectionFilter)}: empty collection");

            IWebElement child = null;

            if (!TryUntilSuccess(() =>
            {
                child = filter(context.Collection);
                return child != null;
            }))
            {
                return context.NewProblem($"{nameof(CollectionFilter): failed}");
            }

            return context.NewContext(child as RemoteWebElement);
        };

        /// <summary>
        /// Returns element from the context Collection
        /// </summary>
        public static FlowStep FirstContainingText(string text) => (Context context) =>
        {
            var msg = O($"{nameof(FirstContainingText)}: '{text}'");

            return
                CollectionFilter(collection => collection.Where(e => e.Text.Contains(text))
                    .FirstOrDefault())(context);
        };

        /// <summary>
        /// Returns element from context Collection
        /// </summary>
        public static FlowStep LastContainingText(string text) => (Context context) =>
        {
            var msg = O($"{nameof(LastContainingText)}: '{text}'");

            return
                CollectionFilter(collection => collection.Where(e => e.Text.Contains(text))
                    .LastOrDefault())(context);
        };

        /// <summary>
        /// Mouse click on the context Element
        /// </summary>
        public static Context Click(Context context)
        {
            var msg = O($"{nameof(Click)}");

            var c = context.Element;

            if (c == null)
            {
                return context.NewProblem(LogError($"{nameof(Click)}: missing context Element!", null));
            }

            try
            {
                new Actions(context.Driver).MoveToElement(c).Perform();
            }
            catch (Exception x)
            {
                O($"{nameof(Click)}: moveToElement failed: " + x.Message);
            }

            if (!TryUntilSuccess(() =>
            {
                if (c.TagName == "li")
                {
                    new Actions(context.Driver).Click(c).Perform();
                }
                else
                {
                    c.Click();
                }

                return true;
            }))
            {
                return context.NewProblem($"{nameof(Click)}: failed");
            }

            return context;
        }

        /// <summary>
        /// Mouse click on page element retuned by given flow step
        /// </summary>

        public static FlowStep Click(FlowStep step) => (Context context) => step(context) | Click;

        /// <summary>
        /// Mouse click on page element returned by CSS selector
        /// </summary>
        public static FlowStep Click(string selector) => (Context context) => context | Find(selector) | Click;

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
                return context.NewProblem(LogError(msg, x));
            }
        }

        public static FlowStep DblClick(string selector) => (Context context) => context | Find(selector) | DblClick;

        /// <summary>
        /// Sets text box, text area and combo text on page
        /// </summary>
        public static FlowStep Fill(string cssSelector, string text) => (Context context) => context | Find(cssSelector) | Fill(text);

        /// <summary>
        /// Sets current context element text if text box, text area or dropdown list.
        /// </summary>
        public static FlowStep Fill(string text) => (Context context) =>
          {
              var msg = O($"{nameof(Fill)} '{text}'");

              if (context.Element == null)
              {
                  return context.NewProblem($"{nameof(Fill)}: missing context Element");
              }

              var element = context.Element;

              TryUntilSuccess(() => element.Displayed);

              try
              {
                  if (element.TagName == "input")
                  {
                      if (!string.IsNullOrEmpty(element.GetAttribute("value")))
                      {
                          element.SendKeys(" ");
                          element.Clear();
                      }
                      element.SendKeys(text);
                  }
                  else if (element.TagName == "textarea")
                  {
                      element.Clear();
                      element.SendKeys(text);
                  }
                  else if (element.TagName == "select")
                  {
                      return Select(context.NewContext(element), text);
                  }
                  else
                  {
                      return context.NewProblem($"{nameof(Fill)}: unexpected tag <{element.TagName}>");
                  }
              }
              catch (Exception x)
              {
                  return context.NewProblem(LogError(msg, x));
              }

              return context;
          };

        public static Context PressEnter(Context context)
        {
            var c = context.Element;

            if (c == null)
            {
                return context.NewProblem("PressEnter: Missing context element");
            }

            var msg = O($"PressEnter: <{c.TagName}>{c.Text}</{c.TagName}>");

            try
            {
                new Actions(context.Driver).SendKeys(c, Keys.Enter).Perform();
            }
            catch (Exception x)
            {
                return context.NewProblem(LogError(msg, x));
            }

            return context;
        }

        /// <summary>
        /// Select display text in combobox
        /// </summary>
        public static Context Select(Context context, string value)
        {
            var c = context.Element;

            if (c == null)
            {
                return context.NewProblem($"{nameof(Select)}: Missing context element");
            }

            var msg = O($"{nameof(Select)}: {value}");

            try
            {
                if (c.Text != value)
                {
                    var item = c.FindElementsByTagName("option").Where(opt => opt.Text == value).FirstOrDefault();

                    if (item == null)
                    {
                        return context.NewProblem($"Can't find combo text '{value}'");
                    }

                    item.Click();
                }

                return context.NewContext(c);
            }

            catch (Exception x)
            {
                return context.NewProblem(LogError(msg, x));
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
              TryUntilSuccess(() =>
              {
                  Click(linkID)(context);
                  return context.Title == targetTitle;
              });

              if (context.Title != targetTitle)
              {
                  return context.NewProblem($"Expected title '{targetTitle}', actual '{context.Title}'");
              }

              return context;
          };

        public static FlowStep AssertAttributeValue(string attributeName, string expected) => (Flow.Context context) =>
        {
            if (!context.HasElement)
            {
                return context.NewProblem($"AssertAttributeValue {attributeName}='{expected}': Missing context element.");
            }

            var actual = context.Element.GetAttribute(attributeName);

            return actual == expected ?
                context : context.NewProblem($"Expected {attributeName}='{expected}', actual {attributeName}='{actual}'");
        };

        public static FlowStep Assertion(Predicate<Context> predicate, string errorMessage) => (Flow.Context context) =>
            predicate(context) ? context : context.NewProblem(errorMessage);

		public static FlowStep Assertion(Predicate<Context> predicate, Func<Context, string> errorMessage) => (Flow.Context context) =>
		   predicate(context) ? context : context.NewProblem(errorMessage(context));



	}
}
