using Content.Client.GameObjects.Components;
using JetBrains.Annotations;
using Robust.Shared.GameObjects.Systems;

namespace Content.Client.GameObjects.EntitySystems
{
    [UsedImplicitly]
    public class PlayerKinesisSystem : EntitySystem
    {
        public override void Update(float frameTime)
        {
            base.Update(frameTime);

            foreach (var brush in ComponentManager.EntityQuery<PlayerBrushComponent>())
            {
                brush.Update();
            }
        }
    }
}
