using IAFramework;
using IAUI;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    internal class GameSettingPanel_Model : UIModel
    {

    }

    internal class GameSettingPanel : UIPanel<GameSettingPanel_Model>
    {
        private UIComGlue<Slider> globalVolumeSlider = new UIComGlue<Slider>("Center/SettingBox/AudioSetBox/GlobalBox/GlobalVolumeSlider");
        private UIComGlue<Toggle> globalAudioToggle = new UIComGlue<Toggle>("Center/SettingBox/AudioSetBox/GlobalBox/AudioOpen");
        private UIComGlue<Slider> bgmVolumeSlider = new UIComGlue<Slider>("Center/SettingBox/AudioSetBox/BGMBox/BGMVolumeSlider");
        private UIComGlue<Slider> audioVolumeSlider = new UIComGlue<Slider>("Center/SettingBox/AudioSetBox/AudioBox/AudioVolumeSlider");

        private UICacheGlue btnCache = new UICacheGlue("Center/SettingBox/Prefabs/BtnItem", "Center/SettingBox/BtnBox", true, false);

        public override void OnAwake()
        {
            BtnUtil.SetClick(transform, "Mask", OnClickCloseBtn);
            BtnUtil.SetClick(transform, "Center/SettingBox/CloseBtn", OnClickCloseBtn);
            InitAudioBtnEvents();
            InitBtns();
        }

        public override void OnShow()
        {
            RefreshAudioInfo();
        }

        private void RefreshAudioInfo()
        {
            globalVolumeSlider.Com.value = GameEnv.Audio.GlobalVolume;
            globalAudioToggle.Com.isOn = GameEnv.Audio.IsMute;
            bgmVolumeSlider.Com.value = GameEnv.Audio.GlobalVolume;
            audioVolumeSlider.Com.value = GameEnv.Audio.GlobalVolume;
        }

        #region 按钮事件

        private void InitAudioBtnEvents()
        {
            globalVolumeSlider.Com.onValueChanged.AddListener((pValue) =>
            {
                GameEnv.Audio.GlobalVolume = pValue;
            });

            globalAudioToggle.Com.onValueChanged.AddListener((pValue) =>
            {
                GameEnv.Audio.IsMute = pValue;
            });

            bgmVolumeSlider.Com.onValueChanged.AddListener((pValue) =>
            {
                GameEnv.Audio.BGVolume = pValue;
            });

            audioVolumeSlider.Com.onValueChanged.AddListener((pValue) =>
            {
                GameEnv.Audio.AudioVolume = pValue;
            });
        }

        private void InitBtns()
        {
            //离开
            GameObject exitBtn = btnCache.Take();
            TextUtil.SetText(exitBtn.transform, "Txt", "离开");
            BtnUtil.SetClick(btnCache.Take().transform, null, () =>
            {
                Application.Quit();
            });
        }

        private void OnClickCloseBtn()
        {
            UILocate.UI.Hide(UIPanelDef.GameSettingPanel);
        }

        #endregion
    }
}
