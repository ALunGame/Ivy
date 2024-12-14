using Game.Network.Client;
using Game.UITest;
using Gameplay;
using IAUI;
using Proto;
using UnityEngine;

namespace Game.UI
{
    public class MainGamePanel_Model : UIModel
    { 

    }

    public class MainGamePanel : UIPanel<MainGamePanel_Model>
    {
        private UIComGlue<RectTransform> localBtn = new UIComGlue<RectTransform>("Center/GameMode/Local");
        private UIComGlue<RectTransform> singelBtn = new UIComGlue<RectTransform>("Center/GameMode/Single");
        private UIComGlue<RectTransform> teamBtn = new UIComGlue<RectTransform>("Center/GameMode/Team");

        private UIComGlue<RectTransform> chooseConnectRoot = new UIComGlue<RectTransform>("Center/ChooseConnectMode");


        private GameModeType currGameMode = GameModeType.Local;

        public override void OnAwake()
        {
            BtnUtil.SetClick(transform, "Center/GameMode/Local", () =>
            {
                CheckAudioPanel_Model model = UILocate.UI.GetPanelModel<CheckAudioPanel_Model>(UIPanelDef.Test_CheckAudioPanel);
                model.clipName = "turkey120";
                model.clipBPM = 120;

                UILocate.UI.Show(UIPanelDef.Test_CheckAudioPanel);
            });

            BtnUtil.SetClick(transform, "Center/GameMode/Single", () =>
            {
                OnClickGameMode(GameModeType.Single);
            });

            BtnUtil.SetClick(transform, "Center/GameMode/Team", () =>
            {
                OnClickGameMode(GameModeType.Team);
            });

            BtnUtil.SetClick(transform, "Center/ChooseConnectMode/ChooseBox/Server", () =>
            {
                OnClickConnectMode(true);
            });

            BtnUtil.SetClick(transform, "Center/ChooseConnectMode/ChooseBox/Client", () =>
            {
                OnClickConnectMode(false);
            });

            chooseConnectRoot.Com.gameObject.SetActive(false);
        }


        private void OnClickGameMode(GameModeType modeType)
        {
            currGameMode = modeType;

            chooseConnectRoot.Com.gameObject.SetActive(modeType != GameModeType.Local);

            if (modeType == GameModeType.Local)
            {
                //GameplayLocate.CreateGame(modeType, true);
                Close();
            }
            else if (modeType == GameModeType.Single)
            {
                chooseConnectRoot.Com.anchoredPosition = new Vector2(480, singelBtn.Com.anchoredPosition.y);
            }
            else if (modeType == GameModeType.Team)
            {
                chooseConnectRoot.Com.anchoredPosition = new Vector2(480, teamBtn.Com.anchoredPosition.y);
            }
        }

        private void OnClickConnectMode(bool isCreateRoom)
        {
            GameplayCtrl.Instance.CreateGame(currGameMode, isCreateRoom);
        }

        private UINetworkMsgGlue OnJoinRoomMsg = new UINetworkMsgGlue((ushort)RoomMsgDefine.JoinRoomS2c, (msgBody) =>
        {
            UILocate.UI.Show(UIPanelDef.RoomPanel);
        });
    } 
}
