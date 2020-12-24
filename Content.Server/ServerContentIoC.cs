using Robust.Shared.IoC;

namespace Content.Server
{
    internal static class ServerContentIoC
    {
        public static void Register()
        {
            // DEVNOTE: IoCManager registrations for the server go here and only here.
            IoCManager.Register<IngressExperienceManager, IngressExperienceManager>();
            IoCManager.Register<ConGroupManager, ConGroupManager>();
        }
    }
}
