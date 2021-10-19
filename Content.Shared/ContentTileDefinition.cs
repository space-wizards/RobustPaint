﻿using JetBrains.Annotations;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using YamlDotNet.RepresentationModel;

namespace Content.Shared
{
    [UsedImplicitly]
    [Prototype("tile")]
    public sealed class ContentTileDefinition : IPrototype, ITileDefinition
    {
        string IPrototype.ID => Name;

        public string Name { get; private set; }
        public ushort TileId { get; private set; }
        public string DisplayName { get; private set; }
        public string SpriteName => Name;
        public float Friction => 0;
        public int Rotation { get; private set; }

        public string Path => "/Textures/Tiles/";

        public void AssignTileId(ushort id)
        {
            TileId = id;
        }

        public void LoadFrom(YamlMappingNode mapping)
        {
            Name = mapping.GetNode("name").ToString();
            DisplayName = mapping.GetNode("display_name").ToString();
            Rotation = int.Parse(mapping.GetNode("rotation").ToString());
        }

    }
}
