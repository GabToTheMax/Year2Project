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
        
        [SerializeField] private int worldLayer;
        public int WorldLayer => worldLayer;

        [Header("Tags - Make sure they're set in the inspector tags tab")]
        
        [SerializeField] private string spiritTag;
        public string SpiritTag => spiritTag;

        [SerializeField] private string mirrorTag;
        public string MirrorTag => mirrorTag;
    }
}
