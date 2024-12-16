using Game.Network;
using Game.Network.Client;
using Gameplay;
using Gameplay.GameData;
using IAConfig;
using IAEngine;
using IAUI;
using Proto;
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

        private UIComGlue<Text> killInfo = new UIComGlue<Text>("Top/GameInfo/KillInfo/Num");
        private UIComGlue<Text> gameTimeInfo = new UIComGlue<Text>("Top/GameInfo/GameTime/Num");

        private UICacheGlue gamerInfoCache = new UICacheGlue("Prefabs/GamerInfo", "Top/GamerList", true);

        private UIUpdateGlue updateGlue = new UIUpdateGlue();

        private UINetworkMsgGlue onGamerDieS2c;

        private UIPartialPanelGlue<FightBeatView> fightBeatView = new UIPartialPanelGlue<FightBeatView>("Center/FightBeatView");

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

            InitBtnClick();

            PlayFightAudio();

            onGamerDieS2c = new UINetworkMsgGlue(this, (ushort)RoomMsgDefine.GamerDieS2c, (msgBody) =>
            {
                RefreshGamerInfos();
            });
        }

        public override void OnShow()
        {
            RefreshGamerInfos();
        }

        public override void OnHide()
        {
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
            MoveClickType clickType = fightBeatView.PartPanel.OnClickMove();
            gamerData.SetMoveInput(pMoveDir, (int)clickType);
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

        #region 战斗音乐

        public void PlayFightAudio()
        {
            FightBeatView_Model model = fightBeatView.GetPanelModel<FightBeatView_Model>();
            model.bgmName = "turkey120";
        }

        #endregion
    }
}
