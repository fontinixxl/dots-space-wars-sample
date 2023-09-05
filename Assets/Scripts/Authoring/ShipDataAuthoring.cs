using Unity.Entities;
using Unity.Mathematics;
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
                AddComponent<ShipData>(entity);
                AddComponent<Shooting>(entity);
                SetComponentEnabled<Shooting>(entity, false);
            }
        }
    }

    public struct ShipData : IComponentData
    {
        public float Speed;
        public float RotationSpeed;
        public int CurrentWaypoint;
        public float3 TargetPlanetPosition;
        public bool IsApproachingPlanet;
        // Debug purpose
        public float lineOfSightAngle;
    }
    
    // This is a tag component that is also an "enableable component".
    // Such components can be toggled on and off without removing the component from the entity,
    // which would be less efficient and wouldn't retain the component's value.
    // An Enableable component is initially enabled.
    public struct Shooting : IComponentData, IEnableableComponent
    {
    }
}