using Unity.Entities;
using UnityEngine;

namespace SpaceWars.Authoring
{
    public class TimeAliveAuthoring : MonoBehaviour
    {
        private class ParticleDataAuthoringBaker : Baker<TimeAliveAuthoring>
        {
            public override void Bake(TimeAliveAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new TimeAlive());
            }
        }
    }

    public struct TimeAlive : IComponentData
    {
        public float Value;
    }
}