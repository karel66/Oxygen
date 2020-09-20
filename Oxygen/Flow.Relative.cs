/*
Oxygen Flow library
*/

namespace Oxygen
{
    public partial class Flow
    {
        /// <summary>
        /// Searches from the current context element.
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
            /// Executes the step if child element by the selector is found
            /// </summary>
            public static FlowStep IfExists(string selector, FlowStep step, int waitMs = 0) => (Context context) =>
                ExistsByCss(context.Element, selector, waitMs) ? step(context) : context;

            /// <summary>
            /// Check for existence of an element
            /// </summary>
            public static bool Exists(Context context, string cssSelector, int waitMs = 0) => ExistsByCss(context.Element, cssSelector, waitMs);

            /// <summary>
            /// Check for existence of an element
            /// </summary>
            public static bool ExistsOnXPath(Context context, string xpath, int waitMs = 0) => ExistsByXPath(context.Element, xpath, waitMs);

        }
    }
}
