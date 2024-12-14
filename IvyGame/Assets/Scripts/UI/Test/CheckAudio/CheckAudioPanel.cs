using IAFramework;
using IAUI;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UITest
{
    public class CheckAudioPanel_Model : UIModel
    {
        public string clipName = "";

        /// <summary>
        /// 每分钟节拍数
        /// </summary>
        public float clipBPM;
    }

    public class CheckAudioPanel : UIPanel<CheckAudioPanel_Model>
    {

        private UIComGlue<Slider> audioSlider = new UIComGlue<Slider>("BeatProcess/Slider");
        private UIComGlue<RectTransform> clickTipTrans = new UIComGlue<RectTransform>("BeatProcess/Beats/ClickTip");
        private UIUpdateGlue updateGlue = new UIUpdateGlue();

        private double playStartTime;
        private float perBeatTime;
        private float audioBPM;

        private float currAudioPos;
        private int currBeat;
        private int nextBeat;
        private float nextBeatPos;

        private bool isPlaying;

        public override void OnAwake()
        {
            BtnUtil.SetClick(transform, "InputClick", OnClick);
            BtnUtil.SetClick(transform, "Btns/Start", OnClickPlayBtn);

            updateGlue.SetFunc(Update);
        }

        public override void OnShow()
        {
            ResetAudioInfo();
        }

        private void Update(float pDeltaTime, float pGameTime)
        {
            if (!isPlaying)
            {
                return;
            }

            currAudioPos = (float)(AudioSettings.dspTime - playStartTime);
            currBeat = (int)(currAudioPos / perBeatTime);

            nextBeat = currBeat + 1;
            nextBeatPos = nextBeat * perBeatTime;

            UpdateInfos();
            UpdateProcess();
        }

        private void UpdateInfos()
        {
            TextUtil.SetText(transform, "Infos/NextBeat/Value", nextBeat.ToString());
            TextUtil.SetText(transform, "Infos/NextBeatPos/Value", nextBeatPos.ToString());
            TextUtil.SetText(transform, "Infos/CurrPos/Value", currAudioPos.ToString());
        }

        private void UpdateProcess()
        {
            int tBeatLoopCnt = (int)(currBeat / 4.0f);
            float value = (currAudioPos - (tBeatLoopCnt * 4 * perBeatTime)) / (4 * perBeatTime);
            audioSlider.Com.value = value;
        }

        private void OnClick()
        {
            int tBeatLoopCnt = (int)(currBeat / 4.0f);
            float value = (currAudioPos - (tBeatLoopCnt * 4 * perBeatTime)) / (4 * perBeatTime);
            clickTipTrans.Com.anchoredPosition = new Vector2(value * 2000, 130);
            TextUtil.SetText(transform, "Infos/InputRange/Value", (nextBeatPos - currAudioPos).ToString());
        }

        private void OnClickPlayBtn()
        {
            GameEnv.Audio.PlayBGM(BindModel.clipName);

            playStartTime = AudioSettings.dspTime;

            ResetAudioInfo();

            isPlaying = true;
        }

        private void ResetAudioInfo()
        {
            currAudioPos = 0;
            currBeat = 0;

            nextBeat = 0;
            nextBeatPos = 0;

            audioBPM = BindModel.clipBPM;
            perBeatTime = 1f / (audioBPM * 1 / 60f);
        }
    }
}
