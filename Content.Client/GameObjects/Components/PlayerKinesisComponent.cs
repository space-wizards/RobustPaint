using Content.Shared.Input;
using Content.Shared.GameObjects.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.Maths;
using Robust.Shared.Input.Binding;
using Robust.Client.Interfaces.Input;
using Robust.Client.Player;
using Robust.Shared.IoC;

namespace Content.Client.GameObjects.Components
{
    /// <summary>
    ///     Player controls!
    ///     On the client, this component exists to serve as prediction fodder.
    /// </summary>
    [RegisterComponent]
    [ComponentReference(typeof(SharedPlayerKinesisComponent))]
    public class PlayerKinesisComponent : SharedPlayerKinesisComponent
    {
    }
}

