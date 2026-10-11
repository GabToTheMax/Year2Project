using GabStuff.Scripts.Singletons;
using UnityEngine;

namespace GabStuff.Scripts
{
    public enum RegisterType
    {
        MirrorObjectParent,
        SpiritObjectParent,
        WorldHolder
    }

    public class Registerer : MonoBehaviour
    {
        [SerializeField] private RegisterType registerType;

        void Awake()
        {
            GameManager.Instance.RegisterGameObject(gameObject, registerType);
        }
    }
}