using DG.Tweening;
using Game.Network.Client;
using Gameplay;
using Gameplay.GameData;
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
        private UIComGlue<RectTransform> drumsBoxTrans = new UIComGlue<RectTransform>("Center/DrumsBox");

        private UIComGlue<Text> killInfo = new UIComGlue<Text>("Top/GameInfo/KillInfo/Num");
        private UIComGlue<Text> gameTimeInfo = new UIComGlue<Text>("Top/GameInfo/GameTime/Num");
        private UIComGlue<Text> clickTypeTipTrans = new UIComGlue<Text>("Center/DrumsBox/ClickTypeTip");

        private UICacheGlue drumsPointCache = new UICacheGlue("Prefabs/DrumsPoint", "Center/DrumsBox/Points", true);
        private UICacheGlue gamerInfoCache = new UICacheGlue("Prefabs/GamerInfo", "Top/GamerList", true);

        private UIUpdateGlue updateGlue = new UIUpdateGlue();

        private UINetworkMsgGlue OnGamerDieS2c;
        private UINetworkMsgGlue OnGamerInputS2c;

        private Tween drumsCenterTween;
        private Dictionary<GameObject, Tween> drumsTweenDict = new Dictionary<GameObject, Tween>();
        private Tween clickTypeTween;

        private LocalGamerData gamerData;

        private Vector2Int moveDir = new Vector2Int();

        public override void OnAwake()
        {
            gamerData = GameplayGlobal.Data.Gamers.GetGamer(GameplayGlobal.Data.SelfGamerUid) as LocalGamerData;

            updateGlue.SetFunc(Update);

            BtnUtil.SetClick(transform, "Left/MoveBox/UpBtn", () =>
            {
                SendMoveMsg(0, 1, 270);
            });

            BtnUtil.SetClick(transform, "Left/MoveBox/DownBtn", () =>
            {
                SendMoveMsg(0, -1, 90);
            });

            BtnUtil.SetClick(transform, "Left/MoveBox/LeftBtn", () =>
            {
                SendMoveMsg(-1, 0, 180);
            });

            BtnUtil.SetClick(transform, "Left/MoveBox/RightBtn", () =>
            {
                SendMoveMsg(1, 0, 0);
            });

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
                string clickTypeStr = "";
                switch (clickType)
                {
                    case MoveClickType.Miss:
                        clickTypeStr = "MISS";
                        break;
                    case MoveClickType.Normal:
                        clickTypeStr = "普通";
                        break;
                    case MoveClickType.Good:
                        clickTypeStr = "优秀";
                        break;
                    case MoveClickType.Perfect:
                        clickTypeStr = "完美";
                        break;
                }
                clickTypeTipTrans.Com.text = clickTypeStr;

                clickTypeTipTrans.Com.gameObject.SetActive(true);

                RectTransform rectTrans = clickTypeTipTrans.Com.GetComponent<RectTransform>();
                clickTypeTween = rectTrans.DOShakeScale(1).OnComplete(() =>
                {
                    clickTypeTipTrans.Com.gameObject.SetActive(false);
                });
            });
        }

        public override void OnShow()
        {
            CreateDrums();
            RefreshGamerInfos();
        }

        public override void OnHide()
        {
            drumsCenterTween?.Kill();
            foreach (var item in drumsTweenDict.Values)
            {
                item.Kill();
            }
            drumsTweenDict.Clear();
        }

        private void Update()
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
        }

        private void RefreshGamerInfos()
        {
            killInfo.Com.text = $"{gamerData.KillCnt.Value}/{gamerData.DieCnt.Value}";

            int maxGamerCnt = 5;
            List<GamerData> gamers = GameplayGlobal.Data.Gamers.Gamers;
            gamers.Sort((x, y) =>
            {
                if (x.KillCnt.Value > y.KillCnt.Value)
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

        #endregion

        #region 移动

        private void HandleInputMove()
        {
            if (Input.GetKeyUp(KeyCode.A))
            {
                SendMoveMsg(-1, 0, 180);
            }
            if (Input.GetKeyUp(KeyCode.D))
            {
                SendMoveMsg(1, 0, 0);
            }
            if (Input.GetKeyUp(KeyCode.W))
            {
                SendMoveMsg(0, 1, 270);
            }
            if (Input.GetKeyUp(KeyCode.S))
            {
                SendMoveMsg(0, -1, 90);
            }
        }

        private void SendMoveMsg(int x, int y, float rotate)
        {
            gamerData.SetMoveInput(new Vector2(x, y), rotate);
        }

        #endregion

        #region 鼓点

        private float drumTime = 0.3f;
        private float drumCnt = 4;
        private float drumOffset = 70;

        private void CreateDrums()
        {
            
            //左右各四个，间隔固定，向中间移动，移动到中间自动换到边缘
            for (int i = 0; i < 4; i++)
            {
                GameObject drumsGo = drumsPointCache.Take();
                HandleMoveTween(drumsGo, drumOffset * i, drumTime * i, 1);
            }

            for (int i = 0; i < 4; i++)
            {
                GameObject drumsGo = drumsPointCache.Take();
                HandleMoveTween(drumsGo, (drumOffset * i)*(-1), drumTime * i, -1);
            }


            //中间节奏提示
            Transform drumsTipTrans = drumsBoxTrans.Com.transform.Find("DrumsTip");
            drumsCenterTween = drumsTipTrans.DOScaleY(1.5f, drumTime).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
        }

        private void HandleMoveTween(GameObject pDrumsGo, float pStartX, float pMoveTime, float pDir)
        {
            if (drumsTweenDict.ContainsKey(pDrumsGo))
            {
                Tween tween = drumsTweenDict[pDrumsGo];
                DOTween.Kill(tween);
                drumsTweenDict.Remove(pDrumsGo);
            }

            pDrumsGo.transform.localPosition = new Vector3(pStartX, 6, 0);
            Tween newTween = pDrumsGo.transform.DOLocalMoveX(0, pMoveTime).OnComplete(() =>
            {
                HandleMoveTween(pDrumsGo, drumOffset * drumCnt * pDir, drumTime * drumCnt, pDir);
            }).SetEase(Ease.Linear);
            drumsTweenDict.Add(pDrumsGo, newTween);
        }

        #endregion
    }
}
