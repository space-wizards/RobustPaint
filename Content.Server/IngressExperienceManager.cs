using System;
using System.Threading;
using Content.Shared;
using Content.Shared.Network;
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
using Robust.Shared.Log;
using Robust.Shared.Timing;
using Robust.Shared.Maths;
using Robust.Shared.Interfaces.Configuration;
using Timer = Robust.Shared.Timers.Timer;

namespace Content.Server
{
    public class IngressExperienceManager
    {
        [Dependency] private IServerNetManager _netManager = default!;
        [Dependency] private IMapManager _mapManager = default!;
        [Dependency] private IEntityManager _entityManager = default!;
        [Dependency] private IPlayerManager _playerManager = default!;
        [Dependency] private IConfigurationManager _cfg = default!;

        public MapId IngressMap { get; private set; } = default!;
        public IMapGrid IngressGrid { get; private set; } = default!;
        public Box2i MapExtent { get; private set; } = default!;
        public Box2 MapExtentF { get; private set; } = default!;

        public void Initialize()
        {
            _netManager.RegisterNetMessage<MsgShowMessage>(nameof(MsgShowMessage));
            // Create ingress point map.
            IngressMap = _mapManager.CreateMap();
            var extent = _cfg.GetCVar(GameConfigVars.MapExtent);
            var extentFrontHalf = extent / 2;
            var extentBackHalf = extent - extentFrontHalf;
            // this is a bit weird but it works
            MapExtent = new Box2i(-extentBackHalf, -extentBackHalf, extentFrontHalf - 1, extentFrontHalf - 1);
            MapExtentF = new Box2(-extentBackHalf, -extentBackHalf, extentFrontHalf, extentFrontHalf);

            var world = IoCManager.Resolve<IMapLoader>().LoadBlueprint(IngressMap, _cfg.GetCVar(GameConfigVars.MapFile));
            if (world == null)
            {
                IngressGrid = _mapManager.CreateGrid(IngressMap);

                IngressGrid.SetTile(new Vector2i(-1, -1), new Tile(0));
                IngressGrid.SetTile(new Vector2i(0, -1), new Tile(1));
                IngressGrid.SetTile(new Vector2i(1, -1), new Tile(0));

                IngressGrid.SetTile(new Vector2i(-1, 0), new Tile(1));
                IngressGrid.SetTile(new Vector2i(0, 0), new Tile(0));
                IngressGrid.SetTile(new Vector2i(1, 0), new Tile(1));

                IngressGrid.SetTile(new Vector2i(-1, 1), new Tile(0));
                IngressGrid.SetTile(new Vector2i(0, 1), new Tile(1));
                IngressGrid.SetTile(new Vector2i(1, 1), new Tile(0));
            }
            else
            {
                IngressGrid = world;
            }

            // Setup join experience.
            _playerManager.PlayerStatusChanged += PlayerStatusChanged;

            StartAutosaveTimer();
        }

        private void StartAutosaveTimer()
        {
            Timer.Spawn(TimeSpan.FromSeconds(_cfg.GetCVar(GameConfigVars.SaveInterval)), () => Autosave());
        }

        private void Autosave()
        {
            var file = _cfg.GetCVar(GameConfigVars.MapFile);
            Logger.InfoS("c.s.ingress", "Saving {0}", file);
            IoCManager.Resolve<IMapLoader>().SaveBlueprint(IngressGrid.Index, file);
            StartAutosaveTimer();
        }

        private void PlayerStatusChanged(object blah, SessionStatusEventArgs args)
        {
            if (args.NewStatus == SessionStatus.Connected)
            {
                // Setup an entity for the player...
                var playerEntity = _entityManager.SpawnEntity("Player", new EntityCoordinates(_mapManager.GetMapEntityId(IngressMap), 0.5f, 0.5f));
                // This brings them into the InGame runlevel and to the InGame session status.
                args.Session.AttachToEntity(playerEntity);
                args.Session.JoinGame();

                var msg = _netManager.CreateNetMessage<MsgShowMessage>();
                msg.Text = "Test MOTD";
                _netManager.ServerSendMessage(msg, args.Session.ConnectedClient);
            }
            else if ((args.NewStatus == SessionStatus.Disconnected) || (args.NewStatus == SessionStatus.Zombie))
            {
                var hasEnt = args.Session.AttachedEntity;
                if (hasEnt != null) {
                    args.Session.DetachFromEntity();
                    hasEnt.Delete();
                }
            }
        }
    }
}
