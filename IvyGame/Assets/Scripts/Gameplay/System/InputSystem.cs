using Game.Network.Client;
using Gameplay.Player;
using Proto;
using UnityEngine;

namespace Gameplay.System
{
    internal class InputSystem : GameplaySystem
    {
        private GamePlayer localPlayer;
        private Vector2Int moveDir = new Vector2Int();
        private Vector2Int lastMoveDir = new Vector2Int();
        private PlayerMoveC2s moveMsg = new PlayerMoveC2s();

        protected override void OnStartGame()
        {
            localPlayer = GameplayLocate.GameIns.GetPlayer(NetClientLocate.LocalToken.PlayerUid);
            moveMsg.playerUid = NetClientLocate.LocalToken.PlayerUid;
            moveMsg.moveDir = new NetVector2();
        }

        protected override void OnUpdate(float pDeltaTime, float pRealElapseSeconds)
        {
            lastMoveDir = moveDir;

            if (Input.GetKeyUp(KeyCode.W))
            {
                moveDir.y = 1;
            }
            else
            {
                if (Input.GetKeyUp(KeyCode.S))
                {
                    moveDir.y = -1;
                }
                else
                {
                    moveDir.y = 0;
                }
            }

            if (Input.GetKeyUp(KeyCode.A))
            {
                moveDir.x = -1;
            }
            else
            {
                if (Input.GetKeyUp(KeyCode.D))
                {
                    moveDir.x = 1;
                }
                else
                {
                    moveDir.x = 0;
                }
            }

            
            if (moveDir != Vector2Int.zero)
            {
                moveMsg.moveDir.X = moveDir.x;
                moveMsg.moveDir.Y = moveDir.y;
                NetClientLocate.Net.Send((ushort)RoomMsgDefine.PlayerMoveC2s, moveMsg);
            }
        }
    }
}
