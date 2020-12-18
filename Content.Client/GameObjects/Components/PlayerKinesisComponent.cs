using Content.Shared.GameObjects.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.Maths;
using Robust.Shared.Input.Binding;
using Robust.Client.Interfaces.Input;
using Robust.Client.Player;
using Robust.Shared.IoC;

namespace Content.Client.GameObjects.Components
{
    /// <summary>
    ///     Player controls!
    ///     On the client, this component exists to serve as prediction fodder.
    /// </summary>
    [RegisterComponent]
    [ComponentReference(typeof(SharedPlayerKinesisComponent))]
    public class PlayerKinesisComponent : SharedPlayerKinesisComponent
    {
        [Dependency] private IInputManager _inputManager = default!;
        [Dependency] private IPlayerManager _playerManager = default!;

        private bool _left = false;
        private bool _right = false;
        private bool _up = false;
        private bool _down = false;

        public override void Initialize()
        {
            base.Initialize();
            _inputManager.KeyBindStateChanged += KeyBindStateChanged;
        }

        protected override void Shutdown()
        {
            _inputManager.KeyBindStateChanged -= KeyBindStateChanged;
            base.Shutdown();
        }

        public override void HandleComponentState(ComponentState curState, ComponentState nextState)
        {
            // Only handle velocity updates provided for other players.
            // This is because otherwise the prediction system has a nasty habit of getting "jerky".
            if (_playerManager.LocalPlayer?.ControlledEntity == Owner)
                return;
            base.HandleComponentState(curState, nextState);
        }

        private void KeyBindStateChanged(BoundKeyEventArgs ev)
        {
            if (_playerManager.LocalPlayer?.ControlledEntity != Owner)
                return;
            bool val = ev.State == BoundKeyState.Down;
            bool handled = false;
            if (ev.Function == EngineKeyFunctions.MoveLeft)
            {
                _left = val;
                handled = true;
            }
            else if (ev.Function == EngineKeyFunctions.MoveRight)
            {
                _right = val;
                handled = true;
            }
            else if (ev.Function == EngineKeyFunctions.MoveUp)
            {
                _up = val;
                handled = true;
            }
            else if (ev.Function == EngineKeyFunctions.MoveDown)
            {
                _down = val;
                handled = true;
            }
            if (handled)
            {
                ev.Handle();
                RecalculateVelocityAndTransmit();
            }
        }

        private void RecalculateVelocityAndTransmit()
        {
            Vector2 vel = new Vector2(0, 0);
            if (_left)
                vel += new Vector2(-1, 0);
            if (_right)
                vel += new Vector2(1, 0);
            if (_up)
                vel += new Vector2(0, 1);
            if (_down)
                vel += new Vector2(0, -1);
            vel *= 16;
            SendNetworkMessage(new PlayerKinesisUpdateMessage(vel));
        }
    }
}

