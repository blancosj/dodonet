using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DodoService.Cleaners
{
    interface ICleaner
    {
        Stream Clean(Stream st);
    }
}
