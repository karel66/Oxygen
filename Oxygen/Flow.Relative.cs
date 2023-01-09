/*
 * Oxygen.Flow library
 * by karel66, 2020
*/

namespace Oxygen
{
    public partial class Flow
    {
        /// <summary>
        /// Relative searches (current element context).
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

        }
    }
}
