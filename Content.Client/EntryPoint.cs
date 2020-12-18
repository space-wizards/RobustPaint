using Content.Shared.Input;
using Robust.Client.Interfaces;
using Robust.Client.Interfaces.Input;
using Robust.Client.Interfaces.Graphics.Lighting;
using Robust.Shared.ContentPack;
using Robust.Shared.Interfaces.Configuration;
using Robust.Shared.Interfaces.GameObjects;
using Robust.Shared.Interfaces.Map;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Client
{
    public class EntryPoint: GameClient
    {
        [Dependency] private readonly IBaseClient _baseClient = default!;
        [Dependency] private readonly IGameController _gameController = default!;
        [Dependency] private readonly ILightManager _lightManager = default!;
        // [Dependency] private readonly IInputManager _inputManager = default!;

        public override void Init()
        {
            var factory = IoCManager.Resolve<IComponentFactory>();
            var prototypes = IoCManager.Resolve<IPrototypeManager>();

            factory.DoAutoRegistrations();

            foreach (var ignoreName in IgnoredComponents.List)
            {
                factory.RegisterIgnore(ignoreName);
            }

            foreach (var ignoreName in IgnoredPrototypes.List)
            {
                prototypes.RegisterIgnore(ignoreName);
            }

            ClientContentIoC.Register();

            IoCManager.BuildGraph();

            // DEVNOTE: This is generally where you'll be setting up the IoCManager further.

            IoCManager.InjectDependencies(this);
        }

        public override void PostInit()
        {
            base.PostInit();

            // *whistles innocently*
            _lightManager.Enabled = false;

            // >:D
            if (_gameController.LaunchState.FromLauncher)
            {
                _baseClient.ConnectToServer(_gameController.LaunchState.ConnectEndpoint);
            }
            else
            {
                _baseClient.ConnectToServer("localhost", 1212);
            }
        }

        public override void Update(ModUpdateLevel level, FrameEventArgs frameEventArgs)
        {
            base.Update(level, frameEventArgs);
            // DEVNOTE: Game update loop goes here. Usually you'll want some independent GameTicker.
        }
    }

    
}
