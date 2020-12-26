using Content.Shared.Input;
﻿using Content.Shared.GameObjects.Components;
using Content.Shared.GameObjects.EntitySystems;
using JetBrains.Annotations;
using Robust.Shared.GameObjects.Systems;
using Robust.Shared.Input;
using Robust.Shared.Log;
using Robust.Shared.Input.Binding;
using Robust.Shared.Players;

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
