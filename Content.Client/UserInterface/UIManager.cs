using System;
using System.Collections.Generic;
using Content.Client;
using Content.Shared.Network;
using Robust.Client;
using Robust.Client.Interfaces;
using Robust.Client.Interfaces.UserInterface;
using Robust.Client.Interfaces.ResourceManagement;
using Robust.Client.UserInterface;
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
        private List<IModal> _modals = new List<IModal>();
        private IModal _currentModal = null;

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
            _netManager.ConnectFailed += (a, b) => {
                ViewMessage("Failed to connect.");
            };
            _netManager.Disconnect += (a, b) => {
                ViewMessage("Disconnected:\n" + (b.Reason ?? "(no reason)"));
            };
        }

        public void ViewMessage(string text)
        {
            PushModal(new MessageDialog(text));
        }

        public void PushModal(IModal modal)
        {
            _modals.Add(modal);
            if (_currentModal == null)
                NextModal();
        }

        private void NextModal()
        {
            if (_modals.Count > 0)
            {
                var modal = _modals[0];
                _modals.RemoveAt(0);
                modal.OnModalClosed += (a, b) => {
                    if (_currentModal == modal)
                    {
                        _currentModal = null;
                        NextModal();
                    }
                };
                _currentModal = modal;
                LayoutContainer.SetAnchorPreset((Control) modal, LayoutContainer.LayoutPreset.Wide);
                LayoutContainer.SetGrowHorizontal((Control) modal, LayoutContainer.GrowDirection.Both);
                LayoutContainer.SetGrowVertical((Control) modal, LayoutContainer.GrowDirection.Both);
                _userInterfaceManager.StateRoot.AddChild((Control) modal);
            }
        }

        public void Update(ushort colour)
        {
            var entry = _tileDefinitionManager[colour];
            _test.Texture = _resourceCache.GetResource<TextureResource>($"/Textures/Constructible/Tiles/{entry.SpriteName}.png");
        }
    }

    public interface IModal
    {
        public event EventHandler OnModalClosed;
    }
}
