using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.XPath;

namespace DodoOpenId
{
    /// <summary>
    /// The Type element in an XRDS document.
    /// </summary>
    public class TypeElement : XrdsNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeElement"/> class.
        /// </summary>
        /// <param name="typeElement">The type element.</param>
        /// <param name="parent">The parent.</param>
        public TypeElement(XPathNavigator typeElement, ServiceElement parent) :
            base(typeElement, parent)
        {
        }

        /// <summary>
        /// Gets the URI.
        /// </summary>
        public string Uri
        {
            get { return Node.Value; }
        }
    }
}
