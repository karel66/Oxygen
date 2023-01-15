/*
* Oxygen.Flow library
* by karel66, 2023
*/

namespace Oxygen
{   
    /// <summary>
    /// Relative searches (current element context).
    /// </summary>

    public partial class Flow
    {
        /// <summary>
        /// Searches child element of the current context element.
        /// </summary>
        public static FlowStep RelativeFind(string cssSelector, int index = 0) => 
            (Context context) => ElementByCss(context.Element, cssSelector, index)(context);

        /// <summary>
        /// Searches all child elements of the current context element.
        /// </summary>
        public static FlowStep RelativeFindAll(string cssSelector) => 
            (Context context) => CollectionByCss(context.Element, cssSelector)(context);

        /// <summary>
        /// Searches child element of the current context element.
        /// </summary>
        public static FlowStep RelativeFindOnXPath(string xpath, int index = 0) => 
            (Context context) => ElementByXPath(context.Element, xpath, index)(context);

        /// <summary>
        /// Searches all child elements of the current context element.
        /// </summary>
        public static FlowStep RelativeFindAllOnXPath(string xpath) => 
            (Context context) => CollectionByXPath(context.Element, xpath)(context);

    }
}
