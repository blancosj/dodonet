using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace DodoOpenId
{
    /// <summary>
    /// A collection of error checking and reporting methods.
    /// </summary>
    internal static class ErrorUtilities
    {
        /// <summary>
        /// Throws a <see cref="ProtocolException"/> if some <paramref name="condition"/> evaluates to false.
        /// </summary>
        /// <param name="condition">True to do nothing; false to throw the exception.</param>
        /// <param name="faultedMessage">The message being processed that would be responsible for the exception if thrown.</param>
        /// <param name="errorMessage">The error message for the exception.</param>
        /// <param name="args">The string formatting arguments, if any.</param>
        /// <exception cref="ProtocolException">Thrown if <paramref name="condition"/> evaluates to <c>false</c>.</exception>
        internal static void VerifyProtocol(bool condition, IProtocolMessage faultedMessage, string errorMessage, params object[] args)
        {
            if (!condition)
            {
                throw new ProtocolException(string.Format(CultureInfo.CurrentCulture, errorMessage, args), faultedMessage);
            }
        }

        /// <summary>
        /// Throws a <see cref="ProtocolException"/> if some <paramref name="condition"/> evaluates to false.
        /// </summary>
        /// <param name="condition">True to do nothing; false to throw the exception.</param>
        /// <param name="message">The error message for the exception.</param>
        /// <param name="args">The string formatting arguments, if any.</param>
        /// <exception cref="ProtocolException">Thrown if <paramref name="condition"/> evaluates to <c>false</c>.</exception>
        internal static void VerifyProtocol(bool condition, string message, params object[] args)
        {
            if (!condition)
            {
                var exception = new ProtocolException(string.Format(CultureInfo.CurrentCulture, message, args));
                throw exception;
            }
        }

        /// <summary>
        /// Verifies that some given value is not null.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="paramName">Name of the parameter, which will be used in the <see cref="ArgumentException"/>, if thrown.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        internal static void VerifyArgumentNotNull(object value, string paramName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        /// <summary>
        /// Throws a <see cref="ProtocolException"/>.
        /// </summary>
        /// <param name="message">The message to set in the exception.</param>
        /// <param name="args">The formatting arguments of the message.</param>
        /// <returns>
        /// An InternalErrorException, which may be "thrown" by the caller in order
        /// to satisfy C# rules to show that code will never be reached, but no value
        /// actually is ever returned because this method guarantees to throw.
        /// </returns>
        /// <exception cref="ProtocolException">Always thrown.</exception>
        internal static Exception ThrowProtocol(string message, params object[] args)
        {
            VerifyProtocol(false, message, args);

            // we never reach here, but this allows callers to "throw" this method.
            return new InternalErrorException();
        }

        /// <summary>
        /// Checks a condition and throws an internal error exception if it evaluates to false.
        /// </summary>
        /// <param name="condition">The condition to check.</param>
        /// <param name="errorMessage">The message to include in the exception, if created.</param>
        /// <exception cref="InternalErrorException">Thrown if <paramref name="condition"/> evaluates to <c>false</c>.</exception>
        internal static void VerifyInternal(bool condition, string errorMessage)
        {
            if (!condition)
            {
                ThrowInternal(errorMessage);
            }
        }

        /// <summary>
        /// Checks a condition and throws an internal error exception if it evaluates to false.
        /// </summary>
        /// <param name="condition">The condition to check.</param>
        /// <param name="errorMessage">The message to include in the exception, if created.</param>
        /// <param name="args">The formatting arguments.</param>
        /// <exception cref="InternalErrorException">Thrown if <paramref name="condition"/> evaluates to <c>false</c>.</exception>
        internal static void VerifyInternal(bool condition, string errorMessage, params object[] args)
        {
            if (!condition)
            {
                errorMessage = string.Format(CultureInfo.CurrentCulture, errorMessage, args);
                throw new InternalErrorException(errorMessage);
            }
        }

        /// <summary>
        /// Throws an internal error exception.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <returns>Nothing.  But included here so callers can "throw" this method for C# safety.</returns>
        /// <exception cref="InternalErrorException">Always thrown.</exception>
        internal static Exception ThrowInternal(string errorMessage)
        {
            // Since internal errors are really bad, take this chance to
            // help the developer find the cause by breaking into the
            // debugger if one is attached.
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }

            throw new InternalErrorException(errorMessage);
        }
    }
}
