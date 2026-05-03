using UnityEngine;

namespace GabStuff.Scripts
{
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "Scriptable Objects/PlayerConfig")]
    public class PlayerConfig : ScriptableObject
    {
        public float distanceToTeleport;
    }
}
