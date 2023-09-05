using SpaceWars.Authoring;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace SpaceWars.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct BulletSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<Config>();
            state.RequireForUpdate<Bullet>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var config = SystemAPI.GetSingleton<Config>();
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var planetsQuery = SystemAPI.QueryBuilder().WithAll<LocalTransform, Planet>().Build();
            var job = new MoveBulletJob
            {
                ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged),
                PlanetTransforms = planetsQuery.ToComponentDataArray<LocalTransform>(state.WorldUpdateAllocator),
                DeltaTime = SystemAPI.Time.DeltaTime,
                Velocity = SystemAPI.GetSingleton<Config>().BulletVelocity,
                ExplosionPrefab = config.ExplosionPrefab,
                Random = Random.CreateFromIndex(1234),
                ExplosionProbability = config.ExplosionProbability
            };
            job.Schedule();
        }
    }

    [BurstCompile]
    public partial struct MoveBulletJob : IJobEntity
    {
        private const float BulletOffset = 0.1f;

        [ReadOnly] public NativeArray<LocalTransform> PlanetTransforms;
        [ReadOnly] public float DeltaTime;
        [ReadOnly] public float Velocity;

        public EntityCommandBuffer ECB;
        public Entity ExplosionPrefab;
        public Random Random;
        public float ExplosionProbability;

        void Execute(ref LocalTransform bulletTransform, in Bullet bulletData, ref LifeTime lifeTime)
        {
            bulletTransform.Position += DeltaTime * bulletData.Direction * Velocity;

            foreach (var planetTransform in PlanetTransforms)
            {
                var minDistanceToPlanet = (planetTransform.Scale / 2) + BulletOffset;
                if (math.distancesq(bulletTransform.Position, planetTransform.Position) <
                    minDistanceToPlanet * minDistanceToPlanet)
                {
                    lifeTime.Value = 0;

                    if (Random.NextFloat() <= ExplosionProbability)
                    {
                        var explosionEntity = ECB.Instantiate(ExplosionPrefab);
                        ECB.SetComponent(explosionEntity, new LocalTransform
                        {
                            Position = bulletTransform.Position,
                            Rotation = quaternion.LookRotationSafe(math.normalize(bulletData.Direction), math.up()),
                            Scale = 0.5f
                        });
                    }
                }
            }
        }
    }
}