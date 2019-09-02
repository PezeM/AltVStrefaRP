using System;
using System.Collections.Generic;
using System.Drawing;
using AltV.Net.Data;
using AltVStrefaRPServer.Helpers;
using AltVStrefaRPServer.Models.Core;
using AltVStrefaRPServer.Modules.Networking;
using Serilog;

namespace AltVStrefaRPServer.Modules.Core
{
    public sealed class MarkerManager
    {
        private static readonly Lazy<MarkerManager> lazy = new Lazy<MarkerManager>(() => new MarkerManager());
        public static MarkerManager Instance => lazy.Value;
        private readonly Dictionary<int, Marker> _markers;
        private readonly IdGenerator _idGenerator;

        private MarkerManager()
        {
            _markers = new Dictionary<int, Marker>();
            _idGenerator = new IdGenerator();
        }

        public Marker AddMarker(int type, Position position, Color color, Position scale, int range = 20, int dimension = 0)
        {
            var marker = new Marker(_idGenerator.GetNextId(), type, position, color, scale, range, dimension);

            lock (_markers)
            {
                if(_markers.TryAdd(marker.Id, marker))
                {
                    OnAddMarker(marker);
                    Log.ForContext<MarkerManager>().Debug("Created new marker ID ({markerId}) Type {markerType} at position {position}", 
                        marker.Id, marker.Type, position);
                    return marker;
                }
            }

            Log.ForContext<MarkerManager>().Error("Creating marker ID ({markerId}) at position {position} failed.", marker.Id, position);
            return null;
        }

        private void OnAddMarker(Marker marker)
        {
            NetworkingManager.Instance.AddNewMarker(marker);
        }

        public bool RemoveMarker(Marker marker)
        {
            lock (_markers)
            {
                if (!_markers.Remove(marker.Id)) return false;
            }

            Log.ForContext<MarkerManager>().Debug("Removed marker ID({markerId})", marker.Id);
            OnMarkerRemove(marker);
            return true;
        }

        private void OnMarkerRemove(Marker marker)
        {
            NetworkingManager.Instance.RemoveNetworkingEntity(marker.NetworkingEntityId);
        }
    }
}
