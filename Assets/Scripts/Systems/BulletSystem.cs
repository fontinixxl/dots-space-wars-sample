using SpaceWars.Authoring;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace SpaceWars.Systems
{
    public partial struct BulletSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GameData>();
            state.RequireForUpdate<BulletComponentData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            
            var job = new MoveBulletJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime,
                Velocity = SystemAPI.GetSingleton<GameData>().BulletVelocity
            };

            job.ScheduleParallel();
        }
    }

    [BurstCompile]
    public partial struct MoveBulletJob : IJobEntity
    {
        [ReadOnly] public float DeltaTime;
        [ReadOnly] public float Velocity;

        void Execute(ref LocalTransform transform, in BulletComponentData bulletData)
        {
            transform.Position += DeltaTime * bulletData.Direction * Velocity;
        }
    }
}