/*
Oxygen Flow library
*/

using System;
using System.Collections.ObjectModel;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace Oxygen
{
    public partial class Flow
    {
        /// <summary>
        /// Searches element from the whole web page.
        /// </summary>
        public static FlowStep Element(string cssSelector, int index = 0) => (Context context) => ElementByCss(context.Driver, cssSelector, index)(context);

        /// <summary>
        /// Searches element from the whole web page.
        /// </summary>
        public static FlowStep Element(HtmlTag tag, int index = 0) => (Context context) => Element(tag.ToString(), index)(context);

        /// <summary>
        /// Searches all elements from the whole web page.
        /// </summary>
        public static FlowStep Collection(string cssSelector) => (Context context) => CollectionByCss(context.Driver, cssSelector)(context);

        /// <summary>
        /// Searches all elements from the whole web page.
        /// </summary>
        public static FlowStep Collection(HtmlTag tag) => (Context context) => Collection(tag.ToString())(context);

        /// <summary>
        /// Searches from whole web page.
        /// </summary>
        public static FlowStep ByXPath(string xpath, int index = 0) => (Context context) => ElementByXPath(context.Driver, xpath, index)(context);

        /// <summary>
        /// Searches from whole web page.
        /// </summary>
        public static FlowStep AllByXPath(string xpath) => (Context context) => CollectionByXPath(context.Driver, xpath)(context);

        public static FlowStep Script(string script, params object[] args) => (Context context) =>
        {
            O($"Script: {script}");
            try
            {
                ((IJavaScriptExecutor)context.Driver).ExecuteScript(script);
            }
            catch (Exception x)
            {
                return context.Problem("Script failed: " + x.Message);
            }
            return context;
        };

        /// <summary>
        /// Executes the step only if the condition is true.
        /// </summary>
        public static FlowStep If(bool condition, FlowStep step) => (Context context) => condition ? step(context) : context;

        public static FlowStep While(Func<Context, bool> predicate, FlowStep step) => (Context context) =>
        {
            Context c = context;
            while (predicate(c))
            {
                if (step != null)
                {
                    c = step(c);
                }
            }
            return c;
        };

        /// <summary>
        /// Executes the step only if the condition returns true.
        /// </summary>
        public static FlowStep If(Func<Context, bool> condition, FlowStep step) => (Context context) => condition(context) ? step(context) : context;
    }
}
