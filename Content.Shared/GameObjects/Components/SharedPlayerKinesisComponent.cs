using System;
using Content.Shared.Input;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.Players;

namespace Content.Shared.GameObjects.Components
{
    /// <summary>
    ///     Player controls!
    /// </summary>
    public abstract class SharedPlayerKinesisComponent : Component
    {
        public override string Name => "PlayerKinesis";

        public byte Controls { get; set; } = 0;
        public Box2 TravelBounds { get; set; } = new Box2();

        public Vector2 Velocity
        {
            get
            {
                Vector2 vel = new Vector2(0, 0);
                if ((Controls & (byte)KinesisKeyFunctionFlags.MoveLeft) != 0)
                    vel += new Vector2(-1, 0);
                if ((Controls & (byte)KinesisKeyFunctionFlags.MoveRight) != 0)
                    vel += new Vector2(1, 0);
                if ((Controls & (byte)KinesisKeyFunctionFlags.MoveUp) != 0)
                    vel += new Vector2(0, 1);
                if ((Controls & (byte)KinesisKeyFunctionFlags.MoveDown) != 0)
                    vel += new Vector2(0, -1);
                vel *= 16;
                if ((Controls & (byte)KinesisKeyFunctionFlags.RP8NTSprint) != 0)
                    vel *= 4;
                return vel;
            }
        }

        /// <inheritdoc />
        public override ComponentState GetComponentState(ICommonSession player)
        {
            return new PlayerKinesisComponentState(Controls, TravelBounds);
        }

        /// <inheritdoc />
        public override void HandleComponentState(ComponentState curState, ComponentState nextState)
        {
            if (curState is PlayerKinesisComponentState state)
            {
                Controls = state.Controls;
                TravelBounds = state.TravelBounds;
            }
        }

        public void KineticMessage(BoundKeyFunction fn, FullInputCmdMessage ev)
        {
            // Logger.WarningS("c.s.go.c.kinesis", "key {0} {1}", fn, ev.State);
            bool val = ev.State == BoundKeyState.Down;
            byte translated = 0;
            if (fn == EngineKeyFunctions.MoveLeft)
                translated = (byte)KinesisKeyFunctionFlags.MoveLeft;
            else if (fn == EngineKeyFunctions.MoveRight)
                translated = (byte)KinesisKeyFunctionFlags.MoveRight;
            else if (fn == EngineKeyFunctions.MoveUp)
                translated = (byte)KinesisKeyFunctionFlags.MoveUp;
            else if (fn == EngineKeyFunctions.MoveDown)
                translated = (byte)KinesisKeyFunctionFlags.MoveDown;
            else if (fn == ContentKeyFunctions.RP8NTSprint)
                translated = (byte)KinesisKeyFunctionFlags.RP8NTSprint;

            if (translated != 0)
            {
                Controls |= translated;
                if (!val)
                    Controls ^= translated;
                Dirty();
            }
        }
    }

    [Serializable, NetSerializable]
    public class PlayerKinesisComponentState : ComponentState
    {
        public readonly byte Controls;
        public readonly Box2 TravelBounds;

        public PlayerKinesisComponentState(byte ctrl, Box2 travelBounds)
        {
            Controls = ctrl;
            TravelBounds = travelBounds;
        }
    }

    [Flags]
    public enum KinesisKeyFunctionFlags : byte
    {
        MoveUp = 1,
        MoveDown = 2,
        MoveLeft = 4,
        MoveRight = 8,
        RP8NTSprint = 16
    }
}