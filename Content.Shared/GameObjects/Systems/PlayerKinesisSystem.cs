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
                var localPosition = kinesis.Owner.Transform.LocalPosition;
                localPosition += (kinesis.Velocity * frameTime);
                localPosition = kinesis.TravelBounds.ClosestPoint(localPosition);
                kinesis.Owner.Transform.LocalPosition = localPosition;
            }
        }
    }
}
