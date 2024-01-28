using Game.Network;
using Proto;
using TMPro;
using UnityEngine;

namespace Gameplay.Player
{

    internal class AddGamePlayerInfo
    {
        public int uid;
        public int id;
        public string name;
        public byte camp;
        public byte PosX;
        public byte PosY;
        public bool IsLocalPlayer;

        public AddGamePlayerInfo(int pUid, int pId, string pName, byte pCamp)
        {
            uid = pUid;
            id = pId;
            name = pName;
            camp = pCamp;
        }
    }

    /// <summary>
    /// 游戏玩法玩家
    /// 1，负责表现
    /// </summary>
    internal class GamePlayer : MonoBehaviour
    {

        #region 属性

        /// <summary>
        /// 本地玩家
        /// </summary>
        public bool IsLocalPlayer;

        /// <summary>
        /// 玩家Uid
        /// </summary>
        public int Uid;

        /// <summary>
        /// 玩家名字
        /// </summary>
        public string Name;

        /// <summary>
        /// 阵营Id
        /// </summary>
        public byte Camp;

        /// <summary>
        /// 击杀次数
        /// </summary>
        public int KillCnt;

        /// <summary>
        /// 死亡次数
        /// </summary>
        public int DieCnt;

        /// <summary>
        /// 玩家状态
        /// </summary>
        public PlayerState State;

        /// <summary>
        /// 目标移动位置
        /// </summary>
        public Vector3 StartMovePos;

        /// <summary>
        /// 目标移动位置
        /// </summary>
        public Vector3 TargetMovePos;

        // 持续时间
        public float lerpDuration = 0.25f;
        // 记录运行时间
        private float _timeElapsed = 0;

        /// <summary>
        /// 方向
        /// </summary>
        public Vector2Int TargetMoveDir;

        /// <summary>
        /// 复活时间
        /// </summary>
        public float RebornTime { get; set; }


        #endregion

        private Transform DisplayRoot;

        #region Unity

        private void Update()
        {
            if (StartMovePos == TargetMovePos)
            {
                return;
            }

            // 记录下一个位置
            Vector3 valueToLerp;
            _timeElapsed += Time.deltaTime;
            if (_timeElapsed < lerpDuration)
            {
                valueToLerp = Vector3.Lerp(StartMovePos, TargetMovePos, _timeElapsed / lerpDuration);
            }
            else
            {
                valueToLerp = TargetMovePos;
            }
            transform.localPosition = valueToLerp;
        }

        #endregion

        public void Init(AddGamePlayerInfo addInfo)
        {
            Uid = addInfo.uid;
            Name = addInfo.name;
            Camp = addInfo.camp;
            KillCnt = 0;
            DieCnt = 0;
            State = PlayerState.Alive;

            IsLocalPlayer = addInfo.IsLocalPlayer;
            transform.localPosition = GameInstance.ServerPosToClient(addInfo.PosX, addInfo.PosY);

            TargetMovePos = transform.localPosition;
            StartMovePos = transform.localPosition;
        }

        public void Die(int rebornTime)
        {
            if (State == PlayerState.Die)
            {
                return;
            }
            DieCnt++;
            State = PlayerState.Die;
            RebornTime = rebornTime;
        }

        public void Reborn()
        {
            if (State == PlayerState.Alive)
            {
                return;
            }

            State = PlayerState.Alive;
        }

        public void UpdateMovePos(PlayerMoveS2c msg)
        {
            if (State != PlayerState.Alive)
            {
                return;
            }

            NetVector2 movePos = msg.movePos;
            if (movePos.X == 0 && movePos.Y == 0)
            {
                return;
            }

            float moveX = NetworkGeneral.DecodeMoveMsgValue(movePos.X);
            float moveY = NetworkGeneral.DecodeMoveMsgValue(movePos.Y);

            _timeElapsed = 0;
            TargetMovePos = new Vector3(moveX, 0, moveY);
            StartMovePos = transform.localPosition;
        }
    }
}
