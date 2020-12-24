using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Robust.Shared.Interfaces.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using YamlDotNet.RepresentationModel;

namespace Content.Server
{
    [UsedImplicitly]
    [Prototype("moderation")]
    public sealed class ModerationDefinition : IPrototype, IIndexedPrototype
    {
        string IIndexedPrototype.ID => UUID;

        public string UUID { get; private set; }
        public string Entity { get; private set; }
        public string Text { get; private set; }

        public void LoadFrom(YamlMappingNode mapping)
        {
            UUID = mapping.GetNode("uuid").ToString();
            Entity = mapping.GetNode("entity").ToString();
            Text = mapping.GetNode("text").ToString();
        }
    }

    [UsedImplicitly]
    [Prototype("message")]
    public sealed class MessageDefinition : IPrototype
    {
        public string Text { get; private set; }

        public void LoadFrom(YamlMappingNode mapping)
        {
            Text = mapping.GetNode("text").ToString();
        }
    }
}

