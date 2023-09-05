using SpaceWars.Authoring;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace SpaceWars.Systems
{
    public partial struct BulletSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GameData>();
            state.RequireForUpdate<Bullet>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var planetsQuery = SystemAPI.QueryBuilder().WithAll<LocalTransform, Planet>().Build();
            var job = new MoveBulletJob
            {
                PlanetTransforms = planetsQuery.ToComponentDataArray<LocalTransform>(state.WorldUpdateAllocator),
                DeltaTime = SystemAPI.Time.DeltaTime,
                Velocity = SystemAPI.GetSingleton<GameData>().BulletVelocity
            };
            job.ScheduleParallel();
        }
    }

    [BurstCompile]
    public partial struct MoveBulletJob : IJobEntity
    {
        private const float BulletOffset = 0.1f;

        [ReadOnly] public NativeArray<LocalTransform> PlanetTransforms;
        [ReadOnly] public float DeltaTime;
        [ReadOnly] public float Velocity;

        void Execute(ref LocalTransform transform, ref Bullet bulletData)
        {
            transform.Position += DeltaTime * bulletData.Direction * Velocity;

            foreach (var planetTransform in PlanetTransforms)
            {
                var minDistanceToPlanet = (planetTransform.Scale / 2) + BulletOffset;
                if (math.distancesq(transform.Position, planetTransform.Position) <
                    minDistanceToPlanet * minDistanceToPlanet)
                {
                    bulletData.LifeTime = 0;
                }
            }
        }
    }
}