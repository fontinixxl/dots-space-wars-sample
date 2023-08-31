using Unity.Entities;
using UnityEngine;

namespace SpaceWars.Authoring
{
    public class ShipDataAuthoring : MonoBehaviour
    {
        public float speed;
        public float rotationSpeed;
        public int currentWaypoint;

        public class ShipDataBaker : Baker<ShipDataAuthoring>
        {
            public override void Bake(ShipDataAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new ShipData
                {
                    Speed = authoring.speed,
                    RotationSpeed = authoring.rotationSpeed,
                    CurrentWaypoint = authoring.currentWaypoint
                });
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
}