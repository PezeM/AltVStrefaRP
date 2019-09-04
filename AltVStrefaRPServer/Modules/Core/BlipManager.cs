using System;
using System.Collections.Generic;
using System.Linq;
using AltV.Net;
using AltVStrefaRPServer.Helpers;
using AltVStrefaRPServer.Models.Core;
using AltVStrefaRPServer.Models.Interfaces.Managers;
using AltVStrefaRPServer.Services.MValueAdapters;
using Serilog;
using MValue = AltV.Net.Elements.Args.MValue;
using Position = AltV.Net.Data.Position;

namespace AltVStrefaRPServer.Modules.Core
{
    public sealed class BlipManager : IBlipManager
    {
        private static readonly Lazy<BlipManager> lazy = new Lazy<BlipManager>(() => new BlipManager());
        public static BlipManager Instance => lazy.Value;
        private readonly Dictionary<int, BlipWrapper> _blips;
        private readonly IIdGenerator _idGenerator;
        private MValue _blipsMValue;

        public IMValueBaseAdapter BlipAdapter { get; set; }

        private BlipManager()
        {
            _blipsMValue = new MValue();
            _idGenerator = new IdGenerator();
            _blips = new Dictionary<int, BlipWrapper>();
            BlipAdapter = new BlipWrapperAdapter();
        }

        public List<BlipWrapper> GetBlips()
        {
            return _blips.Values.ToList();
        }

        public IBlipWrapper CreateBlip(string blipName, int blipSprite, int blipColor, Position position, float scale = 1)
        {
            var newBlip = new BlipWrapper(blipName, blipSprite, blipColor, position, scale);
            var nextId = _idGenerator.GetNextId();
            lock (_blips)
            {
                if (_blips.TryAdd(nextId, newBlip))
                {
                    newBlip.Id = nextId;
                    OnBlipAdd(newBlip);
                    Log.ForContext<BlipManager>().Debug("Created new blip with ID({blipId}) Name {blipName}", nextId, blipName);
                    return newBlip;
                }
            }

            Log.ForContext<BlipManager>().Warning("Creating blip with ID {blipId} Name {blipName} Sprite {blipSprite} failed", nextId, blipName, blipSprite);
            return null;
        }

        public bool Remove(IBlipWrapper blip)
        {
            return Remove(blip.Id);
        }

        public bool Remove(int id)
        {
            BlipWrapper removedBlip;
            lock (_blips)
            {
                if (!_blips.Remove(id, out removedBlip)) return false;
            }

            OnBlipRemove(removedBlip);
            return true;
        }

        internal void UpdateBlipName(BlipWrapper blipWrapper, string value)
        {
            Alt.EmitAllClients("blipManagerUpdateBlipName", blipWrapper.Id, value);
        }

        internal void UpdateBlipPosition(BlipWrapper blipWrapper, Position value)
        {
            Alt.EmitAllClients("blipManagerUpdateBlipPosition", blipWrapper.Id, value);
        }

        internal void UpdateBlipSprite(BlipWrapper blipWrapper, int value)
        {
            Alt.EmitAllClients("blipManagerUpdateBlipSprite", blipWrapper.Id, value);
        }

        internal void UpdateBlipColor(BlipWrapper blipWrapper, int value)
        {
            Alt.EmitAllClients("blipManagerUpdateBlipColor", blipWrapper.Id, value);
        }

        internal void UpdateBlipScale(BlipWrapper blipWrapper, float value)
        {
            Alt.EmitAllClients("blipManagerUpdateBlipScale", blipWrapper.Id, value);
        }

        private void OnBlipAdd(BlipWrapper newBlip)
        {
            // Emit event to all players to remove specific blip
            // Update blip mvalue definition
            _blipsMValue = MValue.CreateFromObject(_blips.Values);
            Alt.EmitAllClients("blipManagerAddedNewBlip", newBlip);
        }

        private void OnBlipRemove(IBlipWrapper removedBlip)
        {
            // Propably event to all players that specific blip was removed
            _blipsMValue = MValue.CreateFromObject(_blips.Values);
            Alt.EmitAllClients("blipManagerRemovedBlip", removedBlip.Id);
        }
    }
}
