using JetBrains.Annotations;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using YamlDotNet.RepresentationModel;

namespace Content.Shared
{
    [UsedImplicitly]
    [Prototype("tile")]
    public sealed class ContentTileDefinition : IPrototype, ITileDefinition
    {
        string IPrototype.ID => Name;

        [DataField("name")]
        public string Name { get; private set; }
        public ushort TileId { get; private set; }
        [DataField("display_name")]
        public string DisplayName { get; private set; }
        public string SpriteName => Name;
        public float Friction => 0;
        [DataField("rotation")]
        public int Rotation { get; private set; }

        public string Path => "/Textures/Constructible/Tiles/";

        public void AssignTileId(ushort id)
        {
            TileId = id;
        }
    }
}
