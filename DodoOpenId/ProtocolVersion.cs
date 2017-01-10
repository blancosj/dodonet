using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DodoOpenId
{
    /// <summary>
    /// An enumeration of the OpenID protocol versions supported by this library.
    /// </summary>
    public enum ProtocolVersion
    {
        /// <summary>
        /// OpenID Authentication 1.0
        /// </summary>
        V10,
        /// <summary>
        /// OpenID Authentication 1.1
        /// </summary>
        V11,
        /// <summary>
        /// OpenID Authentication 2.0
        /// </summary>
        V20,
    }
}
