using SpaceWars.Authoring;
using Unity.Burst;
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
            var job = new SetTargetStateJob();
            job.ScheduleParallel();
        }
    }
}

[BurstCompile]
partial struct SetTargetStateJob : IJobEntity
{
    void Execute(in LocalTransform transform, ref ShipData shipData)
    {
        var distance = math.distance(transform.Position, shipData.TargetPlanetPosition);
        if (distance < 60)
            shipData.IsApproachingPlanet = false;
        else if (distance > 300)
            shipData.IsApproachingPlanet = true;
    }
}
