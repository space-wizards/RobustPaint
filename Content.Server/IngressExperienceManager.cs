using System;
using System.Collections.Generic;
using Content.Shared;
using Content.Shared.Network;
using Robust.Server.Maps;
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Timer = Robust.Shared.Timing.Timer;

namespace Content.Server
{
    public class IngressExperienceManager
    {
        [Dependency] private IServerNetManager _netManager = default!;
        [Dependency] private IMapManager _mapManager = default!;
        [Dependency] private IEntityManager _entityManager = default!;
        [Dependency] private IPlayerManager _playerManager = default!;
        [Dependency] private IConfigurationManager _cfg = default!;
        [Dependency] private IPrototypeManager _prototypeManager = default!;

        public MapId IngressMap { get; private set; } = default!;
        public IMapGrid IngressGrid { get; private set; } = default!;
        public Box2i MapExtent { get; private set; } = default!;
        public Box2 MapExtentF { get; private set; } = default!;

        public Dictionary<string, ModerationDefinition> TempBans = new Dictionary<string, ModerationDefinition>();

        public void Initialize()
        {
            _netManager.RegisterNetMessage<MsgShowMessage>();
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

        private ModerationDefinition GetModerationDefinition(IPlayerSession session)
        {
            var uuid = session.UserId.ToString();
            if (TempBans.ContainsKey(uuid))
                return TempBans[uuid];
            if (_prototypeManager.TryIndex<ModerationDefinition>(uuid, out var moderation))
                return moderation;
            if (session.Name != null)
                if (_prototypeManager.TryIndex<ModerationDefinition>("NAME=" + session.Name, out var moderation2))
                    return moderation2;
            return null;
        }

        private void PlayerStatusChanged(object blah, SessionStatusEventArgs args)
        {
            if (args.NewStatus == SessionStatus.Connected)
            {
                // Determine what to do with them
                var moderationEntity = "Player";
                var moderationText = "";
                var moderation = GetModerationDefinition(args.Session);
                if (moderation != null)
                {
                    moderationEntity = moderation.Entity;
                    moderationText = moderation.Text;
                }

                // Setup an entity for the player...
                var playerEntity = _entityManager.SpawnEntity(moderationEntity, new EntityCoordinates(_mapManager.GetMapEntityId(IngressMap), 0.5f, 0.5f));
                playerEntity.Name = args.Session.ToString();
                // This brings them into the InGame runlevel and to the InGame session status.
                args.Session.AttachToEntity(playerEntity);
                args.Session.JoinGame();

                foreach (var msgPrototype in _prototypeManager.EnumeratePrototypes<MessageDefinition>())
                {
                    var msg = _netManager.CreateNetMessage<MsgShowMessage>();
                    msg.Text = msgPrototype.Text;
                    _netManager.ServerSendMessage(msg, args.Session.ConnectedClient);
                }

                // Show this one last
                if (moderationText != "")
                {
                    var msg = _netManager.CreateNetMessage<MsgShowMessage>();
                    msg.Text = moderationText;
                    _netManager.ServerSendMessage(msg, args.Session.ConnectedClient);
                }
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
