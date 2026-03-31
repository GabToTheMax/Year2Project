using System;
using UnityEngine;

namespace GabStuff.Scripts
{
    // DONE: Add portal getters and make the array private
    // TODO: Make the set Portal thing more robust (in case setPortal fails and I try get the other portal)
    // TODO: Make the PortalManager into a monoBehaviour and start itself, and gather the portals within itself.
    // DONE: Make a PlayerManager and store player data in there.
    
    public class Portal
    {
        public readonly PortalScript Script;
        public readonly GameObject Object;
        public readonly Camera Camera;
        public readonly Material Material;
        public readonly Mesh Mesh;
        public readonly int Index;
        public Vector3 Position => Object.transform.position;

        public Portal(GameObject o, Material m)
        {
            Object = o;
            Script = Object.GetComponent<PortalScript>();
            Index = Script.index;
            Camera = Object.GetComponentInChildren<Camera>();
            Material = m;
            Mesh = Object.GetComponentInChildren<MeshFilter>().mesh;
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

        private readonly Portal[] _portals = new Portal[2];

        public void SetPortal(Portal portal)
        {
            if (_portals[portal.Index] != null)
            {
                throw new Exception("Portals must have different indexes");
            }
            _portals[portal.Index] = portal;
        }

        /// <summary>
        /// Return the other portal
        /// </summary>
        /// <param name="thisPortal">Reference to the portal calling the function</param>
        public Portal GetPortal(Portal thisPortal)
        {
            Portal otherPortal = _portals[1-thisPortal.Index];
            if (otherPortal != null)
            {
                return otherPortal;
            }

            return null;
        }

        public Portal[] GetPortals()
        {
            return _portals;
        }
    }
}