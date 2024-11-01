using GameContext;
using Gameplay;
using IAUI;

namespace Game.UI
{
    public class StartGamePanel_Model : UIModel
    {

    }

    public class StartGamePanel : UIPanel<StartGamePanel_Model>
    {
        public override void OnAwake()
        {
            BtnUtil.SetClick(transform, "Center/StartGame", () =>
            {
            });

            BtnUtil.SetClick(transform, "Center/DelRecord", () =>
            {
                GameContextLocate.DelRecord();
            });

            BtnUtil.SetClick(transform, "Center/ExitGame", () =>
            {

            });
        }
    }
}
