using Robust.Client.UserInterface;
using Robust.Client.Interfaces.UserInterface;
using Robust.Client.Interfaces.ResourceManagement;
using Robust.Client.UserInterface.Controls;
using Robust.Client.ResourceManagement;
using Robust.Shared.Enums;
using Robust.Shared.ContentPack;
using Robust.Shared.Interfaces.GameObjects;
using Robust.Shared.Interfaces.Resources;
using Robust.Shared.Interfaces.Map;
using Robust.Shared.Map;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using Robust.Shared.Maths;

namespace Content.Client
{
    public class ColourSelectorManager
    {
        [Dependency] private IUserInterfaceManager _userInterfaceManager = default!;
        [Dependency] private IResourceCache _resourceCache = default!;
        [Dependency] private ITileDefinitionManager _tileDefinitionManager = default!;

        private TextureRect _test = default!;

        public void Initialize()
        {
            // oh no
            _test = new TextureRect();

            var help = new TextureRect();
            help.Texture = _resourceCache.GetResource<TextureResource>($"/Textures/help.png");

            var box = new HBoxContainer();
            box.AddChild(_test);
            box.AddChild(help);

            _userInterfaceManager.StateRoot.AddChild(box);
            Update(2);
        }

        public void Update(ushort colour)
        {
            var entry = _tileDefinitionManager[colour];
            _test.Texture = _resourceCache.GetResource<TextureResource>($"/Textures/Constructible/Tiles/{entry.SpriteName}.png");
        }
    }
}
