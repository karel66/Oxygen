/*
Oxygen Flow library
*/

using OpenQA.Selenium;

using System;

namespace Oxygen
{
    /*
    Functions targeting the whole web page i.e. driver
    */
    public partial class Flow
    {
        /// <summary>
        /// Searches element from the whole page.
        /// </summary>
        public static FlowStep Find(string cssSelector, int index = 0) => (Context context) => ElementByCss(context.Driver, cssSelector, index)(context);

        /// <summary>
        /// Searches all elements from the whole page.
        /// </summary>
        public static FlowStep FindAll(string cssSelector) => (Context context) => CollectionByCss(context.Driver, cssSelector)(context);

        /// <summary>
        /// Searches from whole page.
        /// </summary>
        public static FlowStep FindOnXPath(string xpath, int index = 0) => (Context context) => ElementByXPath(context.Driver, xpath, index)(context);

        /// <summary>
        /// Searches from whole page.
        /// </summary>
        public static FlowStep FindAllOnXPath(string xpath) => (Context context) => CollectionByXPath(context.Driver, xpath)(context);

        /// <summary>
        /// Switches to iframe in context.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Context SwitchToFrame(Context context)
        {
            O($"SwitchToFrame");
            try
            {
                context.Driver.SwitchTo().Frame(context.Element);
                return context;
            }
            catch (Exception x)
            {
                return context.NewProblem($"{nameof(Script)}: exception: " + x.Message);
            }
        }


        public static FlowStep Script(string script, params object[] args) => (Context context) =>
        {
            O($"{nameof(Script)}: {script}");
            try
            {
                ((IJavaScriptExecutor)context.Driver).ExecuteScript(script, args);
                return context;
            }
            catch (Exception x)
            {
                return context.NewProblem($"{nameof(Script)}: exception: " + x.Message);
            }
        };


        /// <summary>
        /// Check for existence of an element
        /// </summary>
        public static bool Exists(Context context, string cssSelector, int waitMs = 0) => ExistsByCss(context.Driver, cssSelector, waitMs);

        /// <summary>
        /// Check for existence of an element
        /// </summary>
        public static bool ExistsOnXPath(Context context, string xpath, int waitMs = 0) => ExistsByXPath(context.Driver, xpath, waitMs);

        /// <summary>
        /// Executes the step only if the condition is true.
        /// </summary>
        public static FlowStep If(bool condition, FlowStep step) => (Context context) =>
            condition ? step(context) : context;

        /// <summary>
        /// Executes the step only if the condition returns true.
        /// </summary>
        public static FlowStep If(Func<Context, bool> condition, FlowStep step) => (Context context) =>
            condition(context) ? step(context) : context;

        /// <summary>
        /// Executes the step if element by the selector is found
        /// </summary>
        public static FlowStep IfExists(string selector, FlowStep step, int waitMs = 0) => (Context context) =>
            ExistsByCss(context.Driver, selector, waitMs) ? step(context) : context;

        /// <summary>
        /// Executes the step while the condition returns true.
        /// </summary>
        public static FlowStep While(Func<Context, bool> condition, FlowStep step) => (Context context) =>
        {
            while (condition(context))
            {
                context = step(context);
            }

            return context;
        };


    }
}
