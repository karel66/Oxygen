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
            public static FlowStep Element(string cssSelector, int index = 0) => (Context context) => ElementByCss(context.Element, cssSelector, index)(context);

            /// <summary>
            /// Searches all child elements of the current context element.
            /// </summary>
            public static FlowStep Collection(string cssSelector) => (Context context) => CollectionByCss(context.Element, cssSelector)(context);

            /// <summary>
            /// Searches child element of the current context element.
            /// </summary>
            public static FlowStep ByXPath(string xpath, int index = 0) => (Context context) => ElementByXPath(context.Element, xpath, index)(context);

            /// <summary>
            /// Searches all child elements of the current context element.
            /// </summary>
            public static FlowStep AllByXPath(string xpath) => (Context context) => CollectionByXPath(context.Element, xpath)(context);


            protected static class Exists
            {
                public static bool Element(Context context, string selector) => ExistsByCss(context.Element, selector);

                public static bool ByXPath(Context context, string xpath) => ExistsByXPath(context.Element, xpath);
            }
        }
    }
}
