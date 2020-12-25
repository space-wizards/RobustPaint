using System;
using System.Linq;
using System.Text;
using Robust.Server.Interfaces.Console;
using Robust.Server.Interfaces.Player;
using Robust.Shared.Enums;
using Robust.Shared.Interfaces.GameObjects;
using Robust.Shared.Interfaces.Map;
using Robust.Shared.Interfaces.Network;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

namespace Content.Server
{
    public class KickBanCommand : IClientCommand
    {
        public string Command => "kicktempban";
        public string Description => "Kicks a connected player and bans them until server restart. Follow this up with a full incident report and update of server moderation file.";
        public string Help => "kicktempban <username> <reason>";

        public void Execute(IConsoleShell shell, IPlayerSession player, string[] args)
        {
            if (args.Length != 2)
            {
                shell.SendText(player, "Incorrect argument count");
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
                shell.SendText(player, "Their user ID was: " + uuid);
                IoCManager.Resolve<IServerNetManager>().DisconnectChannel(target.ConnectedClient, args[1]);
            }
            else
            {
                shell.SendText(player, "Unknown user");
            }
        }
    }

    public class Kick2Command : IClientCommand
    {
        public string Command => "kick2";
        public string Description => "Kick backup because robusttoolbox issues";
        public string Help => "kick2 <username>";

        public void Execute(IConsoleShell shell, IPlayerSession player, string[] args)
        {
            if (args.Length != 1)
            {
                shell.SendText(player, "Incorrect argument count");
                return;
            }

            if (IoCManager.Resolve<IPlayerManager>().TryGetSessionByUsername(args[0], out var target))
            {
                var uuid = target.UserId.ToString();
                shell.SendText(player, "Their user ID was: " + uuid);
                IoCManager.Resolve<IServerNetManager>().DisconnectChannel(target.ConnectedClient, args[1]);
            }
            else
            {
                shell.SendText(player, "Unknown user");
            }
        }
    }

    public class GetUUIDCommand : IClientCommand
    {
        public string Command => "uuid";
        public string Description => "Get UUID of player";
        public string Help => "uuid <username>";

        public void Execute(IConsoleShell shell, IPlayerSession player, string[] args)
        {
            if (args.Length != 1)
            {
                shell.SendText(player, "Incorrect argument count");
                return;
            }

            if (IoCManager.Resolve<IPlayerManager>().TryGetSessionByUsername(args[0], out var target))
            {
                var uuid = target.UserId.ToString();
                shell.SendText(player, "Their UUID is: " + uuid);
            }
            else
            {
                shell.SendText(player, "Unknown user");
            }
        }
    }

    public class WorldDumpToLogCommand : IClientCommand
    {
        public string Command => "worlddumptolog";
        public string Description => "WARNING: WILL SPAM THE EVERLIVING DAYLIGHTS OUT OF LOG, ONLY USE IF YOU'RE SURE";
        public string Help => "worlddumptolog";

        public void Execute(IConsoleShell shell, IPlayerSession player, string[] args)
        {
            if (args.Length != 0)
            {
                shell.SendText(player, "Incorrect argument count");
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
