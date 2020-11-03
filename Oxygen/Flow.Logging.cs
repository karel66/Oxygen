/*
Oxygen Flow library
*/

using System;
using OpenQA.Selenium.Remote;

namespace Oxygen
{
    /// <summary>
    /// Logging methods
    /// </summary>
    public partial class Flow
    {
        const string LogLine = "*************************************************************************";


        /// <summary>
        /// Trace timestamp and message to stdout.
        /// </summary>
        public static string O(string message)
        {
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture)} {message}");
            return message;
        }

        /// <summary>
        /// Output message in flow step
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Func<Context, Context> Trace(string message) => (Context context) =>
         {
             O(message);
             return context;
         };

        /// <summary>
        /// Trace message and optional error stack to stdout.
        /// </summary>
        public static string LogError(string message, Exception x = null)
        {
            var result = "FAILED " + message;
            if (x != null) result += ": " + x.ToString();

            O(LogLine);
            O(result);
            O(LogLine);

            return result;
        }
    }
}
