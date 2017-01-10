using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace DodoOpenId
{
    /// <summary>
    /// An interface that extension messages must implement.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces", Justification = "Extension messages may gain members later on.")]
    public interface IExtensionMessage : IMessage
    {
    }
}
