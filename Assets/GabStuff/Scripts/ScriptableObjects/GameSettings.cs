using UnityEngine;

namespace GabStuff.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Scriptable Objects/GameSettings")]
    public class GameSettings : ScriptableObject
    {
        [SerializeField] private Shader behindPortal1Shader;
        public Shader BehindPortal1Shader => behindPortal1Shader;
        
        [SerializeField] private Shader behindPortal2Shader;
        public Shader BehindPortal2Shader => behindPortal2Shader;
        
        [SerializeField] private int spiritRealmLayer;
        public int SpiritRealmLayer => spiritRealmLayer;
        
        [SerializeField] private int playerLayer;
        public int PlayerLayer => playerLayer;
        
        [SerializeField] private int grabbedObjectLayer;
        public int GrabbedObjectLayer => grabbedObjectLayer;
    }
}
