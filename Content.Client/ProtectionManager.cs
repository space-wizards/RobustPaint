using System;
using Robust.Client.Interfaces.Graphics.Overlays;
using Robust.Shared.IoC;

namespace Content.Client
{
    public class ProtectionManager
    {
        [Dependency] private readonly IOverlayManager _overlayManager = default!;
        [Dependency] private readonly IPlayerManager _playerManager = default!;

        private bool _inAdminMode = false;
        public bool InAdminMode
        {
            get => _inAdminMode;
            set
            {
                _inAdminMode = value;
                if (value)
                {
                    if (!_overlayManager.HasOverlay(nameof(ProtectionOverlay)))
                        _overlayManager.AddOverlay(new ProtectionOverlay());
                }
                else
                {
                    if (_overlayManager.HasOverlay(nameof(ProtectionOverlay)))
                        _overlayManager.RemoveOverlay(nameof(ProtectionOverlay));
                }
            }
        }
        public ushort Flags { get; set; } = 0;

        public bool LocalPlayerIsAdmin
        {
            get
            {
                var controlledEntity = _playerManager.LocalPlayer?.ControlledEntity;
                if (controlledEntity == null)
                    return false;
                return controlledEntity.HasComponent<PlayerAdminComponent>();
            }
        }
    }
}
