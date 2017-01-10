using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DodoNet.Overlay;

namespace DodoService
{
    public abstract class DodoTask : Timer
    {
        public DodoApplication CurrentApp { get; set; }

        public DodoTask(DodoApplication app, long delayMs) 
            : base(delayMs)
        {
            CurrentApp = app;
        }
    }
}
