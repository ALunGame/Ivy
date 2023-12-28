using IAEngine;
using IAUI;
using UnityEngine.UI;

namespace IAFramework.UI
{
    public class PatchPanelModel : UIModel
    {

    }

    public class PatchPanel : UIPanel<PatchPanelModel>
    {
        private UIComGlue<Text> tipsText = new UIComGlue<Text>("Bottom/Tips");
        private UIComGlue<Slider> processSlider = new UIComGlue<Slider>("Bottom/Slider");

        private UIPartialPanelGlue<PatchPanel_PopPanel> popPanel = new UIPartialPanelGlue<PatchPanel_PopPanel>("Center/PopTipPanel", false);

        private UIUserDataChangeGlue gameStartDataChange = new UIUserDataChangeGlue();

        public override void OnAwake()
        {
            InitPanel();
        }

        public override void OnShow()
        {
            gameStartDataChange.Register(GameStartData.Instance, RefreshTips);
        }

        public override void OnHide()
        {
            popPanel.Hide();
        }

        private void InitPanel()
        {
            tipsText.Com.text = "";
            processSlider.Com.value = 0;
        }

        private void RefreshTips()
        {
            if (GameStartData.Instance.TipType == EGameStartTipType.ProcessTips)
            {
                tipsText.Com.text = GameStartData.Instance.TipText;
            }
            else if (GameStartData.Instance.TipType == EGameStartTipType.PopTips)
            {
                PatchPanel_PopPanelModel popModel = popPanel.GetPanelModel<PatchPanel_PopPanelModel>();
                popModel.titleStr = GameStartData.Instance.TipText;
                popModel.tipStr = GameStartData.Instance.TipText;
                if (GameStartData.Instance.ExParams.IsLegal())
                {
                    popModel.exParam = GameStartData.Instance.ExParams[0].ToString();
                }
                popPanel.Show();
            }
        }
    }
}
