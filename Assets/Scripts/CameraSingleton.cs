using UnityEngine;

namespace SpaceWars
{
    public class CameraSingleton : MonoBehaviour
    {
        public static Camera Instance;

        void Awake()
        {
            Instance = GetComponent<Camera>();
        }
    }
}
