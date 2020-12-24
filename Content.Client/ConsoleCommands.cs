using System;
using Robust.Client.Interfaces.Console;
using Robust.Client.Interfaces.Placement;
using Robust.Client.Interfaces.ResourceManagement;
using Robust.Client.UserInterface.CustomControls;
using Robust.Client.ResourceManagement;
using Robust.Shared.Interfaces.Map;
using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;

namespace Content.Client
{
    class InAdminModeCommand : IConsoleCommand
    {
        public string Command => "admin";
        public string Help => "admin <bool>";
        public string Description => "\"For the honour of...\" Enables admin mode, if you are an admin.";

        public bool Execute(IDebugConsole console, params string[] args)
        {
            var protectionManager = IoCManager.Resolve<ProtectionManager>();
            if (!protectionManager.LocalPlayerIsAdmin)
            {
                console.AddLine("Unauthorized presence detected. Security protocol activated.");
                return true;
            }
            if (args.Length != 1)
            {
                console.AddLine("What is your query?");
                return false;
            }
            if (!bool.TryParse(args[0], out var result))
            {
                console.AddLine("What is your query?");
                return false;
            }
            protectionManager.InAdminMode = result;
            return false;
        }
    }

    class FlagsCommand : IConsoleCommand
    {
        public string Command => "flags";
        public string Help => "flags <num>";
        public string Description => "Controls what tile flags you're writing. See Content.Shared/TileFlags.cs for reference.";

        public bool Execute(IDebugConsole console, params string[] args)
        {
            var protectionManager = IoCManager.Resolve<ProtectionManager>();
            if (!protectionManager.LocalPlayerIsAdmin)
            {
                console.AddLine("Unauthorized presence detected. Security protocol activated.");
                return true;
            }
            if (args.Length != 1)
            {
                console.AddLine("What is your query?");
                return false;
            }
            if (!ushort.TryParse(args[0], out var result))
            {
                console.AddLine("What is your query?");
                return false;
            }
            protectionManager.Flags = result;
            return false;
        }
    }
}
