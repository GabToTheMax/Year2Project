using UnityEngine;

namespace GabStuff.Scripts.Singletons
{
    public class GameManager
    {
        #region Singleton Setup
        private static GameManager _instance;

        private GameManager(){}

        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameManager(); 
                }
                return _instance;
            }
        }
        #endregion
        
        private GameSettings _gameSettings;
        
        public GameSettings GetGameSettings()
        {
            if (_gameSettings == null)
            {
                _gameSettings = 
                    Resources.Load<GameSettings>("ScriptableObjects/GameSettings");
                if (_gameSettings == null)
                    Debug.LogError("Could not load game settings");
            }
            return _gameSettings;
        }
    }
}
