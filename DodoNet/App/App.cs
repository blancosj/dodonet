using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DodoNet.Overlay;

namespace DodoNet
{
    public abstract class App
    {
        /// <summary>
        /// This method is called whenever a new neighbour arrives.
        /// </summary>
        public abstract void NewNeighbour(NodeBind nodeBind);

        /// <summary>
        /// This method is called whenever a neighbour is removed from a routing table.
        /// </summary>
        /// <param name="nodeId">The nodeId of the neighbour that was removed</param>
        /// <param name="failure">
        /// If true, the neighbour was removed because it failed.
        /// If false, the neighbour was removed because it was replaced with a better entry
        /// and it has not necessarily failed.
        /// </param>
        public abstract void NeighbourRemoved(NodeBind nodeBind, bool failure);

        // TODO: We will need additional functionality like this for CLB.

        // Método al que se puede llamar si se quiere notificar al Application de la llegada de un mensaje.
        // Se le llamará desde el método ArrivedAtDestination del mensaje.
        public abstract void MessageReceived(OverlayMessage msg);
    }
}
