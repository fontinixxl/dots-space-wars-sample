using SpaceWars.Authoring;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace SpaceWars.Systems
{
    /// <summary>
    /// System responsible of moving the spaceships either towards their target planet or away from it
    /// This state change is calculated in <see cref="SpaceWars.Systems.SetTargetStateSystem"/>
    /// </summary>
    public partial struct SpaceShipMoveSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ShipData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var job = new MoveTowardsTargetJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime,
            };
            job.ScheduleParallel();
        }
    }

    [BurstCompile]
    partial struct MoveTowardsTargetJob : IJobEntity
    {
        public float DeltaTime;
    
        void Execute(ref LocalTransform transform, in ShipData shipData)
        {
            float3 heading;
            if (shipData.IsApproachingPlanet)
                heading = shipData.TargetPlanetPosition - transform.Position;
            else
                heading = transform.Position - shipData.TargetPlanetPosition;

            var targetDirection = quaternion.LookRotation(heading, math.up());
            transform.Rotation = math.slerp(transform.Rotation, targetDirection, DeltaTime * shipData.RotationSpeed);
            transform.Position += DeltaTime * shipData.Speed * math.forward(transform.Rotation);
        }
    }
}
