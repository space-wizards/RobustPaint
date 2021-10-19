using System;
using Robust.Client.Console;
using Robust.Shared.IoC;

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
