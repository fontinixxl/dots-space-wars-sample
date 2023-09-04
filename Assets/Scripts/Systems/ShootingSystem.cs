using SpaceWars.Authoring;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace SpaceWars.Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial struct ShootingSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GameData>();
            state.RequireForUpdate<Cannon>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var gameData = SystemAPI.GetSingleton<GameData>();

            // Get all entities that have shooting component enabled
            foreach (var (_, entity) in SystemAPI.Query<RefRO<LocalToWorld>>()
                         .WithAll<Shooting>().WithEntityAccess())
            {
                // For each entity, get all the cannon child components in order to get the position and rotation
                // for spawning the bullets
                DynamicBuffer<Child> children = SystemAPI.GetBuffer<Child>(entity);
                for (int i = 0; i < children.Length; i++)
                {
                    var cannonEntity = children[i].Value;
                    if (!SystemAPI.HasComponent<Cannon>(cannonEntity))
                        continue;

                    var cannonWorldPos = SystemAPI.GetComponent<LocalToWorld>(cannonEntity);
                    Entity bullet = state.EntityManager.Instantiate(gameData.BulletPrefab);

                    state.EntityManager.SetComponentData(bullet, new LocalTransform
                    {
                        // The bullet will spawn on the tip of the cannon as defined in the Transform GameObject of the Prefab
                        Position = cannonWorldPos.Position,
                        // Because the bullet may be rotated, we are combining the rotation of the cannon and the rotation of the bullet prefab
                        Rotation = math.mul(cannonWorldPos.Rotation,
                            SystemAPI.GetComponent<LocalTransform>(gameData.BulletPrefab).Rotation),
                        // Keep the same scale as the one found on the prefab
                        Scale = SystemAPI.GetComponent<LocalTransform>(gameData.BulletPrefab).Scale
                    });

                    // Since at this point we have a reference to the localTransform of the cannon, we use it to
                    // specify the direction in which the bullet need to move.
                    // BulletSystem will apply the movement based on this.
                    state.EntityManager.SetComponentData(bullet, new BulletComponentData
                    {
                        Direction = math.forward(cannonWorldPos.Rotation),
                        // Initialize timer to default value specified on Game Data Authoring
                        LifeTime = gameData.BulletInitialLifeTime
                    });
                }
            }
        }
    }
}