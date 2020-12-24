using System;
using System.Collections.Generic;
using Content.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Players;
using Robust.Shared.Serialization;
using Robust.Shared.GameObjects;
using Robust.Shared.Interfaces.GameObjects;
using Robust.Shared.Interfaces.Network;
using Robust.Shared.Log;

namespace Content.Shared.GameObjects.Components
{
    /// <summary>
    ///     Player controls!
    /// </summary>
    public abstract class SharedPlayerBrushComponent : Component
    {
        public override string Name => "PlayerBrush";
        public override uint? NetID => ContentNetIDs.PLAYER_BRUSH;
    }
﻿
    [Serializable, NetSerializable]
    public class PlayerBrushApplyMessage : ComponentMessage
    {
        public readonly Vector2i Position;
        public readonly ushort Type;
        public readonly ushort Data;
        public readonly bool InAdminMode;
        public PlayerBrushApplyMessage(Vector2i pos, ushort type, ushort data, bool inAdminMode)
        {
            Position = pos;
            Type = type;
            Data = data;
            InAdminMode = inAdminMode;
        }
    }
}

