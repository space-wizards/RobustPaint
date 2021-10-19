using Content.Client.UserInterface;
using Content.Shared.Input;
using Robust.Client;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Shared.ContentPack;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Client
{
    public class EntryPoint: GameClient
    {
        [Dependency] private readonly IBaseClient _baseClient = default!;
        [Dependency] private readonly IGameController _gameController = default!;
        [Dependency] private readonly ILightManager _lightManager = default!;
        [Dependency] private readonly IInputManager _inputManager = default!;
        [Dependency] private readonly IOverlayManager _overlayManager = default!;
        [Dependency] private readonly UIManager _uiManager = default!;
        [Dependency] private readonly StyleSheetManager _styleSheetManager = default!;
        [Dependency] private readonly InputHookupManager _inputHookupManager = default!;

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
            var human = _inputManager.Contexts.GetContext("human");
            human.AddFunction(ContentKeyFunctions.RP8NTNextColour);
            human.AddFunction(ContentKeyFunctions.RP8NTPrevColour);
            human.AddFunction(ContentKeyFunctions.RP8NTPlace);
            human.AddFunction(ContentKeyFunctions.RP8NTSprint);

            _uiManager.Initialize();
            _styleSheetManager.Initialize();
            _inputHookupManager.Initialize();

            _overlayManager.AddOverlay(new NameOverlay());

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
