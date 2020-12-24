using Content.Shared.Input;
using Robust.Client.Interfaces;
using Robust.Client.Interfaces.Input;
using Robust.Client.Interfaces.Graphics.Lighting;
using Robust.Client.Interfaces.UserInterface;
using Robust.Client.Interfaces.ResourceManagement;
using Robust.Client.ResourceManagement;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Shared.ContentPack;
using Robust.Shared.Interfaces.Configuration;
using Robust.Shared.Interfaces.GameObjects;
using Robust.Shared.Interfaces.Map;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client
{
    public class StyleSheetManager
    {
        [Dependency] private readonly IUserInterfaceManager _userInterfaceManager = default!;
        [Dependency] private readonly IResourceCache _resourceCache = default!;

        public void Initialize()
        {
            _userInterfaceManager.Stylesheet = new Stylesheet(new[]
            {
                new StyleRule(
                    new SelectorElement(null, null, null, null),
                    new[]
                    {
                        new StyleProperty("font", new VectorFont(_resourceCache.GetResource<FontResource>(new ResourcePath("/Fonts/DigitalWristwatches.ttf")), 12)),
                    }
                ),
            });
        }
    }
}
