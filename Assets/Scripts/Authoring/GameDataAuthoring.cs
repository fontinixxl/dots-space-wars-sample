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
        public int shipsToSpawn = 500;

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
            
                AddComponent(entity, new SpaceShipBakingData
                {
                    ShipPrefab = GetEntity(authoring.spaceShip, TransformUsageFlags.Dynamic),
                    ShipsToSpawn = (ushort)authoring.shipsToSpawn
                });
            }
        }
    }

    public struct SpaceShipBakingData : IComponentData
    {
        public Entity ShipPrefab;
        public UInt16 ShipsToSpawn;
    }

    public struct PlanetsWayPointsPositions : IBufferElementData
    {
        public float3 Value ;
    }
}