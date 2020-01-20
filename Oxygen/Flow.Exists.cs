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
        public static class Exists
        {
            public static bool Element(Context context, string cssSelector) => ExistsByCss(context.Driver, cssSelector);
            public static bool ByXPath(Context context, string xpath) => ExistsByXPath(context.Driver, xpath);
        }
    }
}
