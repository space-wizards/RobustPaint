using Robust.Server.Interfaces.Player;
using Robust.Server.Player;
using Robust.Shared.Enums;
using Robust.Shared.ContentPack;
using Robust.Shared.Interfaces.GameObjects;
using Robust.Shared.Interfaces.Map;
using Robust.Shared.Map;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using Robust.Shared.Maths;

namespace Content.Server
{
    public class IngressExperienceManager
    {
        [Dependency] private IMapManager _mapManager = default!;
        [Dependency] private IEntityManager _entityManager = default!;
        [Dependency] private IPlayerManager _playerManager = default!;

        public MapId IngressMap { get; private set; }
        public IMapGrid IngressGrid { get; private set; }

        public void Initialize()
        {
            // Create ingress point map.
            IngressMap = _mapManager.CreateMap();

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

            // Setup join experience.
            _playerManager.PlayerStatusChanged += PlayerStatusChanged;
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
