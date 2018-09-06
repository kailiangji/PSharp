// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the repo root for full license information.
// ------------------------------------------------------------------------------------------------

using System;

namespace Microsoft.PSharp.IO
{
    /// <summary>
    /// Static class implementing debug reporting methods.
    /// </summary>
    public static class Debug
    {
        /// <summary>
        /// Checks if debugging is enabled.
        /// </summary>
        internal static bool IsEnabled;

        /// <summary>
        /// Static constructor.
        /// </summary>
        static Debug()
        {
            IsEnabled = false;
        }

        /// <summary>
        /// Writes the debugging information to the output stream. The
        /// print occurs only if debugging is enabled.
        /// </summary>
        /// <param name="format">Text</param>
        /// <param name="args">Arguments</param>
        public static void Write(string format, params object[] args)
        {
            if (IsEnabled)
            {
                string message = Utilities.Format(format, args);
                Console.Write(message);
            }
        }

        /// <summary>
        /// Writes the debugging information, followed by the current
        /// line terminator, to the output stream. The print occurs
        /// only if debugging is enabled.
        /// </summary>
        /// <param name="format">Text</param>
        /// <param name="args">Arguments</param>
        public static void WriteLine(string format, params object[] args)
        {
            if (IsEnabled)
            {
                string message = Utilities.Format(format, args);
                Console.WriteLine(message);
            }
        }
    }
}
