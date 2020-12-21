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
    ///     Player controls!
    /// </summary>
    public abstract class SharedPlayerKinesisComponent : Component
    {
        public override string Name => "PlayerKinesis";
        public override uint? NetID => ContentNetIDs.PLAYER_KINESIS;

        public Vector2 Velocity { get; set; } = new Vector2(0, 0);
        public Box2 TravelBounds { get; set; } = new Box2();

        protected virtual bool _shouldHandleVelocity => true;

        /// <inheritdoc />
        public override ComponentState GetComponentState()
        {
            return new PlayerKinesisComponentState(Velocity, TravelBounds);
        }

        /// <inheritdoc />
        public override void HandleComponentState(ComponentState curState, ComponentState nextState)
        {
            base.HandleComponentState(curState, nextState);
            if (nextState == null)
                return;
            if (_shouldHandleVelocity)
                Velocity = ((PlayerKinesisComponentState) nextState).Velocity;
            TravelBounds = ((PlayerKinesisComponentState) nextState).TravelBounds;
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
            {
                Velocity = ((PlayerKinesisUpdateMessage) message).Velocity;
                Dirty();
            }
        }
    }
﻿
    [Serializable, NetSerializable]
    public class PlayerKinesisComponentState : ComponentState
    {
        public readonly Vector2 Velocity;
        public readonly Box2 TravelBounds;
        public PlayerKinesisComponentState(Vector2 vel, Box2 travelBounds) : base(ContentNetIDs.PLAYER_KINESIS)
        {
            Velocity = vel;
            TravelBounds = travelBounds;
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

