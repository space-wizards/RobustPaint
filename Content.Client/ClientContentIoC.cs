using Content.Client.UserInterface;
using Robust.Shared.IoC;

namespace Content.Client
{
    internal static class ClientContentIoC
    {
        public static void Register()
        {
            // DEVNOTE: IoCManager registrations for the client go here and only here.
            IoCManager.Register<UIManager, UIManager>();
            IoCManager.Register<StyleSheetManager, StyleSheetManager>();
            IoCManager.Register<ProtectionManager, ProtectionManager>();
            IoCManager.Register<ConGroupManager, ConGroupManager>();
        }
    }
}
