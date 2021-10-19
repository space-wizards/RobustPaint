using Content.Shared.Input;
using Content.Shared.GameObjects.Components;
using JetBrains.Annotations;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.Players;

namespace Content.Shared.GameObjects.EntitySystems
{
    [UsedImplicitly]
    public abstract class SharedPlayerKinesisSystem : EntitySystem
    {
        public override void Initialize()
        {
            base.Initialize();

            CommandBinds.Builder
                .Bind(EngineKeyFunctions.MoveUp, new PlayerKinesisInputCmdHandler(EngineKeyFunctions.MoveUp))
                .Bind(EngineKeyFunctions.MoveLeft, new PlayerKinesisInputCmdHandler(EngineKeyFunctions.MoveLeft))
                .Bind(EngineKeyFunctions.MoveRight, new PlayerKinesisInputCmdHandler(EngineKeyFunctions.MoveRight))
                .Bind(EngineKeyFunctions.MoveDown, new PlayerKinesisInputCmdHandler(EngineKeyFunctions.MoveDown))
                .Bind(ContentKeyFunctions.RP8NTSprint, new PlayerKinesisInputCmdHandler(ContentKeyFunctions.RP8NTSprint))
                .Register<SharedPlayerKinesisSystem>();
        }

        /// <inheritdoc />
        public override void Shutdown()
        {
            CommandBinds.Unregister<SharedPlayerKinesisSystem>();
            base.Shutdown();
        }

        public override void Update(float frameTime)
        {
            base.Update(frameTime);

            foreach (var kinesis in EntityManager.EntityQuery<SharedPlayerKinesisComponent>())
            {
                var localPosition = kinesis.Owner.Transform.LocalPosition;
                localPosition += (kinesis.Velocity * frameTime);
                localPosition = kinesis.TravelBounds.ClosestPoint(localPosition);
                kinesis.Owner.Transform.LocalPosition = localPosition;
            }
        }

        private sealed class PlayerKinesisInputCmdHandler : InputCmdHandler
        {
            private BoundKeyFunction _fn;
            public PlayerKinesisInputCmdHandler(BoundKeyFunction fn)
            {
                _fn = fn;
            }

            public override bool HandleCmdMessage(ICommonSession session, InputCmdMessage message)
            {
                // Logger.Warning("😀️ {0}, {1}", session, message);
                if (!(message is FullInputCmdMessage full))
                    return false;

                var entity = session?.AttachedEntity;
                if (entity == null)
                    return false;
                if (entity.TryGetComponent<SharedPlayerKinesisComponent>(out var kinesis))
                    kinesis.KineticMessage(_fn, (FullInputCmdMessage) message);
                return false;
            }
        }
    }
}
