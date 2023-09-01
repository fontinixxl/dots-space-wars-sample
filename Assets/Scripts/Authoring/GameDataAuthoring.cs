using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace SpaceWars.Authoring
{
    public class GameDataAuthoring : MonoBehaviour
    {
        public Transform[] waypoints;
        public GameObject spaceShip;
        public GameObject bulletPrefab;
        public int shipsToSpawn = 500;
        public float bulletVelocity = 100f;

        class Baker : Baker<GameDataAuthoring>
        {
            public override void Bake(GameDataAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);

                // In order to bake an array/List to a component, we need to create a new DynamicBuffer and attach it as a component of the entity
                var bufferPosition = AddBuffer<PlanetsWayPointsPositions>(entity).Reinterpret<float3>();
                foreach (var waypointTransform in authoring.waypoints)
                {
                    bufferPosition.Add(GetComponent<Transform>(waypointTransform).position);
                }
            
                AddComponent(entity, new GameData
                {
                    ShipPrefab = GetEntity(authoring.spaceShip, TransformUsageFlags.Dynamic),
                    ShipsToSpawn = (ushort)authoring.shipsToSpawn,
                    BulletPrefab = GetEntity(authoring.bulletPrefab, TransformUsageFlags.Dynamic),
                    BulletVelocity = authoring.bulletVelocity
                });
            }
        }
    }

    public struct GameData : IComponentData
    {
        public Entity ShipPrefab;
        public Entity BulletPrefab;
        public UInt16 ShipsToSpawn;
        public float BulletVelocity;
    }

    public struct PlanetsWayPointsPositions : IBufferElementData
    {
        public float3 Value ;
    }
}