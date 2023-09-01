using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace SpaceWars.Authoring
{
    public class BulletAuthoring : MonoBehaviour
    {
        public class BulletAuthoringBaker : Baker<BulletAuthoring>
        {
            public override void Bake(BulletAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new BulletComponentData());
            }
        }
    }

    public struct BulletComponentData : IComponentData
    {
        public float3 Direction;
    }
}