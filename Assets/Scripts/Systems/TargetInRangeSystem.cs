using SpaceWars.Authoring;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace SpaceWars.Systems
{
    // Determines whether a spaceship is in target's range to start shooting
    [UpdateBefore(typeof(ShootingSystem))]
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial struct TargetInRangeSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GameData>();
            state.RequireForUpdate<Planet>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Access to current way point -> ShipData component
            var gameData = SystemAPI.GetSingleton<GameData>();

            var job = new SetShootingStateJob
            {
                FieldOfViewAngle = gameData.FieldOfViewAngle,
                MinDistanceShooting = gameData.MinDistanceToTargetShooting
            };
            job.ScheduleParallel();

        }

        [WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
        [BurstCompile]
        public partial struct SetShootingStateJob : IJobEntity
        {
            [ReadOnly] public float FieldOfViewAngle;
            [ReadOnly] public float MinDistanceShooting;
                
            void Execute(in LocalTransform transform, in ShipData shipData, EnabledRefRW<Shooting> shootingState)
            {
                shootingState.ValueRW = IsTargetInSight(transform, shipData.TargetPlanetPosition);
            }

            // Calculates the angle between the spaceship's forward direction and the direction to the target manually
            // using the dot product ( a⋅b=∣a∣×∣b∣×cos(θ) ) and the magnitudes of the two vectors
            private bool IsTargetInSight(LocalTransform transform, float3 targetPosition)
            {
                var directionToTarget = targetPosition - transform.Position;
                
                // If we are far from the target, no need to even check for the line of sight
                if (math.lengthsq(directionToTarget) > MinDistanceShooting * MinDistanceShooting)
                {
                    return false;
                }
                
                var shipForwardDir = math.forward(transform.Rotation);
                var dotProduct = math.dot(shipForwardDir, directionToTarget);
                var magnitudeProduct = math.length(shipForwardDir) * math.length(directionToTarget);
                var cosTheta = dotProduct / magnitudeProduct;
                
                return math.acos(cosTheta) < math.radians(FieldOfViewAngle);
            }
        }
    }
}