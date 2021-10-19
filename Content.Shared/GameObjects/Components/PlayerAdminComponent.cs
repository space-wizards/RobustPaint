using Robust.Shared.GameObjects;

namespace Content.Shared.GameObjects.Components
{
    /// <summary>
    ///     This player is an admin!
    /// </summary>
    [RegisterComponent]
    public class PlayerAdminComponent : Component
    {
        public override string Name => "PlayerAdmin";
    }
 }

