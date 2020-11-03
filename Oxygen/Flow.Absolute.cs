/*
Oxygen Flow library
*/

using OpenQA.Selenium;

using System;

namespace Oxygen
{
    /// <summary>
    /// Global searches (driver context).
    /// </summary>
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
        /// Check for existence of an element
        /// </summary>
        public static bool Exists(Context context, string cssSelector, int waitMs = 0) => ExistsByCss(context.Driver, cssSelector, waitMs);

        /// <summary>
        /// Check for existence of an element
        /// </summary>
        public static bool ExistsOnXPath(Context context, string xpath, int waitMs = 0) => ExistsByXPath(context.Driver, xpath, waitMs);

        /// <summary>
        /// Executes the step if element by the selector is found
        /// </summary>
        public static FlowStep IfExists(string selector, FlowStep step, int waitMs = 0) => (Context context) =>
            ExistsByCss(context.Driver, selector, waitMs) ? step(context) : context;
    }
}
