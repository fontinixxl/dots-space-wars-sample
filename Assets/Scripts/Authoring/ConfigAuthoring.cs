using System;
using Unity.Entities;
using UnityEngine;

namespace SpaceWars.Authoring
{
    public class ConfigAuthoring : MonoBehaviour
    {
        public GameObject spaceShip;
        public GameObject bulletPrefab;
        public GameObject explosionPrefab;
        public int shipsToSpawn = 500;
        public float bulletVelocity = 100f;
        public float bulletLifeTime = 1.0f;
        public float fieldOfViewAngle = 10f;
        public float distanceShootingTarget = 100;
        public float explosionProbability = 0.3f;

        class Baker : Baker<ConfigAuthoring>
        {
            public override void Bake(ConfigAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new Config
                {
                    ShipPrefab = GetEntity(authoring.spaceShip, TransformUsageFlags.Dynamic),
                    ShipsToSpawn = (ushort)authoring.shipsToSpawn,
                    BulletPrefab = GetEntity(authoring.bulletPrefab, TransformUsageFlags.Dynamic),
                    BulletVelocity = authoring.bulletVelocity,
                    BulletInitialLifeTime = authoring.bulletLifeTime,
                    FieldOfViewAngle = authoring.fieldOfViewAngle,
                    MinDistanceToTargetShooting = authoring.distanceShootingTarget,
                    ExplosionPrefab = GetEntity(authoring.explosionPrefab, TransformUsageFlags.Dynamic),
                    ExplosionProbability = authoring.explosionProbability
                });
            }
        }
    }

    public struct Config : IComponentData
    {
        public Entity ShipPrefab;
        public Entity BulletPrefab;
        public Entity ExplosionPrefab;
        public UInt16 ShipsToSpawn;
        public float BulletVelocity;
        public float BulletInitialLifeTime;
        public float FieldOfViewAngle;
        public float MinDistanceToTargetShooting;
        public float ExplosionProbability;
    }
}