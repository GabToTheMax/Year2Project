using System;
using System.Collections.Generic;
using GabStuff.Scripts.Singletons;
using UnityEngine;

namespace GabStuff.Scripts
{
    public class PortalCollisionScript : MonoBehaviour
    {
        private Dictionary<GameObject, PortalledObject> _portalled = PortalManager.Instance.Portalled;
        private Portal _thisPortal;
        
        private void Start()
        {
            _thisPortal = GetComponent<PortalScript>().ThisPortal;
            _portalled = PortalManager.Instance.Portalled;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Teleportable>() && !_portalled.ContainsKey(other.gameObject))
            {
                _portalled.Add(other.gameObject, new PortalledObject(other.gameObject));
                print("The portalled object has been fully setup");
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (_portalled.ContainsKey(other.gameObject))
            {
                _portalled[other.gameObject].Slice(_thisPortal);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!_portalled.ContainsKey(other.gameObject)) return;
            
            _portalled[other.gameObject].Cleanup();
            _portalled.Remove(other.gameObject);
        }
    }
}
