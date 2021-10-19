using Robust.Shared.Console;
using Robust.Shared.IoC;

namespace Content.Client
{
    class InAdminModeCommand : IConsoleCommand
    {
        public string Command => "admin";
        public string Help => "admin <bool>";
        public string Description => "\"For the honour of...\" Enables admin mode, if you are an admin.";

        public void Execute(IConsoleShell console, string argStr, params string[] args)
        {
            var protectionManager = IoCManager.Resolve<ProtectionManager>();
            if (!protectionManager.LocalPlayerIsAdmin)
            {
                console.WriteLine("Unauthorized presence detected. Security protocol activated.");
                return;
            }
            if (args.Length != 1)
            {
                console.WriteLine("What is your query?");
                return;
            }
            if (!bool.TryParse(args[0], out var result))
            {
                console.WriteLine("What is your query?");
                return;
            }
            protectionManager.InAdminMode = result;
        }
    }

    class FlagsCommand : IConsoleCommand
    {
        public string Command => "flags";
        public string Help => "flags <num>";
        public string Description => "Controls what tile flags you're writing. See Content.Shared/TileFlags.cs for reference.";

        public void Execute(IConsoleShell console, string argStr, params string[] args)
        {
            var protectionManager = IoCManager.Resolve<ProtectionManager>();
            if (!protectionManager.LocalPlayerIsAdmin)
            {
                console.WriteLine("Unauthorized presence detected. Security protocol activated.");
                return;
            }
            if (args.Length != 1)
            {
                console.WriteLine("What is your query?");
                return;
            }
            if (!ushort.TryParse(args[0], out var result))
            {
                console.WriteLine("What is your query?");
                return;
            }
            protectionManager.Flags = result;
        }
    }
}
