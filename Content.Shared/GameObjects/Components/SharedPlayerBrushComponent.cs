using System;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;

namespace Content.Shared.GameObjects.Components
{
    /// <summary>
    ///     Player controls!
    /// </summary>
    [NetworkedComponent]
    public abstract class SharedPlayerBrushComponent : Component
    {
        public override string Name => "PlayerBrush";
    }
    
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

