using Unity.Entities;
using UnityEngine;

namespace SpaceWars.Authoring
{
    public class PlanetAuthoringAuthoring : MonoBehaviour
    {
        private class PlanetAuthoringBaker : Baker<PlanetAuthoringAuthoring>
        {
            public override void Bake(PlanetAuthoringAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Planet());
            }
        }
    }

    public struct Planet : IComponentData
    {
    }
}
