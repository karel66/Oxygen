/*
Oxygen Flow library
*/

namespace Oxygen
{
    public partial class Flow
    {
        /// <summary>
        /// Check for existence of element on page
        /// </summary>
        protected static class Exists
        {
            public static bool Element(Context context, string cssSelector, int waitMs = 0) => ExistsByCss(context.Driver, cssSelector, waitMs);
            public static bool ByXPath(Context context, string xpath, int waitMs = 0) => ExistsByXPath(context.Driver, xpath, waitMs);
        }
    }
}
