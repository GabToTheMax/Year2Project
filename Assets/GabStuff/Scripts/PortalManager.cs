using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace GabStuff.Scripts
{
    public class Portal
    {
        public readonly PortalScript PortalScript;
        public readonly GameObject Object;
        public readonly Camera Camera;
        public readonly int Index;

        public Portal(GameObject o)
        {
            Object = o;
            PortalScript = Object.GetComponent<PortalScript>();
            Index = PortalScript.index;
            Camera = Object.GetComponentInChildren<Camera>();
        }
    }
    
    public sealed class PortalManager
    {
        #region Singleton Setup
        private static PortalManager _instance;

        public PortalManager(){}

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

        public void SetPortal(Portal portal)
        {
            if (Portals[portal.Index] != null)
            {
                throw new Exception("Portals must have different indexes");
            }
            Portals[portal.Index] = portal;
        }
    }
}