using System;
using System.Threading;
using Content.Shared;
using Content.Shared.GameObjects.Components;
using Content.Shared.Network;
using Robust.Client.Player;
using Robust.Client.Console;
using Robust.Shared.Enums;
using Robust.Shared.ContentPack;
using Robust.Shared.Console;
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

namespace Content.Client
{
    public class ConGroupManager : IPostInjectInit, IClientConGroupImplementation
    {
        [Dependency] private readonly IClientConGroupController _conGroup = default!;

        public event Action ConGroupUpdated;

        void IPostInjectInit.PostInject()
        {
            _conGroup.Implementation = this;
            ConGroupUpdated.Invoke();
        }

        public bool CanCommand(string cmdName) => true;
        public bool CanViewVar() => true;
        public bool CanAdminPlace() => true;
        public bool CanScript() => true;
        public bool CanAdminMenu() => true;
    }
}
