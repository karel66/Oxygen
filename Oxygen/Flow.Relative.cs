/*
Oxygen Flow library
*/

namespace Oxygen
{
    public partial class Flow
    {
        /// <summary>
        /// Realtice searches (current element context).
        /// </summary>
        protected static class Relative
        {
            /// <summary>
            /// Searches child element of the current context element.
            /// </summary>
            public static FlowStep Find(string cssSelector, int index = 0) => (Context context) => ElementByCss(context.Element, cssSelector, index)(context);

            /// <summary>
            /// Searches all child elements of the current context element.
            /// </summary>
            public static FlowStep FindAll(string cssSelector) => (Context context) => CollectionByCss(context.Element, cssSelector)(context);

            /// <summary>
            /// Searches child element of the current context element.
            /// </summary>
            public static FlowStep FindOnXPath(string xpath, int index = 0) => (Context context) => ElementByXPath(context.Element, xpath, index)(context);

            /// <summary>
            /// Searches all child elements of the current context element.
            /// </summary>
            public static FlowStep FindAllOnXPath(string xpath) => (Context context) => CollectionByXPath(context.Element, xpath)(context);

            /// <summary>
            /// Check for existence of an element.
            /// </summary>
            public static bool Exists(Context context, string cssSelector) => ExistsByCss(context.Element, cssSelector);

            /// <summary>
            /// Check for existence of an element.
            /// </summary>
            public static bool ExistsOnXPath(Context context, string xpath) => ExistsByXPath(context.Element, xpath);

            /// <summary>
            /// Executes the step if child element by the selector is found.
            /// </summary>
            public static FlowStep IfExists(string selector, FlowStep step) =>
                (Context context) => ExistsByCss(context.Element, selector) ? step(context) : context;
        }
    }
}
