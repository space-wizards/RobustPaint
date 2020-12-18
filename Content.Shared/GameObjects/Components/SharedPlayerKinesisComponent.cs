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

namespace Content.Shared.GameObjects.Components
{
    /// <summary>
    ///     Player controls!
    /// </summary>
    public abstract class SharedPlayerKinesisComponent : Component
    {
        public override string Name => "PlayerKinesis";
        public override uint? NetID => ContentNetIDs.PLAYER_KINESIS;

        public Vector2 Velocity { get; set; } = new Vector2(0, 0);

        /// <inheritdoc />
        public override ComponentState GetComponentState()
        {
            return new PlayerKinesisComponentState(Velocity);
        }

        /// <inheritdoc />
        public override void HandleComponentState(ComponentState curState, ComponentState nextState)
        {
            Velocity = ((PlayerKinesisComponentState) nextState).Velocity;

            Dirty();
        }

        /// <inheritdoc />
        public override void HandleNetworkMessage(ComponentMessage message, INetChannel netChannel, ICommonSession session) {
            base.HandleNetworkMessage(message, netChannel, session);
            if (session == null)
                return;
            if (session.AttachedEntity != Owner)
                return;
            if (message is PlayerKinesisUpdateMessage)
                Velocity = ((PlayerKinesisUpdateMessage) message).Velocity;
        }
    }
﻿
    [Serializable, NetSerializable]
    public class PlayerKinesisComponentState : ComponentState
    {
        public readonly Vector2 Velocity;
        public PlayerKinesisComponentState(Vector2 vel) : base(ContentNetIDs.PLAYER_KINESIS)
        {
            Velocity = vel;
        }
    }

    [Serializable, NetSerializable]
    public class PlayerKinesisUpdateMessage : ComponentMessage
    {
        public readonly Vector2 Velocity;
        public PlayerKinesisUpdateMessage(Vector2 vel)
        {
            Velocity = vel;
        }
    }
}

