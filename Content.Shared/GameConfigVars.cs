using Robust.Shared;
using Robust.Shared.Configuration;

namespace Content.Shared
{
    // DEVNOTE: This is the same as SS14's CCVars. Except it's not named CCVars as that name is 
    // hot garbage.
    [CVarDefs]
    public sealed class GameConfigVars : CVars
    {
        public static readonly CVarDef<string>
            MapFile = CVarDef.Create("game.savefile", "world.yml", CVar.SERVERONLY | CVar.ARCHIVE);

        public static readonly CVarDef<int>
            SaveInterval = CVarDef.Create("game.saveinterval", 60, CVar.SERVERONLY | CVar.ARCHIVE);
    }
}
