using System;
using Content.Shared.GameObjects.Components;
using Content.Shared;
using Robust.Shared.Players;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Network;
using Timer = Robust.Shared.Timing.Timer;

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
                // Confirm that tile type is vaguely valid
                if (msg.Type < 0)
                    return;
                if (msg.Type >= _tileDefinitionManager.Count)
                    return;
                // Is player performing this write as an admin? (acts as "sudo" barrier to prevent accidents)
                var admin = Owner.HasComponent<PlayerAdminComponent>() && msg.InAdminMode;
                // Confirm tile in rotation if user is not admin
                if (!admin)
                    if (((ContentTileDefinition) _tileDefinitionManager[msg.Type]).Rotation == -1)
                        return;
                // Confirm in map bounds
                if (!(_ingressExperienceManager.MapExtent.Contains(msg.Position) || admin))
                    return;
                // Get old tile and check protection
                var oldTile = _ingressExperienceManager.IngressGrid.GetTileRef(msg.Position).Tile;
                if ((!admin) && ((oldTile.Data & (ushort) TileFlags.Protected) != 0))
                {
                    Logger.WarningS("c.s.go.co.brush", "{0} at {1} X {2}", session, msg.Position, msg.Type);
                    return;
                }
                // Prepare new tile, filtering flags if regular player
                var tile = new Tile(msg.Type, (ushort) (admin ? msg.Data : 0));
                if (oldTile != tile)
                {
                    Logger.WarningS("c.s.go.co.brush", "{0} at {1} = {2}", session, msg.Position, msg.Type);
                    if (oldTile.TypeId == tile.TypeId)
                    {
                        // RobustToolbox bug workaround, see MapChunk.cs SetTile "is same tile" condition
                        var tmp = (ushort) ((tile.TypeId == 0) ? 1 : 0);
                        _ingressExperienceManager.IngressGrid.SetTile(msg.Position, new Tile(tmp, (ushort) 1));
                        Timer.Spawn(TimeSpan.FromSeconds(1), () => _ingressExperienceManager.IngressGrid.SetTile(msg.Position, tile));
                    }
                    else
                    {
                        _ingressExperienceManager.IngressGrid.SetTile(msg.Position, tile);
                    }
                }
            }
        }
    }
}

