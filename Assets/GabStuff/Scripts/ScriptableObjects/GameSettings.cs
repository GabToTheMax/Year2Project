using System.Collections.Generic;
using UnityEngine;

namespace GabStuff.Scripts
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Scriptable Objects/GameSettings")]
    public class GameSettings : ScriptableObject
    {
        [SerializeField] private Shader behindPortal1Shader;
        public Shader BehindPortal1Shader => behindPortal1Shader;
        
        [SerializeField] private Shader behindPortal2Shader;
        public Shader BehindPortal2Shader => behindPortal2Shader;
    }
}
