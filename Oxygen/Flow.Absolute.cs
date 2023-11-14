/*
* Oxygen.Flow library
* by karel66, 2023
*/

namespace Elements.Oxygen
{
    /// <summary>
    /// Global searches (driver context).
    /// </summary>
    public partial class Flow
    {
        /// <summary>
        /// Searches element from the whole page.
        /// </summary>
        public static FlowStep Find(string selector, int index = 0) => (Context context) => ElementByCss(context.Driver, selector, index)(context);
        public static FlowStep FindLast(string selector) => (Context context) => ElementByCss(context.Driver, selector, -1)(context);

        /// <summary>
        /// Searches all elements from the whole page.
        /// </summary>
        public static FlowStep FindAll(string selector) => (Context context) => CollectionByCss(context.Driver, selector)(context);

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
        public static bool Exists(Context context, string selector, double waitSeconds) => ExistsByCss(context.Driver, selector, waitSeconds);

        public static bool ExistsOnXPath(Context context, string xpath, double waitSeconds) => ExistsByXPath(context.Driver, xpath, waitSeconds);

        /// <summary>
        /// Executes the step if element by the selector is found
        /// </summary>
        public static FlowStep IfExists(string selector, FlowStep onTrue = null, FlowStep onFalse = null, double waitSeconds = 0) =>
                (Context context) =>
                {
                    if (ExistsByCss(context.Driver, selector, waitSeconds))
                    {
                        if (onTrue != null) return context | onTrue;
                    }
                    else
                    {
                        if (onFalse != null) return context | onFalse;
                    }

                    return context;
                };
    }
}
