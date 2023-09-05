using SpaceWars.Authoring;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace SpaceWars.Systems
{
    [UpdateAfter(typeof(BulletSystem))]
    public partial struct ScaleOverLifeTimeSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TimeAlive>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (transform, timeAlive) in 
                     SystemAPI.Query<RefRW<LocalTransform>, RefRW<TimeAlive>>())
            {
                timeAlive.ValueRW.Value += SystemAPI.Time.DeltaTime;
                transform.ValueRW.Scale += timeAlive.ValueRO.Value * 0.7f;
            }
        }
    }
}