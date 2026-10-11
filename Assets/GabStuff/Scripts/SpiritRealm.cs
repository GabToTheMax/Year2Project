using System.Collections.Generic;
using GabStuff.Scripts.Singletons;
using UnityEngine;

namespace GabStuff.Scripts
{
    public class SpiritRealm : MonoBehaviour
    {
        private Dictionary<GameObject, SpiritObject> _spiritObjects;
        private Portal _thisPortal;
        private Portal _otherPortal;
        private int _spiritRealmLayer;
        private int _worldLayer;

        private void Start()
        {
            GameManager.Instance.TestForMeshColliders();
            _thisPortal = GetComponent<PortalScript>().ThisPortal;
            _spiritObjects = new Dictionary<GameObject, SpiritObject>();
            _spiritRealmLayer = GameManager.Instance.GetGameSettings().SpiritRealmLayer;
            _worldLayer = GameManager.Instance.GetGameSettings().WorldLayer;
            Physics.IgnoreLayerCollision(_spiritRealmLayer, _worldLayer, true);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == GameManager.Instance.GetGameSettings().SpiritTag || _spiritObjects.ContainsKey(other.gameObject)) return;
            
            var original = other.gameObject;
            
            _spiritObjects.Add(original, new SpiritObject(other.gameObject, _thisPortal));
        }

        private void MoveSpiritObject(GameObject original)
        {
            Vector3 objectPosition = original.gameObject.transform.position;
            Quaternion objectRotation = original.gameObject.transform.rotation;
                
            _spiritObjects[original.gameObject].TransformSpiritObject(objectPosition-_thisPortal.Position, objectRotation, _thisPortal);
        }

        private void OnTriggerStay(Collider other)
        {
            if (_spiritObjects.ContainsKey(other.gameObject))
            {
                MoveSpiritObject(other.gameObject);
                _spiritObjects[other.gameObject].SliceMeshAtPortal(_thisPortal);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_spiritObjects.ContainsKey(other.gameObject))
            {
                _spiritObjects[other.gameObject].Cleanup();
                _spiritObjects.Remove(other.gameObject);
            }
        }
    }
}
