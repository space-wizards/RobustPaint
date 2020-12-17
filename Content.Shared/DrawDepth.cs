using Robust.Shared.Serialization;
using DrawDepthTag = Robust.Shared.GameObjects.DrawDepth;

namespace Content.Shared.GameObjects
{
    [ConstantsFor(typeof(DrawDepthTag))]
    public enum DrawDepth
    {
        Objects = DrawDepthTag.Default
    }
}
