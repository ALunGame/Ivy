using DG.Tweening;
using Game.Network.Client;
using IAConfig;
using IAFramework;
using IAUI;
using Proto;
using ProtoBuf;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    internal class FightBeatView_Model : UIModel
    {
        public string bgmName;
    }

    internal class FightBeatView : UIPanel<FightBeatView_Model>
    {
        private UIComGlue<Text> clickTypeTipTrans = new UIComGlue<Text>("ClickTypeTip");

        private UICacheGlue drumsPointCache = new UICacheGlue("Prefabs/DrumsPoint", "Points", true, false);

        private UIUpdateGlue updateGlue = new UIUpdateGlue();

        private UINetworkMsgGlue onGamerInputS2c;

        private float perBeatTime;
        private float audioBPM;
        private double playStartTime;
        
        private float currAudioPos;
        private int currBeat;
        private int nextBeat;
        private float nextBeatPos;

        private float beatGoCnt = 4;
        private float beatGoOffset = 70;
        private float beatGoSpeed = 0;
        private List<GameObject> beatGoList = new List<GameObject>();
        private Tween inputAnimTween;

        public override void OnAwake()
        {
            ResetAudioInfo();

            PlayBGM();

            CreateBeatGos();

            updateGlue.SetFunc(Update);

            onGamerInputS2c = new UINetworkMsgGlue(this, (ushort)RoomMsgDefine.GamerInputS2c, OnRecGamerInputS2c);
        }

        private void Update(float pDeltaTime, float pGameTime)
        {
            UpdateBeatInfo();
            UpdateDrumsPoints(pDeltaTime);
        }

        public override void OnHide()
        {
            inputAnimTween?.Kill();
        }

        private void ResetAudioInfo()
        {
            currAudioPos = 0;
            currBeat = 0;

            nextBeat = 0;
            nextBeatPos = 0;

            audioBPM = Config.FightDrumsMusicCfg[BindModel.bgmName].bpm;
            perBeatTime = 1f / (audioBPM * 1 / 60f);
        }

        private void PlayBGM()
        {
            GameEnv.Audio.PlayBGM(BindModel.bgmName);
            playStartTime = AudioSettings.dspTime;
        }

        private void CreateBeatGos()
        {
            beatGoSpeed = beatGoOffset * beatGoCnt / (perBeatTime * beatGoCnt);

            //创建节奏点
            for (int i = 0; i < beatGoCnt * 2; i++)
            {
                int tmpValue = i < beatGoCnt ? -1 : 1;

                GameObject tGo = drumsPointCache.Take();
                tGo.name = i.ToString();
                tGo.transform.localPosition = new Vector3((i % beatGoCnt) * beatGoOffset * tmpValue, 0, 0);

                beatGoList.Add(tGo);
            }

            //中间节奏提示
            Transform drumsTipTrans = transform.Find("BeatClickTip");
            drumsTipTrans.DOScaleY(1.5f, perBeatTime).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
        }

        private void UpdateBeatInfo()
        {
            currAudioPos = (float)(AudioSettings.dspTime - playStartTime);
            currBeat = (int)(currAudioPos / perBeatTime);

            nextBeat = currBeat + 1;
            nextBeatPos = nextBeat * perBeatTime;
        }

        private void UpdateDrumsPoints(float pDeltaTime)
        {
            for (int i = 0; i < 8; i++)
            {
                int tmpValue = i < 4 ? 1 : -1;
                float xDelta = tmpValue * beatGoSpeed * pDeltaTime;
                GameObject drumsGo = beatGoList[i];
                drumsGo.transform.localPosition += new Vector3(xDelta, 0, 0);

                if (Mathf.Abs(drumsGo.transform.localPosition.x) <= 6f)
                {
                    drumsGo.transform.localPosition = new Vector3(4 * beatGoOffset * tmpValue * -1, 0, 0);
                }
            }
        }


        #region 点击事件

        public MoveClickType OnClickMove()
        {
            float clickAudioPos = (float)(AudioSettings.dspTime - playStartTime);
            float clickPos = Math.Abs(nextBeatPos - clickAudioPos);
            float rangeValue = clickPos / perBeatTime;

            MoveClickType clickType;
            //普通
            if (0 < rangeValue && rangeValue < 0.3f)
            {
                clickType = MoveClickType.Normal;
            }
            //优秀
            else if (0.3f <= rangeValue && rangeValue < 0.6f)
            {
                clickType = MoveClickType.Good;
            }
            //完美
            else if (0.6f <= rangeValue && rangeValue <= 1.0f)
            {
                clickType = MoveClickType.Perfect;
            }
            //失误
            else
            {
                clickType = MoveClickType.Miss;
            }

            return clickType;
        }


        #endregion

        #region 网络消息

        private void OnRecGamerInputS2c(IExtensible pMsg)
        {
            inputAnimTween?.Kill();

            clickTypeTipTrans.Com.gameObject.SetActive(true);

            GamerInputS2c msg = (GamerInputS2c)pMsg;
            MoveClickType clickType = (MoveClickType)msg.moveClickType;

            Vector3 doScaleValue = Vector3.one;
            string clickTypeStr = "";
            switch (clickType)
            {
                case MoveClickType.Miss:
                    doScaleValue *= 0.8f;
                    clickTypeStr = "MISS";
                    break;
                case MoveClickType.Normal:
                    doScaleValue *= 1f;
                    clickTypeStr = "普通";
                    break;
                case MoveClickType.Good:
                    doScaleValue *= 1.3f;
                    clickTypeStr = "优秀";
                    break;
                case MoveClickType.Perfect:
                    doScaleValue *= 1.6f;
                    clickTypeStr = "完美";
                    break;
            }
            clickTypeTipTrans.Com.text = clickTypeStr;

            clickTypeTipTrans.Com.gameObject.SetActive(true);

            RectTransform rectTrans = clickTypeTipTrans.Com.GetComponent<RectTransform>();
            rectTrans.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            inputAnimTween = rectTrans.DOScale(doScaleValue, 0.2f).SetEase(Ease.OutElastic).OnComplete(() =>
            {
                clickTypeTipTrans.Com.gameObject.SetActive(false);
            });
        }

        #endregion
    }
}
