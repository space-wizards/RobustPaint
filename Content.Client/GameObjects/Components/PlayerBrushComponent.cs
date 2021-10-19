using System;
using Content.Client.UserInterface;
using Content.Shared;
using Content.Shared.GameObjects.Components;
using Content.Shared.Input;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.Maths;
using Robust.Shared.Map;
using Robust.Shared.IoC;
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
        [Dependency] private readonly IInputManager _inputManager = default!;
        [Dependency] private readonly IPlayerManager _playerManager = default!;
        [Dependency] private readonly IEyeManager _eyeManager = default!;
        [Dependency] private readonly ITileDefinitionManager _tileDefinitionManager = default!;
        [Dependency] private readonly UIManager _uiManager = default!;
        [Dependency] private readonly ProtectionManager _protectionManager = default!;

        private bool _down = false;
        private Vector2i _lastDrawn = new Vector2i(-8192, -8192);

        public ushort Colour { get; set; } = 2;

        protected override void Initialize()
        {
            base.Initialize();
            _inputManager.KeyBindStateChanged += KeyBindStateChanged;
        }

        protected override void Shutdown()
        {
            _inputManager.KeyBindStateChanged -= KeyBindStateChanged;
            base.Shutdown();
        }

        private void KeyBindStateChanged(ViewportBoundKeyEventArgs vp)
        {
            var ev = vp.KeyEventArgs;
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
                    RotateColour(true);
                }
                handled = true;
            }
            else if (ev.Function == ContentKeyFunctions.RP8NTPrevColour)
            {
                if (val)
                {
                    RotateColour(false);
                }
                handled = true;
            }
            if (handled)
            {
                _uiManager.Update(Colour);
                ev.Handle();
            }
        }

        private void RotateColour(bool next)
        {
            int targetRotation = ((ContentTileDefinition) _tileDefinitionManager[Colour]).Rotation + (next ? 1 : -1);
            int highestRotation = -1;
            int highestRotationIndex = 1;
            int zeroRotationIndex = 1;
            for (var i = 0; i < _tileDefinitionManager.Count; i++)
            {
                int pRot = ((ContentTileDefinition) _tileDefinitionManager[i]).Rotation;
                if ((pRot != -1) && (pRot == targetRotation))
                {
                    Colour = (ushort) i;
                    return;
                }
                if (pRot == 0)
                    zeroRotationIndex = i;
                if (pRot >= highestRotation)
                {
                    highestRotation = pRot;
                    highestRotationIndex = i;
                }
            }
            // if we got here, we couldn't find a tile with the target rotation
            // therefore, we have reached the edge of the rotations
            Colour = (ushort) (next ? zeroRotationIndex : highestRotationIndex);
        }

        public void Update()
        {
            if (!_down)
                return;

            var (x, y) = _eyeManager.ScreenToMap(_inputManager.MouseScreenPosition);
            var pos = new Vector2i((int) Math.Floor(x), (int) Math.Floor(y));
            if (_lastDrawn != pos) {
                SendNetworkMessage(new PlayerBrushApplyMessage(pos, Colour, _protectionManager.Flags, _protectionManager.InAdminMode));
                _lastDrawn = pos;
            }
        }
    }
}

