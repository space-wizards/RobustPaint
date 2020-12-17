using Robust.Server.Interfaces.Player;
using Robust.Server.Player;
using Robust.Shared.Enums;
using Robust.Shared.ContentPack;
using Robust.Shared.Interfaces.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Server
{
    public class EntryPoint : GameServer
    {
        public override void Init()
        {
            base.Init();

            var factory = IoCManager.Resolve<IComponentFactory>();

            factory.DoAutoRegistrations();

            foreach (var ignoreName in IgnoredComponents.List)
            {
                factory.RegisterIgnore(ignoreName);
            }

            ServerContentIoC.Register();

            IoCManager.BuildGraph();

            // DEVNOTE: This is generally where you'll be setting up the IoCManager further.
        }

        public override void PostInit()
        {
            base.PostInit();
            // DEVNOTE: Can also initialize IoC stuff more here.

            IoCManager.Resolve<IngressExperienceManager>().Initialize();
        }

        public override void Update(ModUpdateLevel level, FrameEventArgs frameEventArgs)
        {
            base.Update(level, frameEventArgs);
            // DEVNOTE: Game update loop goes here. Usually you'll want some independent GameTicker.
        }

        private void PlayerStatusChanged(object blah, SessionStatusEventArgs args)
        {
            if (args.NewStatus == SessionStatus.Connected)
            {
                // Setup an entity for the player...

                // This brings them into the InGame runlevel and to the InGame session status.
                args.Session.JoinGame();
            }
        }
    }
}
