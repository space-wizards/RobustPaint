using Content.Shared.Input;
using Robust.Client.Interfaces;
using Robust.Client.Interfaces.Graphics.ClientEye;
using Robust.Client.Interfaces.Input;
using Robust.Client.Player;
using Robust.Client.GameObjects.EntitySystems;
using Robust.Shared.ContentPack;
using Robust.Shared.Interfaces.Configuration;
using Robust.Shared.Interfaces.GameObjects;
using Robust.Shared.Interfaces.Map;
using Robust.Shared.Interfaces.Timing;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client
{
    public class InputHookupManager
    {
        [Dependency] protected readonly IInputManager _inputManager = default!;
        [Dependency] protected readonly IEntityManager _entityManager = default!;
        [Dependency] protected readonly IEntitySystemManager _entitySystemManager = default!;
        [Dependency] protected readonly IPlayerManager _playerManager = default!;
        [Dependency] protected readonly IGameTiming _gameTiming = default!;

        public void Initialize()
        {
            _inputManager.KeyBindStateChanged += OnKeyBindStateChanged;
        }

        private void OnKeyBindStateChanged(BoundKeyEventArgs args)
        {
            if (!_entitySystemManager.TryGetEntitySystem<InputSystem>(out var inputSystem))
                return;
            var message = new FullInputCmdMessage(_gameTiming.CurTick, _gameTiming.TickFraction, _inputManager.NetworkBindMap.KeyFunctionID(args.Function), args.State, EntityCoordinates.Invalid, args.PointerLocation, EntityUid.Invalid);
            if (inputSystem.HandleInputCommand(_playerManager.LocalPlayer.Session, args.Function, message))
                args.Handle();
        }
    }
}

