using Robust.Client.ResourceManagement;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.IoC;

namespace Content.Client
{
    public class StyleSheetManager
    {
        [Dependency] private readonly IUserInterfaceManager _userInterfaceManager = default!;
        [Dependency] private readonly IResourceCache _resourceCache = default!;

        public VectorFont FontUsername { get; set; } = default!;

        public void Initialize()
        {
            var fontRes = _resourceCache.GetResource<FontResource>("/Fonts/DigitalWristwatches.ttf");
            var font = new VectorFont(fontRes, 16);
            FontUsername = new VectorFont(fontRes, 8);

            var panelTex = _resourceCache.GetResource<TextureResource>("/Textures/panel.png");
            var panel = new StyleBoxTexture { Texture = panelTex };
            panel.SetPatchMargin(StyleBox.Margin.All, 2);
            panel.SetExpandMargin(StyleBox.Margin.All, 2);

            var panelDarkTex = _resourceCache.GetResource<TextureResource>("/Textures/panelDark.png");
            var panelDark = new StyleBoxTexture { Texture = panelDarkTex };
            panelDark.SetPatchMargin(StyleBox.Margin.All, 2);
            panelDark.SetExpandMargin(StyleBox.Margin.All, 2);

            _userInterfaceManager.Stylesheet = new Stylesheet(new[]
            {
                new StyleRule(
                    new SelectorElement(null, null, null, null),
                    new[]
                    {
                        new StyleProperty("font", font),
                        new StyleProperty(PanelContainer.StylePropertyPanel, panel),
                        new StyleProperty(LineEdit.StylePropertyStyleBox, panelDark)
                    }
                )
            });
        }
    }
}
