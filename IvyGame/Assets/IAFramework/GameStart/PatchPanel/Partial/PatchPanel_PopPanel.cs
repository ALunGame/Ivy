using IAUI;
using UnityEngine.UI;

namespace IAFramework.UI
{
    public class PatchPanel_PopPanelModel : UIModel
    {
        public string titleStr;
        public string tipStr;

        public string exParam;
    }

    public class PatchPanel_PopPanel : UIPanel<PatchPanel_PopPanelModel>
    {
        private UIComGlue<Text> titleText = new UIComGlue<Text>("Center/Panel/Img_Title/Title");
        private UIComGlue<Text> tipsText = new UIComGlue<Text>("Center/Panel/Tips");

        private UICacheGlue btnCache = new UICacheGlue("Cache/Btn", "Center/Panel/BtnList", true);

        public override void OnShow()
        {
            titleText.Com.text = BindModel.titleStr;
            tipsText.Com.text = BindModel.tipStr;
        }
    }
}
