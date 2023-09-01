using Unity.Entities;
using UnityEngine;

namespace SpaceWars.Authoring
{
    public class CannonAuthoringAuthoring : MonoBehaviour
    {
        private class CannonAuthoringBaker : Baker<CannonAuthoringAuthoring>
        {
            public override void Bake(CannonAuthoringAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Cannon());
            }
        }
    }
}

// We use cannon as a tag to identify each of the four cannons on a spaceship.
// Not only that, but Entities with cannon component will be use to spawn the Bullets
public struct Cannon : IComponentData
{
}