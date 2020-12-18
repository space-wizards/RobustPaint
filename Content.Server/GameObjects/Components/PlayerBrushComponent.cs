using System;
using System.Collections.Generic;
using Content.Shared.GameObjects;
using Content.Shared.GameObjects.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Players;
using Robust.Shared.Serialization;
using Robust.Shared.GameObjects;
using Robust.Shared.Interfaces.GameObjects;
using Robust.Shared.Interfaces.Network;
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
        /// <inheritdoc />
        public override void HandleNetworkMessage(ComponentMessage message, INetChannel netChannel, ICommonSession session) {
            base.HandleNetworkMessage(message, netChannel, session);
            if (session == null)
                return;
            if (session.AttachedEntity != Owner)
                return;
            if (message is PlayerBrushApplyMessage)
            {
                // TODO
            }
        }
    }
}

