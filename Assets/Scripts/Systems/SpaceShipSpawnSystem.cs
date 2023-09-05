using SpaceWars.Authoring;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace SpaceWars.Systems
{
    public partial struct SpaceShipSpawnSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Config>();
            state.RequireForUpdate<Planet>();
        }
    
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // We only want to spawn one time. Disabling the system stops subsequent updates.
            state.Enabled = false;
        
            // Get component data using Singleton, since there is only one Entity matching those components
            var gameData = SystemAPI.GetSingleton<Config>();
        
            var rand = new Random(123);
            for (var i = 0; i < gameData.ShipsToSpawn; i++)
            {
                var spaceShip = state.EntityManager.Instantiate(gameData.ShipPrefab);
                var position = new float3
                {
                    x = rand.NextInt(-300, 300),
                    y = rand.NextInt(-300, 300),
                    z = rand.NextInt(-300, 300)

                };
                var q = Quaternion.Euler(new Vector3(0, 45, 0));
                state.EntityManager.SetComponentData(spaceShip, new LocalTransform
                {
                    Position = position,
                    Scale = .5f,
                    Rotation = new quaternion(q.x, q.y, q.z, q.w)
                });

                var closestDistance = math.INFINITY;
                float3 targetPlanetPos = float3.zero;
                foreach (var transform in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<Planet>())
                {
                    var distanceSq = math.distancesq(transform.ValueRO.Position, position);
                    if ( distanceSq < closestDistance)
                    {
                        closestDistance = distanceSq;
                        targetPlanetPos = transform.ValueRO.Position;
                    }
                }
                
                state.EntityManager.SetComponentData(spaceShip, new ShipData
                {
                    Speed = rand.NextInt(5, 20),
                    RotationSpeed = rand.NextInt(3, 5),
                    TargetPlanetPosition = targetPlanetPos
                });
            }
        }
    }
}
