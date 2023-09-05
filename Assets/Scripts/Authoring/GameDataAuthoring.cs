using System;
using Unity.Entities;
using UnityEngine;

namespace SpaceWars.Authoring
{
    public class GameDataAuthoring : MonoBehaviour
    {
        public GameObject spaceShip;
        public GameObject bulletPrefab;
        public int shipsToSpawn = 500;
        public float bulletVelocity = 100f;
        public float bulletLifeTime = 1.0f;
        public float fieldOfViewAngle = 10f;
        public float distanceShootingTarget = 100;

        class Baker : Baker<GameDataAuthoring>
        {
            public override void Bake(GameDataAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new GameData
                {
                    ShipPrefab = GetEntity(authoring.spaceShip, TransformUsageFlags.Dynamic),
                    ShipsToSpawn = (ushort)authoring.shipsToSpawn,
                    BulletPrefab = GetEntity(authoring.bulletPrefab, TransformUsageFlags.Dynamic),
                    BulletVelocity = authoring.bulletVelocity,
                    BulletInitialLifeTime = authoring.bulletLifeTime,
                    FieldOfViewAngle = authoring.fieldOfViewAngle,
                    MinDistanceToTargetShooting = authoring.distanceShootingTarget
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
        public float BulletInitialLifeTime;
        public float FieldOfViewAngle;
        public float MinDistanceToTargetShooting;
    }
}