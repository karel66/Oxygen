/* 
 * Oxygen.Flow library
 * by karel66, 2020
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
    /// <summary>
    /// Common UI testing steps
    /// </summary>
    public partial class Flow
    {
        /// <summary>
        /// Run JavaScript.
        /// </summary>
        public static FlowStep Script(string script, params object[] args) =>
            (Context context) =>
            {
                try
                {
                    ((IJavaScriptExecutor)context.Driver).ExecuteScript(script, args);
                    return context;
                }
                catch (Exception x)
                {
                    return context.CreateProblem($"{nameof(Script)}: exception: " + x.Message);
                }
            };

        /// <summary>
        /// Switches to iframe in context.
        /// </summary>
        public static Context SwitchToFrame(Context context)
        {
            try
            {
                context.Driver.SwitchTo().Frame(context.Element);
                return context;
            }
            catch (Exception x)
            {
                return context.CreateProblem($"{nameof(Script)}: exception: " + x.Message);
            }
        }

        /// <summary>
        /// Locates and switches to the iframe.
        /// </summary>
        public static FlowStep SwitchToFrame(string iframeSelector) =>
                (Context context) => context | Find(iframeSelector) | SwitchToFrame;

        /// <summary>
        /// Switches to main or first frame in context.
        /// </summary>
        public static Context SwitchToDefault(Context context)
        {
            try
            {
                context.Driver.SwitchTo().DefaultContent();
                return context;
            }
            catch (Exception x)
            {
                return context.CreateProblem($"{nameof(Script)}: exception: " + x.Message);
            }
        }

        /// <summary>
        /// Executes the step only if the condition is true.
        /// </summary>
        public static FlowStep If(bool condition, FlowStep step) =>
            (Context context) => condition ? step(context) : context;

        /// <summary>
        /// Executes the step only if the condition returns true.
        /// </summary>
        public static FlowStep If(Func<Context, bool> condition, FlowStep step) =>
            (Context context) => condition(context) ? step(context) : context;

        /// <summary>
        /// Executes the step while the condition returns true.
        /// </summary>
        public static FlowStep While(Func<Context, bool> condition, FlowStep step) =>
            (Context context) =>
            {
                while (condition(context))
                {
                    context = step(context);
                }

                return context;
            };


        /// <summary>
        /// Retries until success or the given number of attempts has failed.
        /// </summary>
        public static bool TryUntilSuccess(Func<bool> success, int numberOfAttempts = 10)
        {
            if (success == null) throw new ArgumentNullException(nameof(success));

            int delay = 0;

            for (int i = 1; i <= numberOfAttempts; i++)
            {
                try
                {
                    if (success())
                    {
                        return true;
                    }
                }
                catch (Exception x)
                {
                    LogError(x.Message);
                }

                Log($"RETRY [{i}]");

                delay += 200;

                Thread.Sleep(delay);
            }

            return false;
        }

        /// <summary>
        /// Provides the step result context for action.
        /// </summary>
        public static FlowStep Use(FlowStep step, Action<Context> action) =>
            (Context context) =>
            {
                if (context.HasProblem) return context;

                if (action == null) return context.CreateProblem($"{nameof(Use)}: NULL action argument.");

                return step(context).Use(action);
            };


        /// <summary>
        /// Provides the step result element for action.
        /// </summary>
        public static FlowStep Use(FlowStep step, Action<RemoteWebElement> action) =>
            (Context context) =>
            {
                if (context.HasProblem) return context;

                if (action == null) return context.CreateProblem($"{nameof(Use)}: NULL action argument.");

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
                    return context.CreateProblem(x);
                }
            };

        /// <summary>
        /// Provides current context for action.
        /// </summary>
        public static FlowStep Use(Action<Context> action) =>
            (Context context) =>
            {
                if (context.HasProblem) return context;

                if (action == null) return context.CreateProblem($"{nameof(Use)}: NULL action argument.");

                return context.Use(action);
            };

        /// <summary>
        /// Provides current context element for the action.
        /// </summary>
        public static FlowStep UseElement(Action<RemoteWebElement> action) =>
            (Context context) =>
            {
                if (context.HasProblem) return context;

                if (action == null) return context.CreateProblem($"{nameof(UseElement)}: NULL action argument.");

                try
                {
                    action(context.Element);
                }
                catch (Exception x)
                {
                    return context.CreateProblem(x);
                }

                return context;
            };

        static FlowStep CollectionFilter(Func<ReadOnlyCollection<IWebElement>, IWebElement> filter) =>
            (Context context) =>
            {
                if (filter == null) return context.CreateProblem($"{nameof(CollectionFilter)}: NULL filter");
                if (context.Collection == null) return context.CreateProblem($"{nameof(CollectionFilter)}: missing collection");
                if (context.Collection.Count == 0) return context.CreateProblem($"{nameof(CollectionFilter)}: empty collection");

                IWebElement child = null;

                if (!TryUntilSuccess(() =>
                {
                    child = filter(context.Collection);
                    return child != null;
                }))
                {
                    return context.CreateProblem($"{nameof(CollectionFilter): failed}");
                }

                return context.NextContext(child as RemoteWebElement);
            };

        /// <summary>
        /// Returns element from the context Collection
        /// </summary>
        public static FlowStep FirstContainingText(string text) =>
            (Context context) =>
                    CollectionFilter(collection => collection.Where(e => e.Text.Contains(text))
                        .FirstOrDefault())(context);


        /// <summary>
        /// Returns element from context Collection
        /// </summary>
        public static FlowStep LastContainingText(string text) =>
            (Context context) =>
                    CollectionFilter(collection => collection.Where(e => e.Text.Contains(text))
                        .LastOrDefault())(context);


        /// <summary>
        /// Mouse click on the context Element
        /// </summary>
        public static Context Click(Context context)
        {
            if (context == null)
            {
                return context.CreateProblem(LogError($"{nameof(Click)}: NULL context", null));
            }

            var c = context.Element;

            if (c == null)
            {
                return context.CreateProblem(LogError($"{nameof(Click)}: NULL element!", null));
            }

            try
            {
                new Actions(context.Driver).MoveToElement(c).Perform();
            }
            catch (Exception x)
            {
                LogError(x.Message);
            }

            if (TryUntilSuccess(() =>
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
                return context;
            }
            else
            {
                return context.CreateProblem($"{nameof(Click)}: failed");
            }
        }

        /// <summary>
        /// Mouse click on page element retuned by given flow step
        /// </summary>

        public static FlowStep Click(FlowStep step) =>
            (Context context) => context | step | Click;

        /// <summary>
        /// Mouse click on page element returned by CSS selector
        /// </summary>
        public static FlowStep Click(string selector) =>
            (Context context) => 
                context 
                | Find(selector) 
                | Click;

        public static Context DblClick(Context context)
        {
            Click(context);

            try
            {
                new Actions(context.Driver).DoubleClick(context.Element).Perform();

                Thread.Sleep(10);

                return context;
            }
            catch (Exception x)
            {
                return context.CreateProblem(x);
            }
        }

        public static FlowStep DblClick(string selector) =>
            (Context context) => context | Find(selector) | DblClick;

        /// <summary>
        /// Sets text box, text area and combo text on page
        /// </summary>
        public static FlowStep SetText(string cssSelector, string text) =>
            (Context context) => context | Find(cssSelector) | SetText(text);

        /// <summary>
        /// Sets current context element text if text box, text area or dropdown list.
        /// </summary>
        public static FlowStep SetText(string text) =>
            (Context context) =>
            {
                if (context.Element == null)
                {
                    return context.CreateProblem($"{nameof(SetText)}: missing context Element");
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
                        return Select(context.NextContext(element), text);
                    }
                    else
                    {
                        return context.CreateProblem($"{nameof(SetText)}: unexpected tag <{element.TagName}>");
                    }
                }
                catch (Exception x)
                {
                    return context.CreateProblem(x);
                }

                return context;
            };

        /// <summary>
        /// Send enter key to the context element
        /// </summary>
        public static Context PressEnter(Context context)
        {
            var c = context.Element;

            if (c == null)
            {
                return context.CreateProblem("PressEnter: Missing context element");
            }

            try
            {
                new Actions(context.Driver).SendKeys(c, Keys.Enter).Perform();
            }
            catch (Exception x)
            {
                return context.CreateProblem(x);
            }

            return context;
        }

        /// <summary>
        /// Select display text in the combobox context element
        /// </summary>
        public static Context Select(Context context, string value)
        {
            var c = context.Element;

            if (c == null)
            {
                return context.CreateProblem($"{nameof(Select)}: Missing context element");
            }

            try
            {
                if (c.Text != value)
                {
                    var item = c.FindElementsByTagName("option").Where(opt => opt.Text == value).FirstOrDefault();

                    if (item == null)
                    {
                        return context.CreateProblem($"Can't find combo text '{value}'");
                    }

                    item.Click();
                }

                return context.NextContext(c);
            }

            catch (Exception x)
            {
                return context.CreateProblem(x);
            }

        }

        /// <summary>
        /// Clicks teh hyperlink and checks target window title.
        /// </summary>
        public static FlowStep FollowLink(string linkID, string targetTitle) =>
            (Context context) =>
            {
                TryUntilSuccess(() =>
                {
                    Click(linkID)(context);
                    return context.Title == targetTitle;
                });

                if (context.Title != targetTitle)
                {
                    return context.CreateProblem($"Expected title '{targetTitle}', actual '{context.Title}'");
                }

                return context;
            };

        public static FlowStep AssertAttributeValue(string attributeName, string expected) =>
            (Context context) =>
            {
                if (!context.HasElement)
                {
                    return context.CreateProblem($"AssertAttributeValue {attributeName}='{expected}': Missing context element.");
                }

                var actual = context.Element.GetAttribute(attributeName);

                return actual == expected ?
                    context : context.CreateProblem($"Expected {attributeName}='{expected}', actual {attributeName}='{actual}'");
            };

        public static FlowStep Assertion(Predicate<Context> predicate, string errorMessage) =>
            (Context context) => predicate(context) ? context : context.CreateProblem(errorMessage);

        public static FlowStep Assertion(Predicate<Context> predicate, Func<Context, string> errorMessage) =>
            (Context context) => predicate(context) ? context : context.CreateProblem(errorMessage(context));

    }
}
