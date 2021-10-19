using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Network;

namespace Content.Server
{
    public class KickBanCommand : IConsoleCommand
    {
        public string Command => "kicktempban";
        public string Description => "Kicks a connected player and bans them until server restart. Follow this up with a full incident report and update of server moderation file.";
        public string Help => "kicktempban <username> <reason>";

        public void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            if (args.Length != 2)
            {
                shell.WriteLine("Incorrect argument count");
                return;
            }

            if (IoCManager.Resolve<IPlayerManager>().TryGetSessionByUsername(args[0], out var target))
            {
                var uuid = target.UserId.ToString();
                var synth = new ModerationDefinition();
                synth.UUID = uuid;
                synth.Entity = "BannedPlayer";
                synth.Text = "Ban Until Server Restart:\n" + args[1];
                IoCManager.Resolve<IngressExperienceManager>().TempBans[uuid] = synth;
                shell.WriteLine("Their user ID was: " + uuid);
                IoCManager.Resolve<IServerNetManager>().DisconnectChannel(target.ConnectedClient, args[1]);
            }
            else
            {
                shell.WriteLine("Unknown user");
            }
        }
    }

    public class Kick2Command : IConsoleCommand
    {
        public string Command => "kick2";
        public string Description => "Kick backup because robusttoolbox issues";
        public string Help => "kick2 <username>";

        public void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            if (args.Length != 1)
            {
                shell.WriteLine("Incorrect argument count");
                return;
            }

            if (IoCManager.Resolve<IPlayerManager>().TryGetSessionByUsername(args[0], out var target))
            {
                var uuid = target.UserId.ToString();
                shell.WriteLine("Their user ID was: " + uuid);
                IoCManager.Resolve<IServerNetManager>().DisconnectChannel(target.ConnectedClient, args[1]);
            }
            else
            {
                shell.WriteLine("Unknown user");
            }
        }
    }

    public class GetUUIDCommand : IConsoleCommand
    {
        public string Command => "uuid";
        public string Description => "Get UUID of player";
        public string Help => "uuid <username>";

        public void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            if (args.Length != 1)
            {
                shell.WriteLine("Incorrect argument count");
                return;
            }

            if (IoCManager.Resolve<IPlayerManager>().TryGetSessionByUsername(args[0], out var target))
            {
                var uuid = target.UserId.ToString();
                shell.WriteLine("Their UUID is: " + uuid);
            }
            else
            {
                shell.WriteLine("Unknown user");
            }
        }
    }

    public class WorldDumpToLogCommand : IConsoleCommand
    {
        public string Command => "worlddumptolog";
        public string Description => "WARNING: WILL SPAM THE EVERLIVING DAYLIGHTS OUT OF LOG, ONLY USE IF YOU'RE SURE";
        public string Help => "worlddumptolog";

        public void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            if (args.Length != 0)
            {
                shell.WriteLine("Incorrect argument count");
                return;
            }

            var iem = IoCManager.Resolve<IngressExperienceManager>();
            foreach (var tileRef in iem.IngressGrid.GetAllTiles(false))
            {
                Logger.WarningS("c.s.worlddumptolog", "{0} = {1}", tileRef.GridIndices, tileRef.Tile.TypeId);
            }
       }
    }
}
