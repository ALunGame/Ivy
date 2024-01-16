using UnityEngine;

namespace GameContext
{
    internal class GameContextInstance : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            GameContextLocate.Init();
        }

        private void OnDestroy()
        {
            GameContextLocate.Clear();
        }
    }
}
