using System.Collections.Generic;
using GabStuff.Scripts.Singletons;
using UnityEngine;

namespace GabStuff.Scripts
{
    public class NewMonoBehaviourScript : MonoBehaviour
    {
        private Dictionary<GameObject, SpiritObject> _spiritObjects;
        private Portal _thisPortal;
        private Portal _otherPortal;

        private void Start()
        {
            GameManager.Instance.TestForMeshColliders();
            _thisPortal = GetComponent<PortalScript>().ThisPortal;
            _spiritObjects = new Dictionary<GameObject, SpiritObject>();
            int spiritRealmLayer = GameManager.Instance.GetGameSettings().SpiritRealmLayer;
            int playerLayer = GameManager.Instance.GetGameSettings().PlayerLayer;
            int grabbedObjectLayer = GameManager.Instance.GetGameSettings().GrabbedObjectLayer;
            Physics.IgnoreLayerCollision(spiritRealmLayer, playerLayer, true);
            Physics.IgnoreLayerCollision(spiritRealmLayer, grabbedObjectLayer, true);
            Physics.IgnoreLayerCollision(spiritRealmLayer, 0, true);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<Teleportable>())
            {
                _spiritObjects.Add(other.gameObject, new SpiritObject(other.gameObject, _thisPortal));
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (_spiritObjects.ContainsKey(other.gameObject))
            {
                SpiritObject currentSpiritObject = _spiritObjects[other.gameObject];
                Vector3 objectPosition = other.gameObject.transform.position;
                Quaternion objectRotation = other.gameObject.transform.rotation;
                
                currentSpiritObject.TransformSpiritObject(objectPosition-_thisPortal.Position, objectRotation, _thisPortal);
                currentSpiritObject.SliceMeshAtPortal(_thisPortal);
            }
        }
    }
}
