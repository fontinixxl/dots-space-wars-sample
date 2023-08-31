using SpaceWars.Authoring;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace SpaceWars.Systems
{
    // System responsible for determining whether a ship is approaching its target planet
    public partial struct SetTargetStateSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ShipData>();
        }
        
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var job = new SetTargetStateJob
            {
                Waypoints =  SystemAPI.GetSingletonBuffer<PlanetsWayPointsPositions>().
                    Reinterpret<float3>().AsNativeArray()
            };
            job.ScheduleParallel();
        }
    }
}

[BurstCompile]
partial struct SetTargetStateJob : IJobEntity
{
    [ReadOnly] public NativeArray<float3> Waypoints;
    
    void Execute(in LocalTransform transform, ref ShipData shipData)
    {
        var distance = math.distance(transform.Position, Waypoints[shipData.CurrentWaypoint]);
        if (distance < 60)
            shipData.IsApproachingPlanet = false;
        else if (distance > 300)
            shipData.IsApproachingPlanet = true;
    }
}
