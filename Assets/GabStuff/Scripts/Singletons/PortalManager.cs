using System;
using System.Collections.Generic;
using GabStuff.Scripts.ScriptableObjects;
using UnityEngine;
using EzySlice;
using GabStuff.Scripts.Singletons;
using UnityEngine.UIElements;
using UnityEngine.XR.ARSubsystems;
using Object = UnityEngine.Object;
using Plane = EzySlice.Plane;

namespace GabStuff.Scripts.Singletons
{
    // DONE: Add portal getters and make the array private
    // TODO: Make the set Portal thing more robust (in case setPortal fails and I try get the other portal)
    // TODO: Make the PortalManager into a monoBehaviour and start itself, and gather the portals within itself.
    // DONE: Make a PlayerManager and store player data in there.
    
    public class Portal
    {
        public readonly PortalScript Script;
        public readonly PortalTeleportationScript TeleportationScript;
        public readonly GameObject GameObject;
        public readonly Camera Camera;
        public readonly Material Material;
        public readonly Mesh Mesh;
        public readonly int Index;
        public readonly SpiritRealm SpiritRealm;
        public Vector3 Position => GameObject.transform.position;

        public Portal(GameObject o, Material m)
        {
            GameObject = o;
            Script = GameObject.GetComponent<PortalScript>();
            Index = Script.index;
            Camera = GameObject.GetComponentInChildren<Camera>();
            Material = m;
            Mesh = GameObject.GetComponentInChildren<MeshFilter>().mesh;
            SpiritRealm = GameObject.GetComponent<SpiritRealm>();
            TeleportationScript = GameObject.GetComponent<PortalTeleportationScript>();
        }
    }
    
    public sealed class PortalManager
    {
        #region Singleton Setup
        private static PortalManager _instance;

        private PortalManager(){}

        public static PortalManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PortalManager(); 
                }
                return _instance;
            }
        }
        #endregion

        public readonly Portal[] Portals = new Portal[2];
        public readonly Dictionary<GameObject, MirrorObject> Mirrors = new();
        public readonly Dictionary<GameObject, PortalledObject> Portalled = new();
        private GameSettings _gameSettings;
        public GameObject MirrorObjectParent { get; private set; }
        public GameObject SpiritObjectParent { get; private set; }

        public void SetPortal(Portal portal)
        {
            if (Portals[portal.Index] != null)
            {
                throw new Exception("Portals must have different indexes");
            }
            Portals[portal.Index] = portal;
        }

        /// <summary>
        /// Return the other portal
        /// </summary>
        /// <param name="thisPortal">Reference to the portal calling the function</param>
        public Portal GetPortal(Portal thisPortal)
        {
            Portal otherPortal = Portals[1-thisPortal.Index];
            if (otherPortal != null)
            {
                return otherPortal;
            }

            return null;
        }

        public void RegisterObject(GameObject go, RegisterType type)
        {
            switch (type)
            {
                case RegisterType.MirrorObjectParent:
                    MirrorObjectParent = go;
                    break;
                case RegisterType.SpiritObjectParent:
                    SpiritObjectParent = go;
                    break;
                default:
                    throw new Exception($"Portal manager only accepts Mirror and Spirit parents");
            }
        }
        
        public static Mesh ScaleMesh(Mesh mesh, Vector3 scale)
        {
            if (mesh.vertices.Length == 0) return mesh;
            Vector3[] vertices = mesh.vertices;

            for (var i = 0; i < vertices.Length; i++)
            {
                vertices[i] = Vector3.Scale(mesh.vertices[i], scale);
            }

            Mesh newMesh = Object.Instantiate(mesh);
            newMesh.vertices = vertices;
            newMesh.RecalculateBounds();
            
            return newMesh;
        }
    }
}