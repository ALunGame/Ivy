using UnityEngine;

namespace Game
{
    public class GameStart : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            IAUI.UILocate.UI.Show(IAUI.UIPanelDef.MainGamePanel);
        }

        // Update is called once per frame
        void Update()
        {

        }
    } 
}
