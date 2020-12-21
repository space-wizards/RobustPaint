using Robust.Shared.IoC;
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
        [Dependency] private IngressExperienceManager _ingressExperienceManager = default!;

        public override void Initialize()
        {
            base.Initialize();
            TravelBounds = _ingressExperienceManager.MapExtentF;
            Dirty();
        }
    }
}

