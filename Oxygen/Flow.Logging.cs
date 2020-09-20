/*
Oxygen Flow library
*/

using System;
using OpenQA.Selenium.Remote;

namespace Oxygen
{
    public partial class Flow
    {
        const string LogLine = "*************************************************************************";


        /// <summary>
        /// Trace output to stdout.
        /// </summary>
        /// <param name="message"></param>
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


        public static string LogError(string msg, Exception x = null)
        {
            var result = "FAILED " + msg;
            if (x != null) result += ": " + x.ToString();

            O(LogLine);
            O(result);
            O(LogLine);

            return result;
        }
    }
}
