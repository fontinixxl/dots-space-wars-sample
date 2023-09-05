using SpaceWars.Authoring;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace SpaceWars.Systems
{
    public partial struct CameraFollowSystem : ISystem
    {
        private Entity _target;
        private Random _random;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _random = new Random(321);
        }

        public void OnUpdate(ref SystemState state)
        {
            if (_target == Entity.Null || Input.GetKeyDown(KeyCode.Space))
            {
                var spaceShipQuery = SystemAPI.QueryBuilder().WithAll<ShipData>().Build();
                var spaceShips = spaceShipQuery.ToEntityArray(Allocator.Temp);
                if (spaceShips.Length == 0) return;

                _target = spaceShips[_random.NextInt(spaceShips.Length)];
            }
            
            var cameraTransform = CameraSingleton.Instance.transform;
            var spaceShipTransform = SystemAPI.GetComponent<LocalToWorld>(_target);
            cameraTransform.position = spaceShipTransform.Position;
            cameraTransform.position -= 20.0f * (Vector3)spaceShipTransform.Forward;  // move the camera back from the SpaceShip
            cameraTransform.position += new Vector3(0, 5, 0);  // raise the camera by an offset
            cameraTransform.LookAt(spaceShipTransform.Position);
        }
    }
}