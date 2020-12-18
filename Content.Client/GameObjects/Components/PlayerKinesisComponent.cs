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
            Velocity = new Vector2(0, 0);
            if (_left)
                Velocity += new Vector2(-1, 0);
            if (_right)
                Velocity += new Vector2(1, 0);
            if (_up)
                Velocity += new Vector2(0, 1);
            if (_down)
                Velocity += new Vector2(0, -1);
            Velocity *= 16;
            SendNetworkMessage(new PlayerKinesisUpdateMessage(Velocity));
        }
    }
}

