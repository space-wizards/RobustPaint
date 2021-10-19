using Robust.Client.GameObjects;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Timing;

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

        private void OnKeyBindStateChanged(ViewportBoundKeyEventArgs viewportArgs)
        {
            var args = viewportArgs.KeyEventArgs;
            if (!_entitySystemManager.TryGetEntitySystem<InputSystem>(out var inputSystem))
                return;
            var message = new FullInputCmdMessage(_gameTiming.CurTick, _gameTiming.TickFraction, _inputManager.NetworkBindMap.KeyFunctionID(args.Function), args.State, EntityCoordinates.Invalid, args.PointerLocation, EntityUid.Invalid);
            if (inputSystem.HandleInputCommand(_playerManager.LocalPlayer.Session, args.Function, message))
                args.Handle();
        }
    }
}

