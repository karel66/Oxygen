/*
 * Oxygen.Flow library
 * by karel66, 2020
*/

using System;

namespace Oxygen
{
    /// <summary>
    /// Logging methods
    /// </summary>
    public partial class Flow
    {
        /// <summary>
        /// Trace timestamp and message to stdout.
        /// </summary>
        public static string Log(string message)
        {
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture)} {message}");
            return message;
        }

        /// <summary>
        /// Trace message and optional error stack to stdout.
        /// </summary>
        public static string LogError(string message, Exception x = null)
        {
            var result = "*** ERROR *** " + message;
            if (x != null) result += ": " + x.ToString();
            Log(result);
            return result;
        }
    }
}
