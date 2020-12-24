using Content.Shared;
﻿using Robust.Client.Graphics.ClientEye;
using Robust.Client.Graphics.Drawing;
using Robust.Client.Graphics.Overlays;
using Robust.Client.Interfaces.Graphics;
using Robust.Client.Interfaces.Graphics.ClientEye;
using Robust.Shared.GameObjects.Systems;
using Robust.Shared.Interfaces.Map;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client
{
    public class ProtectionOverlay : Overlay
    {
        [Dependency] private readonly IMapManager _mapManager = default!;
        [Dependency] private readonly IEyeManager _eyeManager = default!;
        [Dependency] private readonly IClyde _clyde = default!;

        public override OverlaySpace Space => OverlaySpace.WorldSpace;

        public ProtectionOverlay() : base(nameof(ProtectionOverlay))
        {
            IoCManager.InjectDependencies(this);
        }

        protected override void Draw(DrawingHandleBase handle, OverlaySpace overlay)
        {
            var drawHandle = (DrawingHandleWorld) handle;

            var mapId = _eyeManager.CurrentMap;
            var eye = _eyeManager.CurrentEye;

            var worldBounds = Box2.CenteredAround(eye.Position.Position,
                _clyde.ScreenSize / (float) EyeManager.PixelsPerMeter * eye.Zoom);

            foreach (var mapGrid in _mapManager.FindGridsIntersecting(mapId, worldBounds))
            {
                var gridBounds = new Box2(mapGrid.WorldToLocal(worldBounds.BottomLeft), mapGrid.WorldToLocal(worldBounds.TopRight));

                foreach (var tile in mapGrid.GetTilesIntersecting(gridBounds, false))
                {
                    if ((tile.Tile.Data & (ushort) TileFlags.Protected) != 0)
                        drawHandle.DrawRect(Box2.FromDimensions(mapGrid.LocalToWorld(new Vector2(tile.X, tile.Y)), new Vector2(0.5f, 0.5f)), Color.Red);
                }
            }
        }
    }
}
