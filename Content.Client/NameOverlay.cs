using Content.Shared;
﻿using Robust.Client.Graphics.ClientEye;
using Robust.Client.Graphics.Drawing;
using Robust.Client.Graphics.Overlays;
using Robust.Client.Interfaces.Graphics;
using Robust.Client.Interfaces.Graphics.ClientEye;
using Robust.Shared.Interfaces.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.GameObjects.Systems;
using Robust.Shared.Interfaces.Map;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client
{
    public class NameOverlay : Overlay
    {
        [Dependency] private readonly IEntityManager _entityManager = default!;
        [Dependency] private readonly IEyeManager _eyeManager = default!;
        [Dependency] private readonly IClyde _clyde = default!;
        [Dependency] private readonly StyleSheetManager _styleSheetManager = default!;

        public override OverlaySpace Space => OverlaySpace.ScreenSpace;

        public NameOverlay() : base(nameof(NameOverlay))
        {
            IoCManager.InjectDependencies(this);
        }

        protected override void Draw(DrawingHandleBase handle, OverlaySpace overlay)
        {
            var drawHandle = (DrawingHandleScreen) handle;

            var mapId = _eyeManager.CurrentMap;
            var eye = _eyeManager.CurrentEye;

            var worldBounds = Box2.CenteredAround(eye.Position.Position,
                _clyde.ScreenSize / (float) EyeManager.PixelsPerMeter * eye.Zoom);

            var font = _styleSheetManager.FontUsername;
            var ascent = font.GetAscent(1.0f);
            var margin = new Vector2(2.0f, 2.0f);
            var hover = 32.0f;

            foreach (var entity in _entityManager.GetEntitiesIntersecting(mapId, worldBounds))
            {
                var name = entity.Name;
                if (name == "")
                    continue;
                var screenCoords = _eyeManager.CoordinatesToScreen(entity.Transform.Coordinates);

                var basePos = screenCoords.Position;

                var totalWidth = 0.0f;
                var totalHeight = font.GetHeight(1.0f);
                foreach (var chr in name)
                {
                    var metric = font.GetCharMetrics(chr, 1.0f);
                    if (metric != null)
                        totalWidth += metric.Value.Advance;
                }

                var tlPos = basePos - new Vector2(totalWidth / 2, totalHeight + hover);
                drawHandle.DrawRect(UIBox2.FromDimensions(tlPos - margin, new Vector2(totalWidth, totalHeight) + (margin * 2)), Color.Black);

                // for actual render, use ascent baseline
                tlPos += new Vector2(0.0f, ascent);
                foreach (var chr in name)
                {
                    tlPos += new Vector2(font.DrawChar(drawHandle, chr, tlPos, 1.0f, Color.White), 0.0f);
                }
            }
        }
    }
}
