using SpaceWars.Authoring;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace SpaceWars.Systems
{
    [UpdateAfter(typeof(BulletSystem))]
    public partial struct LifeTimeSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<Bullet>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var destroyTimedJob = new DestroyTimedJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime,
                ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged)
            };
            destroyTimedJob.Schedule();
        }
    }

    [BurstCompile]
    public partial struct DestroyTimedJob : IJobEntity
    {
        [ReadOnly] public float DeltaTime;
        public EntityCommandBuffer ECB;

        void Execute(Entity entity, ref LifeTime lifeTime)
        {
            lifeTime.Value -= DeltaTime;
            if (lifeTime.Value <= 0)
            {
                ECB.DestroyEntity(entity);
            }
        }
    }
}