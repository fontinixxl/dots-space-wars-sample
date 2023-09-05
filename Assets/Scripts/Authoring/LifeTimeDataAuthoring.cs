using Unity.Entities;
using UnityEngine;

namespace SpaceWars.Authoring
{
    public class LifeTimeDataAuthoringAuthoring : MonoBehaviour
    {
        public float lifeTime;
        
        private class LifeTimeDataAuthoringBaker : Baker<LifeTimeDataAuthoringAuthoring>
        {
            public override void Bake(LifeTimeDataAuthoringAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new LifeTime { Value = authoring.lifeTime});
            }
        }
    }

    public struct LifeTime : IComponentData
    {
        public float Value;
    }
}