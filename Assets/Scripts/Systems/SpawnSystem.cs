using SpaceWars.Authoring;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace SpaceWars.Systems
{
    public partial struct SpawnSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SpaceShipBakingData>();
            state.RequireForUpdate<PlanetsWayPointsPositions>();
        }
    
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // We only want to spawn obstacles one time. Disabling the system stops subsequent updates.
            state.Enabled = false;
        
            // Get component data using Singleton, since there is only one Entity matching those components
            // Notice though, for the Buffer we need to use GetSingletonBuffer
            var wayPointsArray = SystemAPI.GetSingletonBuffer<PlanetsWayPointsPositions>().Reinterpret<float3>().AsNativeArray();
            var shipData = SystemAPI.GetSingleton<SpaceShipBakingData>();
        
            var rand = new Random(123);
            for (var i = 0; i < shipData.ShipsToSpawn; i++)
            {
                var spaceShip = state.EntityManager.Instantiate(shipData.ShipPrefab);
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

                var closestWp = 0;
                var closestDistance = math.INFINITY;
                for (var j = 0; j < wayPointsArray.Length; j++)
                {
                    var distance = Vector3.Distance(wayPointsArray[j], position);
                    if ( distance < closestDistance)
                    {
                        closestWp = j;
                        closestDistance = distance;
                    }
                }
                
                state.EntityManager.SetComponentData(spaceShip, new ShipData
                {
                    Speed = rand.NextInt(5, 20),
                    RotationSpeed = rand.NextInt(3, 5),
                    CurrentWaypoint = closestWp
                });
            }
        }
    }
}