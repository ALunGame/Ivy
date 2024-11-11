using Game.Network;
using Game.Network.Client;
using Proto;
using UnityEngine;

namespace Gameplay.GameData
{
    public class LocalGamerData : GamerData
    {
        private bool firstStateReceived;
        private int lastServerTick;
        private int lastServerCommandTick;

        private GamerInputC2s inputCommand;
        private Vector2 inputMoveDir;
        private Vector2 lastInputMoveDir;
        private bool waitSyncState;

        public LocalGamerData(GamerInfo pInfo) : base(pInfo)
        {
        }

        /// <summary>
        /// 设置输入
        /// </summary>
        /// <param name="pVelocity"></param>
        /// <param name="pRotation"></param>
        public void SetInput(Vector2 pVelocity, float pRotation)
        {
            inputCommand = new GamerInputC2s();
            inputCommand.gamerUid = GamerUid;
            inputCommand.commandType = 0;

            if (pVelocity.x < -0.5f)
                inputCommand.commandType = PlayerInputCommand.Move_Left;
            if (pVelocity.x > 0.5f)
                inputCommand.commandType = PlayerInputCommand.Move_Right;
            if (pVelocity.y < -0.5f)
                inputCommand.commandType = PlayerInputCommand.Move_Up;
            if (pVelocity.y > 0.5f)
                inputCommand.commandType = PlayerInputCommand.Move_Down;

            inputCommand.Rotation = pRotation;

            NetClientLocate.Net.Send((ushort)RoomMsgDefine.GamerInputC2s, inputCommand);

            LastPosition = Position;
            LastRotation = Rotation;

            waitSyncState = true;

            lastInputMoveDir = inputMoveDir;
            inputMoveDir = Vector2.zero;

            if (inputCommand.commandType == PlayerInputCommand.Move_Down)
                inputMoveDir.y = -1f;
            if (inputCommand.commandType == PlayerInputCommand.Move_Up)
                inputMoveDir.y = 1f;
            if (inputCommand.commandType == PlayerInputCommand.Move_Left)
                inputMoveDir.x = -1f;
            if (inputCommand.commandType == PlayerInputCommand.Move_Right)
                inputMoveDir.x = 1f;

            //切换到水平移动
            if (lastInputMoveDir.x == 0 && inputMoveDir.x != 0)
            {
                Position = new Vector2(Position.x, (int)Position.y);
            }
            
            //切换到竖直移动
            if (lastInputMoveDir.y == 0 && inputMoveDir.y != 0)
            {
                Position = new Vector2((int)Position.x, Position.y);
            }
        }

        public override void UpdateLogic(float pTimeDelta, float pGameTime)
        {
            if (waitSyncState)
            {
                Position += inputMoveDir.normalized * MoveSpeed * pTimeDelta;
                if (!GameplayGlobal.Data.Map.CheckPosLegal(Position))
                {
                    Position = LastPosition;
                }
                Rotation = inputCommand.Rotation;
            }
        }

        public override void OnReceiveServerStateMsg(ServerStateS2c pServerStateMsg, GamerBaseState pMsg)
        {
            if (!LastPosition.Equals(pMsg.Pos.ToVector2()))
            {
                waitSyncState = false;
            }

            LastPosition = Position;
            LastRotation = Rotation;

            lastServerTick = pServerStateMsg.serverTick;
            lastServerCommandTick = pServerStateMsg.commandTick;

            //同步
            Position = pMsg.Pos.ToVector2();
            Rotation = pMsg.Rotation;
        }

        public virtual void UpdateInputData(GamerInputC2s pInputMsg, float pDelta)
        {
            Vector2 velocity = Vector2.zero;

            if (pInputMsg.commandType == PlayerInputCommand.Move_Down)
                velocity.y = -1f;
            if (pInputMsg.commandType == PlayerInputCommand.Move_Up)
                velocity.y = 1f;
            if (pInputMsg.commandType == PlayerInputCommand.Move_Left)
                velocity.x = -1f;
            if (pInputMsg.commandType == PlayerInputCommand.Move_Right)
                velocity.x = 1f;

            Position += velocity.normalized * MoveSpeed * pDelta;
            if (!GameplayGlobal.Data.Map.CheckPosLegal(Position))
            {
                Position = LastPosition;
            }
            Rotation = pInputMsg.Rotation;
        }
    }
}
