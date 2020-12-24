using System;
using Content.Client;
using Robust.Client.UserInterface;
using Robust.Client.Interfaces.UserInterface;
using Robust.Client.Interfaces.ResourceManagement;
using Robust.Client.UserInterface.Controls;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface.XAML;
using Robust.Client.AutoGenerated;
using Robust.Shared.Enums;
using Robust.Shared.ContentPack;
using Robust.Shared.Interfaces.GameObjects;
using Robust.Shared.Interfaces.Resources;
using Robust.Shared.Interfaces.Map;
using Robust.Shared.Map;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using Robust.Shared.Maths;

namespace Content.Client.UserInterface
{
    [GenerateTypedNameReferences]
    public partial class MessageDialog : Control, IModal
    {
        public event EventHandler OnModalClosed;
        public MessageDialog(string text)
        {
            RobustXamlLoader.Load(this);
            Label.Text = text;
            OkButton.OnPressed += (a) => { Parent.RemoveChild(this); OnModalClosed.Invoke(this, new EventArgs()); };
        }
    }
}
