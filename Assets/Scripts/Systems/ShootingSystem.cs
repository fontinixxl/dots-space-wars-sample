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
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var gameData = SystemAPI.GetSingleton<GameData>();

            // Get all the cannons (x4 in one spaceship) of all spaceships, so when can spawn bullets from them.
            foreach (var localToWorld in SystemAPI.Query<RefRO<LocalToWorld>>()
                         .WithAll<Cannon>())
            {
                Entity bullet = state.EntityManager.Instantiate(gameData.BulletPrefab);
                
                state.EntityManager.SetComponentData(bullet, new LocalTransform
                {
                    // The bullet will spawn on the tip of the cannon as defined in the Transform GameObject of the Prefab
                    Position = localToWorld.ValueRO.Position,
                    // Because the bullet may be rotated, we are combining the rotation of the cannon and the rotation of the bullet prefab
                    Rotation = math.mul(localToWorld.ValueRO.Rotation, SystemAPI.GetComponent<LocalTransform>(gameData.BulletPrefab).Rotation),
                    // Keep the same scale as the one found on the prefab
                    Scale = SystemAPI.GetComponent<LocalTransform>(gameData.BulletPrefab).Scale
                });
                
                // Since at this point we have a reference to the localTransform of the cannon, we use it to
                // specify the direction in which the bullet need to move.
                // BulletSystem will apply the movement based on this.
                state.EntityManager.SetComponentData(bullet, new BulletComponentData
                {
                    Direction = math.forward(localToWorld.ValueRO.Rotation),
                    // Initializer timer to default value specified on Game Data Authoring
                    LifeTime = gameData.BulletInitialLifeTime
                });
            }
        }
    }
}