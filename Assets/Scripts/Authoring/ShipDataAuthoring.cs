using Unity.Entities;
using UnityEngine;

namespace SpaceWars.Authoring
{
    public class ShipDataAuthoring : MonoBehaviour
    {
        public class ShipDataBaker : Baker<ShipDataAuthoring>
        {
            public override void Bake(ShipDataAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new ShipData());
            }
        }
    }

    public struct ShipData : IComponentData
    {
        public float Speed;
        public float RotationSpeed;
        public int CurrentWaypoint;
        public bool IsApproachingPlanet;
    }

    public struct Shooting : IComponentData, IEnableableComponent
    {
    }
}