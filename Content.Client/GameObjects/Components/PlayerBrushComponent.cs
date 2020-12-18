using System;
using Content.Shared.GameObjects.Components;
using Content.Shared.Input;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.Maths;
using Robust.Shared.Map;
using Robust.Shared.Log;
using Robust.Shared.Input.Binding;
using Robust.Shared.Interfaces.GameObjects;
using Robust.Shared.Interfaces.Map;
using Robust.Shared.IoC;
using Robust.Client.Interfaces.Input;
using Robust.Client.Interfaces.Graphics.ClientEye;
using Robust.Client.Player;

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
        [Dependency] private IEyeManager _eyeManager = default!;
        [Dependency] private ITileDefinitionManager _tileDefinitionManager = default!;

        private bool _down = false;
        private Vector2i _lastDrawn = new Vector2i(-8192, -8192);

        public ushort Colour { get; set; } = 2;

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
                if (!val)
                    _lastDrawn = new Vector2i(-8192, -8192);
                // Logger.WarningS("c.c.go.co.brush", "!");
                handled = true;
            }
            else if (ev.Function == ContentKeyFunctions.RP8NTNextColour)
            {
                if (val)
                {
                    if (Colour >= _tileDefinitionManager.Count - 1)
                    {
                        Colour = 0;
                    }
                    else
                    {
                        Colour++;
                    }
                }
                handled = true;
            }
            else if (ev.Function == ContentKeyFunctions.RP8NTPrevColour)
            {
                if (val)
                {
                    if (Colour <= 0)
                    {
                        Colour = (ushort) (_tileDefinitionManager.Count - 1);
                    }
                    else
                    {
                        Colour--;
                    }
                }
                handled = true;
            }
            if (handled)
            {
                ev.Handle();
            }
        }

        public void Update()
        {
            if (!_down)
                return;

            var (x, y) = _eyeManager.ScreenToMap(_inputManager.MouseScreenPosition);
            var pos = new Vector2i((int) Math.Floor(x), (int) Math.Floor(y));
            if (_lastDrawn != pos) {
                SendNetworkMessage(new PlayerBrushApplyMessage(pos, Colour));
                _lastDrawn = pos;
            }
        }
    }
}

