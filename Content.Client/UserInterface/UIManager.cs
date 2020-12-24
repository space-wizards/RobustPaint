using Content.Client;
using Content.Shared.Network;
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
using Robust.Shared.Interfaces.Network;
using Robust.Shared.Map;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using Robust.Shared.Maths;

namespace Content.Client.UserInterface
{
    public class UIManager
    {
        [Dependency] private IClientNetManager _netManager = default!;
        [Dependency] private IUserInterfaceManager _userInterfaceManager = default!;
        [Dependency] private IResourceCache _resourceCache = default!;
        [Dependency] private ITileDefinitionManager _tileDefinitionManager = default!;

        private TextureRect _test = default!;

        public void Initialize()
        {
            var hud = new HUD();
            LayoutContainer.SetAnchorPreset(hud, LayoutContainer.LayoutPreset.Wide);
            LayoutContainer.SetGrowHorizontal(hud, LayoutContainer.GrowDirection.Both);
            LayoutContainer.SetGrowVertical(hud, LayoutContainer.GrowDirection.Both);
            _userInterfaceManager.StateRoot.AddChild(hud);
            _test = hud.TileAccessible;

            Update(2);
            _netManager.RegisterNetMessage<MsgShowMessage>(nameof(MsgShowMessage), message => ViewMessage(message.Text));
        }

        public void ViewMessage(string text)
        {
            var msg = new MessageDialog(text);
            LayoutContainer.SetAnchorPreset(msg, LayoutContainer.LayoutPreset.Wide);
            LayoutContainer.SetGrowHorizontal(msg, LayoutContainer.GrowDirection.Both);
            LayoutContainer.SetGrowVertical(msg, LayoutContainer.GrowDirection.Both);
            _userInterfaceManager.StateRoot.AddChild(msg);
        }

        public void Update(ushort colour)
        {
            var entry = _tileDefinitionManager[colour];
            _test.Texture = _resourceCache.GetResource<TextureResource>($"/Textures/Constructible/Tiles/{entry.SpriteName}.png");
        }
    }
}
