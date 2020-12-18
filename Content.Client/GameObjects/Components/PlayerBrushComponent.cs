using Content.Shared.GameObjects.Components;
using Content.Shared.Input;
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
    ///     On the client, this component determines when to place paint and where (yes, so it's always client that is trusted to determine paint placement, shush)
    /// </summary>
    [RegisterComponent]
    [ComponentReference(typeof(SharedPlayerBrushComponent))]
    public class PlayerBrushComponent : SharedPlayerBrushComponent
    {
        [Dependency] private IInputManager _inputManager = default!;
        [Dependency] private IPlayerManager _playerManager = default!;

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
            if (ev.Function == ContentKeyFunctions.RP8NTPlace)
            {
                _down = val;
                handled = true;
            }
            if (handled)
            {
                ev.Handle();
            }
        }

        public void Update()
        {
            // TODO: Stuff!
            //SendNetworkMessage(new PlayerKinesisUpdateMessage(vel));
        }
    }
}

