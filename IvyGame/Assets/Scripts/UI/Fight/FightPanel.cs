using DG.Tweening;
using Game.Network;
using Game.Network.Client;
using Gameplay;
using Gameplay.GameData;
using IAConfig;
using IAEngine;
using IAUI;
using Proto;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    internal class FightPanel_Model : UIModel
    {

    }

    internal class FightPanel : UIPanel<FightPanel_Model>
    {
        private UIComGlue<RectTransform> gameInfoTrans = new UIComGlue<RectTransform>("Top/GameInfo");
        private UIComGlue<RectTransform> moveBoxTrans = new UIComGlue<RectTransform>("Left/MoveBox");
        private UIComGlue<RectTransform> skillBoxTrans = new UIComGlue<RectTransform>("Right/SkillBox");
        private UIComGlue<RectTransform> drumsBoxTrans = new UIComGlue<RectTransform>("Center/DrumsBox");

        private UIComGlue<Text> killInfo = new UIComGlue<Text>("Top/GameInfo/KillInfo/Num");
        private UIComGlue<Text> gameTimeInfo = new UIComGlue<Text>("Top/GameInfo/GameTime/Num");
        private UIComGlue<Text> clickTypeTipTrans = new UIComGlue<Text>("Center/DrumsBox/ClickTypeTip");

        private UICacheGlue drumsPointCache = new UICacheGlue("Prefabs/DrumsPoint", "Center/DrumsBox/Points", true, false);
        private UICacheGlue gamerInfoCache = new UICacheGlue("Prefabs/GamerInfo", "Top/GamerList", true);

        private UIUpdateGlue updateGlue = new UIUpdateGlue();

        private UINetworkMsgGlue OnGamerDieS2c;
        private UINetworkMsgGlue OnGamerInputS2c;

        private float drumTime = 0.3f;
        private float drumCnt = 4;
        private float drumOffset = 70;
        private float drumSpeed = 0;
        private List<GameObject> drumsGoList = new List<GameObject>();

        private Tween drumsCenterTween;

        private Tween clickTypeTween;

        private TimerModel dashTimer;
        private bool isDashInCd;

        private LocalGamerData gamerData;

        private Vector2Int moveDir = new Vector2Int();

        public override void OnAwake()
        {
            //冲刺配置
            dashTimer = new TimerModel(GetType(), int.Parse(Config.MiscCfg["DashCdTime"].value), () =>
            {
                TextUtil.SetText(transform, "Right/SkillBox/DashBtn/Txt", "冲刺");
                isDashInCd = false;
            }, 1);
            gamerData = GameplayGlobal.Data.Gamers.GetGamer(GameplayGlobal.Data.SelfGamerUid) as LocalGamerData;

            updateGlue.SetFunc(Update);

            InitDrums();
            InitBtnClick();

            OnGamerDieS2c = new UINetworkMsgGlue(this, (ushort)RoomMsgDefine.GamerDieS2c, (msgBody) =>
            {
                RefreshGamerInfos();
            });

            OnGamerInputS2c = new UINetworkMsgGlue(this, (ushort)RoomMsgDefine.GamerInputS2c, (msgBody) =>
            {
                clickTypeTween?.Kill();

                clickTypeTipTrans.Com.gameObject.SetActive(true);

                GamerInputS2c msg = (GamerInputS2c)msgBody;
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
                clickTypeTween = rectTrans.DOScale(doScaleValue, 0.2f).SetEase(Ease.OutElastic).OnComplete(() =>
                {
                    clickTypeTipTrans.Com.gameObject.SetActive(false);
                });
            });
        }

        public override void OnShow()
        {
            //CreateDrums();
            RefreshGamerInfos();
        }

        public override void OnHide()
        {
            drumsCenterTween?.Kill();
        }

        private void InitDrums()
        {
            drumTime = gamerData.DrumsTime;
            drumSpeed = drumOffset * drumCnt / (drumTime * drumCnt);

            //创建节奏点
            for (int i = 0; i < drumCnt * 2; i++)
            {
                int tmpValue = i < drumCnt ? -1 : 1;

                GameObject tGo = drumsPointCache.Take();
                tGo.name = i.ToString();
                tGo.transform.localPosition = new Vector3((i % drumCnt) * drumOffset * tmpValue, 0, 0);

                drumsGoList.Add(tGo);
            }

            //中间节奏提示
            Transform drumsTipTrans = drumsBoxTrans.Com.transform.Find("DrumsTip");
            drumsCenterTween = drumsTipTrans.DOScaleY(1.5f, drumTime).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
        }

        private void InitBtnClick()
        {
            BtnUtil.SetClick(transform, "Left/MoveBox/UpBtn", () =>
            {
                SendMoveMsg(PlayerInputCommand.Move_Up);
            });

            BtnUtil.SetClick(transform, "Left/MoveBox/DownBtn", () =>
            {
                SendMoveMsg(PlayerInputCommand.Move_Down);
            });

            BtnUtil.SetClick(transform, "Left/MoveBox/LeftBtn", () =>
            {
                SendMoveMsg(PlayerInputCommand.Move_Left);
            });

            BtnUtil.SetClick(transform, "Left/MoveBox/RightBtn", () =>
            {
                SendMoveMsg(PlayerInputCommand.Move_Right);
            });

            BtnUtil.SetClick(transform, "Right/SkillBox/DashBtn", () =>
            {
                SendDaskSkillMsg();
            });
        }

        private void Update(float pDeltaTime, float pGameTime)
        {
            Refresh();
            HandleInputMove();
            UpdateDrumsPoints(pDeltaTime);
        }

        private void Refresh()
        {
            RefreshGameInfo();
        }

        #region 游戏信息

        private void RefreshGameInfo()
        {
            gameTimeInfo.Com.text = $"{(int)GameplayGlobal.Ctrl.GameTime}s";

            RefreshDashCd();
        }

        private void RefreshGamerInfos()
        {
            killInfo.Com.text = $"{gamerData.KillCnt.Value}/{gamerData.DieCnt.Value}";

            int maxGamerCnt = 5;
            List<GamerData> gamers = GameplayGlobal.Data.Gamers.Gamers;
            gamers.Sort((x, y) =>
            {
                if (x.KillCnt.Value >= y.KillCnt.Value)
                    return 1;
                else
                    return -1;
            });

            gamerInfoCache.RecycleAll();
            int gamerCnt = gamers.Count > maxGamerCnt ? maxGamerCnt : gamers.Count;
            for (int i = 0; i < gamerCnt; i++)
            {
                GamerData gamer = gamers[i];
                GameObject gamerGo = gamerInfoCache.Take();
                TextUtil.SetText(gamerGo.transform, "Name", gamer.Name);
                TextUtil.SetText(gamerGo.transform, "KillCnt", gamer.KillCnt.Value.ToString());
            }
        }

        private void RefreshDashCd()
        {
            if (!isDashInCd)
            {
                return;
            }

            dashTimer.Update(Time.deltaTime);
            float cdTime = dashTimer.GetCdTime();
            TextUtil.SetText(transform, "Right/SkillBox/DashBtn/Txt", $"{cdTime}/S");
        }

        #endregion

        #region 移动

        private void HandleInputMove()
        {
            if (Input.GetKeyUp(KeyCode.A))
            {
                SendMoveMsg(PlayerInputCommand.Move_Left);
            }
            if (Input.GetKeyUp(KeyCode.D))
            {
                SendMoveMsg(PlayerInputCommand.Move_Right);
            }
            if (Input.GetKeyUp(KeyCode.W))
            {
                SendMoveMsg(PlayerInputCommand.Move_Up);
            }
            if (Input.GetKeyUp(KeyCode.S))
            {
                SendMoveMsg(PlayerInputCommand.Move_Down);
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                SendDaskSkillMsg();
            }
        }

        private void SendMoveMsg(int pMoveDir)
        {
            gamerData.SetMoveInput(pMoveDir);
        }

        private void SendDaskSkillMsg()
        {
            if (isDashInCd)
            {
                return;
            }

            isDashInCd = true;
            dashTimer.Start();

            GamerSkillInputC2s msg = new GamerSkillInputC2s();
            msg.gamerUid = gamerData.GamerUid;
            msg.skillId = 1;
            NetClientLocate.Net.Send((ushort)RoomMsgDefine.GamerSkillInputC2s, msg);
        }

        #endregion

        #region 鼓点

        private void UpdateDrumsPoints(float pDeltaTime)
        {
            for (int i = 0; i < 8; i++)
            {
                int tmpValue = i < 4 ? 1 : -1;
                float xDelta = tmpValue * drumSpeed * pDeltaTime;
                GameObject drumsGo = drumsGoList[i];
                drumsGo.transform.localPosition += new Vector3(xDelta, 0, 0);

                if (Mathf.Abs(drumsGo.transform.localPosition.x) <= 6f)
                {
                    drumsGo.transform.localPosition = new Vector3(4 * drumOffset * tmpValue * -1, 0, 0);
                }
            }
        }

        #endregion
    }
}
