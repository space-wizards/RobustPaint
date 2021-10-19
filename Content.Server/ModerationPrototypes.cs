using JetBrains.Annotations;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server
{
    [UsedImplicitly]
    [Prototype("moderation")]
    public sealed class ModerationDefinition : IPrototype
    {
        string IPrototype.ID => UUID;

        [DataField("uuid")]
        public string UUID { get; set; }
        [DataField("entity")]
        public string Entity { get; set; }
        [DataField("text")]
        public string Text { get; set; }
    }

    [UsedImplicitly]
    [Prototype("message")]
    public sealed class MessageDefinition : IPrototype
    {
        [DataField("text")]
        public string Text { get; private set; }
        [DataField("id")]
        public string ID { get; private set; }
    }
}

