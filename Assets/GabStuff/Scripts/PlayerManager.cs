using UnityEngine;

namespace GabStuff.Scripts
{
    public class Player
    {
        public readonly GameObject Object;
        public readonly GameObject CameraVertical;
        public readonly Camera Camera;
        public readonly FPSCamera CameraScript;
        public readonly PlayerMovement MovementScript;
        public readonly Rigidbody Rigidbody;
        public readonly Mesh Mesh;
        public readonly Material Material;
        public Vector3 Position => Object.transform.position;

        public Player(GameObject gameObject, GameObject cameraVertical)
        {
            Object = gameObject;
            CameraVertical = cameraVertical;
            Camera = gameObject.GetComponentInChildren<Camera>();
            CameraScript = gameObject.GetComponent<FPSCamera>();
            MovementScript = gameObject.GetComponent<PlayerMovement>();
            Rigidbody = gameObject.GetComponent<Rigidbody>();
            Mesh = gameObject.GetComponent<MeshFilter>().mesh;
            Material = gameObject.GetComponent<Renderer>().material;
            
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