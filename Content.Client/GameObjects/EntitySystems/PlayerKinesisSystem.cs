using Content.Shared.GameObjects.EntitySystems;
using JetBrains.Annotations;

namespace Content.Client.GameObjects.EntitySystems
{
    [UsedImplicitly]
    public class PlayerKinesisSystem : SharedPlayerKinesisSystem
    {
        public override void FrameUpdate(float frameTime)
        {
            base.FrameUpdate(frameTime);
            Update(frameTime);
        }
    }
}
