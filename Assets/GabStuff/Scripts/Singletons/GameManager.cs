using System;
using System.Collections.Generic;
using GabStuff.Scripts.ScriptableObjects;
using Unity.VectorGraphics;
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
        public GameObject WorldHolder { get; private set; }

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

        public void RegisterGameObject(GameObject go, RegisterType registerType)
        {
            switch (registerType)
            {
                case RegisterType.MirrorObjectParent:
                    PortalManager.Instance.RegisterObject(go, registerType);
                    break;
                
                case RegisterType.SpiritObjectParent:
                    PortalManager.Instance.RegisterObject(go, registerType);
                    break;
                
                case RegisterType.WorldHolder:
                    WorldHolder = go;
                    break;
                
                default:
                    Debug.LogError($"Unknown registerType {registerType}");
                    break;
            }
        }
        
        public GameObject[] GetEveryObject()
        {
            if (WorldHolder == null)
            {
                Debug.LogError("Missing world holder");
                EditorApplication.isPlaying = false;
                Application.Quit();
            }
            Transform[] transforms = WorldHolder.GetComponentsInChildren<Transform>(false);
            GameObject[] gameObjects = new GameObject[transforms.Length];
            for (var i = 0; i < transforms.Length; i++)
            {
                gameObjects[i] = transforms[i].gameObject;
            }
            return gameObjects;
        }
    }
}
