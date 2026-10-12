using UnityEngine;

namespace GabStuff.Scripts.Singletons
{
    public class Player
    {
        public readonly GameObject GameObject;
        public readonly GameObject CameraVertical;
        public readonly Camera Camera;
        public readonly FPSCamera CameraScript;
        public readonly PlayerMovement MovementScript;
        public readonly Rigidbody Rigidbody;
        public readonly Mesh Mesh;
        public readonly Material Material;
        public readonly Collider Collider;
        public readonly PlayerGrabber PlayerGrabber;
        public Vector3 Position => GameObject.transform.position;

        public Player(GameObject gameGameObject, GameObject cameraVertical)
        {
            GameObject = gameGameObject;
            CameraVertical = cameraVertical;
            Camera = gameGameObject.GetComponentInChildren<Camera>();
            CameraScript = gameGameObject.GetComponent<FPSCamera>();
            MovementScript = gameGameObject.GetComponent<PlayerMovement>();
            Rigidbody = gameGameObject.GetComponent<Rigidbody>();
            Mesh = gameGameObject.GetComponent<MeshFilter>().mesh;
            Material = gameGameObject.GetComponent<Renderer>().material;
            Collider = gameGameObject.GetComponent<Collider>();
            PlayerGrabber = gameGameObject.GetComponent<PlayerGrabber>();
        }
    }
    
    public class PlayerManager
    {
        #region Singleton Setup
        private static PlayerManager _instance;
        
        private PlayerManager(){}

        public static PlayerManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PlayerManager();
                }
                return _instance;
            }
        }
        #endregion

        private Player _player;

        public Player GetPlayer()
        {
            return _player;
        }

        public void SetPlayer(Player playerToSet)
        {
            if (_player == null)
            {
                _player = playerToSet;
            }
        }
    }
}