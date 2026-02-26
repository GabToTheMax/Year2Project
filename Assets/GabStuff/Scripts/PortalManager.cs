using System.Collections.Generic;
using UnityEngine;

namespace GabStuff.Scripts
{
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

        public readonly GameObject[] PortalObjects = new GameObject[2];
        public readonly PortalScript[] PortalScripts = new PortalScript[2];
        
        public void SetPortal(GameObject portal)
        {
            var portalScript = portal.GetComponent<PortalScript>();
            var index = portalScript.portalIndex;
            PortalObjects[index] = portal;
            PortalScripts[index] = portalScript;
        }
        
        
    }
}