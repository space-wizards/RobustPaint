using Content.Shared.GameObjects.Components;
using JetBrains.Annotations;
using Robust.Shared.GameObjects.Systems;

namespace Content.Shared.GameObjects.EntitySystems
{
    [UsedImplicitly]
    public class PlayerKinesisSystem : EntitySystem
    {
        public override void Update(float frameTime)
        {
            base.Update(frameTime);

            foreach (var kinesis in ComponentManager.EntityQuery<SharedPlayerKinesisComponent>())
            {
                kinesis.Owner.Transform.LocalPosition += (kinesis.Velocity * frameTime);
            }
        }
    }
}
