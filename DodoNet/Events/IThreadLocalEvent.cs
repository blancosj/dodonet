using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DodoNet
{
    public interface IThreadLocalEvent
    {
        /// <summary>
        /// se ha completado
        /// </summary>
        bool IsCompleted { get; set; }

        // cancelar evento
        void Cancel();
    }
}
