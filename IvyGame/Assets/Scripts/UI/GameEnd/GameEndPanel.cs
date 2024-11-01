using Gameplay;
using IAUI;

namespace Game.UI
{
    internal class GameEndPanel_Model : UIModel
    {

    }

    internal class GameEndPanel : UIPanel<FightPanel_Model>
    {
        public override void OnAwake()
        {
            BtnUtil.SetClick(transform, "Center/BackBtn", () =>
            {
                GameplayGlobal.Ctrl.ExitGame();
                UILocate.UI.Show(UIPanelDef.MainGamePanel);
            });
        }
    }
}
