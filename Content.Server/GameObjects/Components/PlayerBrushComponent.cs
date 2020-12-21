using System;
using System.Collections.Generic;
using Content.Shared.GameObjects;
using Content.Shared.GameObjects.Components;
using Content.Server;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Players;
using Robust.Shared.Serialization;
using Robust.Shared.GameObjects;
using Robust.Shared.Interfaces.GameObjects;
using Robust.Shared.Interfaces.Network;
using Robust.Shared.Interfaces.Map;
using Robust.Shared.Map;
using Robust.Shared.IoC;
using Robust.Shared.Log;

namespace Content.Server.GameObjects.Components
{
    /// <summary>
    ///     Player controls!
    ///     On the server, this component exists to actually apply the brushwork.
    /// </summary>
    [RegisterComponent]
    [ComponentReference(typeof(SharedPlayerBrushComponent))]
    public class PlayerBrushComponent : SharedPlayerBrushComponent
    {
        [Dependency] private IngressExperienceManager _ingressExperienceManager = default!;
        [Dependency] private ITileDefinitionManager _tileDefinitionManager = default!;
        /// <inheritdoc />
        public override void HandleNetworkMessage(ComponentMessage message, INetChannel netChannel, ICommonSession session) {
            base.HandleNetworkMessage(message, netChannel, session);
            if (session == null)
                return;
            if (session.AttachedEntity != Owner)
                return;
            if (message is PlayerBrushApplyMessage)
            {
                var msg = (PlayerBrushApplyMessage) message;
                if (msg.Type < 0)
                    return;
                if (msg.Type >= _tileDefinitionManager.Count)
                    return;
                if (_ingressExperienceManager.MapExtent.Contains(msg.Position))
                {
                    var tile = new Tile(msg.Type);
                    if (_ingressExperienceManager.IngressGrid.GetTileRef(msg.Position).Tile != tile)
                    {
                        Logger.WarningS("c.s.go.co.brush", "{0} at {1} = {2}", session, msg.Position, msg.Type);
                        _ingressExperienceManager.IngressGrid.SetTile(msg.Position, tile);
                    }
                }
            }
        }
    }
}

