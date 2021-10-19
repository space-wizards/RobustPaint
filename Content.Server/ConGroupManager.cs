using Content.Shared.GameObjects.Components;
using Robust.Server.Player;
using Robust.Server.Console;
using Robust.Shared.IoC;

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
        public bool CanAdminReloadPrototypes(IPlayerSession session) => IsAdmin(session);
    }
}
