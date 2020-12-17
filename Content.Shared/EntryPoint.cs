using System;
using System.Collections.Generic;
using System.Globalization;
using Robust.Shared.ContentPack;
using Robust.Shared.Interfaces.Map;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Localization.Macros;
using Robust.Shared.Prototypes;

namespace Content.Shared
{
    public class EntryPoint: GameShared
    {
        // See line 25. Controls the default game culture and language.
        // Robust calls this culture, but you might find it more fitting to call it the game
        // language. Robust doesn't support changing this mid-game. Load your config file early
        // if you want that.
        private const string Culture = "en-US";

        [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
        [Dependency] private readonly ITileDefinitionManager _tileDefinitionManager = default!;

        public override void PreInit()
        {
            IoCManager.InjectDependencies(this);
            var textMacroFactory = IoCManager.Resolve<ITextMacroFactory>();
            textMacroFactory.DoAutoRegistrations();

            // Default to en-US.
            // DEVNOTE: If you want your game to be multiregional at runtime, you'll need to 
            // do something more complicated here.
            Loc.LoadCulture(new CultureInfo(Culture));
            // TODO: Document what else you might want to put here
        }

        public override void Init()
        {
            // TODO: Document what you put here
        }

        public override void PostInit()
        {
            base.PostInit();

            // Init tiles!
            foreach (var tileDef in _prototypeManager.EnumeratePrototypes<ContentTileDefinition>())
                _tileDefinitionManager.Register(tileDef);

            _tileDefinitionManager.Initialize();
        }
    }
}
