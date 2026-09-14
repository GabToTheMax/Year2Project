using GabStuff.Scripts.ScriptableObjects;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine;

namespace GabStuff.Scripts.Singletons
{
    public class GameManager
    {
        #region Singleton Setup
        private static GameManager _instance;

        private GameManager(){}

        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameManager(); 
                }
                return _instance;
            }
        }
        #endregion
        
        private GameSettings _gameSettings;
        
        public GameSettings GetGameSettings()
        {
            if (_gameSettings == null)
            {
                _gameSettings = 
                    Resources.Load<GameSettings>("ScriptableObjects/GameSettings");
                if (_gameSettings == null)
                    Debug.LogError("Could not load game settings");
            }
            return _gameSettings;
        }

        public void TestForMeshColliders()
        {
            bool allMeshColliders = true;
            GameObject[] allObjects = UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            foreach (GameObject o in allObjects)
            {
                if (!o.GetComponent<Collider>()) continue;
                if (o.gameObject.GetComponent<Collider>().GetType() == typeof(MeshCollider)) continue;
                if (o.gameObject.GetComponent<PortalScript>()) continue;
                Debug.LogError($"WARNING: {o.name} HAS A NON-MESH COLLIDER");
                allMeshColliders = false;
            }
            if (allMeshColliders)
                Debug.Log("GOOD: ALL GAMEOBJECTS HAVE MESH COLLIDERS");
        }
    }
}
