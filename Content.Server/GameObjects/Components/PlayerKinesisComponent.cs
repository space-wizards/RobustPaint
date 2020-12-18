using Robust.Shared.GameObjects;
using Content.Shared.GameObjects.Components;

namespace Content.Server.GameObjects.Components
{
    /// <summary>
    ///     Player controls!
    ///     On the server, this component exists to move the entity and send back prediction information.
    /// </summary>
    [RegisterComponent]
    [ComponentReference(typeof(SharedPlayerKinesisComponent))]
    public class PlayerKinesisComponent : SharedPlayerKinesisComponent
    {
    }
}

