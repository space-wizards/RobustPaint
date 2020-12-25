using System;
using System.Threading;
using Content.Shared;
using Content.Shared.GameObjects.Components;
using Content.Shared.Network;
using Robust.Server.Console;
using Robust.Server.Interfaces.Console;
using Robust.Server.Interfaces.Player;
using Robust.Server.Interfaces.Maps;
using Robust.Server.Player;
using Robust.Shared.Enums;
using Robust.Shared.ContentPack;
using Robust.Shared.Interfaces.GameObjects;
using Robust.Shared.Interfaces.Map;
using Robust.Shared.Interfaces.Network;
using Robust.Shared.Map;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Network;
using Robust.Shared.Utility;
using Robust.Shared.Log;
using Robust.Shared.Timing;
using Robust.Shared.Maths;
using Robust.Shared.Interfaces.Configuration;
using Robust.Shared.Prototypes;

namespace Content.Server
{
    public class ConGroupManager : IPostInjectInit, IConGroupControllerImplementation
    {
        [Dependency] private readonly IConGroupController _conGroup = default!;

        void IPostInjectInit.PostInject()
        {
            _conGroup.Implementation = this;
        }

        public bool IsAdmin(IPlayerSession session)
        {
            return session.AttachedEntity?.HasComponent<PlayerAdminComponent>() ?? false;
        }

        public bool CanCommand(IPlayerSession session, string cmdName)
        {
            if (cmdName == "listplayers")
                return true;
            if (cmdName == "showtime")
                return true;
            if (cmdName == "list")
                return true;
            if (cmdName == "help")
                return true;
            return IsAdmin(session);
        }

        public bool CanViewVar(IPlayerSession session) => IsAdmin(session);
        public bool CanAdminPlace(IPlayerSession session) => IsAdmin(session);
        public bool CanScript(IPlayerSession session) => IsAdmin(session);
        public bool CanAdminMenu(IPlayerSession session) => IsAdmin(session);
    }
}
