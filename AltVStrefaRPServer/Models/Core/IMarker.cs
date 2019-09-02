using AltV.Net.Data;
using AltVStrefaRPServer.Models.Interfaces;

namespace AltVStrefaRPServer.Models.Core
{
    public interface IMarker : IPosition
    {
        int Id { get; set; }
        int NetworkingEntityId { get; set; }

        /// <summary>
        /// Marker type. https://wiki.rage.mp/index.php?title=Marker::Marker
        /// </summary>
        int Type { get; set; }

        /// <summary>
        /// Range of the marker
        /// </summary>
        int Range { get; set; }

        /// <summary>
        /// Dimension of the marker. 0 for every dimension
        /// </summary>
        int Dimension { get; set; }

        float ScaleX { get; set; }
        float ScaleY { get; set; }
        float ScaleZ { get; set; }
        int Red { get; set; }
        int Green { get; set; }
        int Blue { get; set; }
        int Alpha { get; set; }
        bool DestroyMarker();
    }
}