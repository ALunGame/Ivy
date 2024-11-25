using Proto;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Network
{
    internal static class NetworkGeneral
    {
        public static readonly int PacketTypesCount = Enum.GetValues(typeof(PacketType)).Length;

        /// <summary>
        /// 最大消息标识
        /// </summary>
        public const int MaxGameSequence = 1024;
        public const int HalfMaxGameSequence = MaxGameSequence / 2;

        public static int SeqDiff(int a, int b)
        {
            return Diff(a, b, HalfMaxGameSequence);
        }

        public static int Diff(int a, int b, int halfMax)
        {
            return (a - b + halfMax * 3) % (halfMax * 2) - halfMax;
        }

        /// <summary>
        /// 网络坐标转Unity坐标
        /// </summary>
        /// <param name="pPos"></param>
        /// <returns></returns>
        public static Vector2 ToVector2(this NetVector2 pPos)
        {
            Vector2 pos = new Vector2();
            pos.x = pPos.X / 100;
            pos.y = pPos.Y / 100;
            return pos;
        }

        /// <summary>
        /// 网络坐标转Unity坐标
        /// </summary>
        /// <param name="pPos"></param>
        /// <returns></returns>
        public static Vector2Int ToVector2Int(this NetVector2 pPos)
        {
            Vector2Int pos = new Vector2Int();
            pos.x = (int)pPos.X;
            pos.y = (int)pPos.Y;
            return pos;
        }

        /// <summary>
        /// Unity坐标转网络坐标
        /// </summary>
        /// <param name="pPos"></param>
        /// <returns></returns>
        public static NetVector2 ToNetVector2(this Vector2 pPos)
        {
            NetVector2 netVector2 = new NetVector2();
            netVector2.X = (int)(pPos.x * 100);
            netVector2.Y = (int)(pPos.y * 100);
            return netVector2;
        }

        /// <summary>
        /// Unity坐标转网络坐标
        /// </summary>
        /// <param name="pPos"></param>
        /// <returns></returns>
        public static NetVector2 ToNetVector2(this Vector2Int pPos)
        {
            NetVector2 netVector2 = new NetVector2();
            netVector2.X = pPos.x;
            netVector2.Y = pPos.y;
            return netVector2;
        }

        /// <summary>
        /// 服务器端口
        /// </summary>
        public const int ServerPort = 10515;

        /// <summary>
        /// 连接标识
        /// </summary>
        public const string NetConnectKey = "IvyGame";

        /// <summary>
        /// 基础速度
        /// </summary>
        public const float BaseSpeed = 50;

        /// <summary>
        /// 编码移动消息数据
        /// </summary>
        /// <param name="moveDir"></param>
        /// <param name="moveDel"></param>
        /// <returns></returns>
        public static int EncodeMoveMsgValue(float moveDel)
        {
            return (int)(moveDel * 100);
        }

        public static float DecodeMoveMsgValue(int moveDel)
        {
            return moveDel / 100.0f;
        }
    }

    public enum PlayerState
    {
        /// <summary>
        /// 存活
        /// </summary>
        Alive,

        /// <summary>
        /// 死亡
        /// </summary>
        Die,
    }

    public enum GameState
    {
        None,

        /// <summary>
        /// 等待
        /// </summary>
        Wait,

        /// <summary>
        /// 开始
        /// </summary>
        Start,

        /// <summary>
        /// 结束
        /// </summary>
        End,
    }

    public class PlayerInputCommand
    {
        public const int None       = 0;

        public const int Move_Up    = 1;
        public const int Move_Down  = 2;
        public const int Move_Left  = 3;
        public const int Move_Right = 4;

        public static Dictionary<int, int> MoveDirRotateDict = new Dictionary<int, int>()
        {
            {PlayerInputCommand.Move_Up, 270},
            {PlayerInputCommand.Move_Down, 90},
            {PlayerInputCommand.Move_Left, 180},
            {PlayerInputCommand.Move_Right, 0},
        };
    }
}
