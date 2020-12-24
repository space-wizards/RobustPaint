using System;
using System.Collections.Generic;
using Content.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Players;
using Robust.Shared.Serialization;
using Robust.Shared.GameObjects;
using Robust.Shared.Interfaces.GameObjects;
using Robust.Shared.Interfaces.Network;
using Robust.Shared.Log;

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
﻿}

