using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;

namespace Content.Shared.GameObjects.Components
{
    /// <summary>
    ///     This player is an admin!
    /// </summary>
    [RegisterComponent]
    [NetworkedComponent]
    public class PlayerAdminComponent : Component
    {
        public override string Name => "PlayerAdmin";
    }
 }

