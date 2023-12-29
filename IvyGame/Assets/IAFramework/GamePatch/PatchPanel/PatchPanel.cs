using IAEngine;
using IAUI;
using UnityEngine.SceneManagement;
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
            gameStartDataChange.Register(GamePatchData.Instance, RefreshTips);
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
            if (GamePatchData.Instance.CurrState == EGamePatchState.Success)
            {
                Hide();
                SceneManager.LoadScene("GameScene");
                return;
            }


            if (GamePatchData.Instance.TipType == EGamePatchTipType.ProcessTips)
            {
                tipsText.Com.text = GamePatchData.Instance.TipText;
            }
            else if (GamePatchData.Instance.TipType == EGamePatchTipType.PopTips)
            {
                PatchPanel_PopPanelModel popModel = popPanel.GetPanelModel<PatchPanel_PopPanelModel>();
                popModel.titleStr = GamePatchData.Instance.TipText;
                popModel.tipStr = GamePatchData.Instance.TipText;
                if (GamePatchData.Instance.ExParams.IsLegal())
                {
                    popModel.exParam = GamePatchData.Instance.ExParams[0].ToString();
                }
                popPanel.Show();
            }
        }
    }
}
