using Game.Network;
using Game.Network.Client;
using IAEngine;
using Proto;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.GameData
{
    internal class AIGamerData : GamerData
    {
        private static List<Vector2Int> MoveDirList = new List<Vector2Int>()
        {
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1),
        };

        private TimerModel updateCDTimer;
        private bool needUpdate;
        private GameDataField<Vector2Int> moveDir;

        private TimerModel moveToCampTimer;
        private bool moveToCamp;

        private GamerInputC2s inputCommand;

        public AIGamerData(GamerInfo pInfo) : base(pInfo)
        {
            float timeCd = RandomHelper.Range(3.0f, 4.0f, GetHashCode());
            needUpdate = false;
            updateCDTimer = new TimerModel($"AIGamer_{GamerUid}_UpdateCD", timeCd, () =>
            {
                needUpdate = true;
            });
            updateCDTimer.Start();

            timeCd = RandomHelper.Range(6.0f, 7.0f, GetHashCode());
            moveToCamp = false;
            moveToCampTimer = new TimerModel($"AIGamer_{GamerUid}_UpdateMoveToCamp", timeCd, () =>
            {
                moveToCamp = true;
            });
            moveToCampTimer.Start();

            moveDir = new GameDataField<Vector2Int>(this);
            moveDir.SetValueWithoutNotify(Vector2Int.zero);
            moveDir.RegValueChangedEvent((newValue, oldValue) =>
            {
                inputCommand.commandType = PlayerInputCommand.None;

                if (newValue.x != 0)
                {
                    if (newValue.x > 0)
                        inputCommand.commandType = PlayerInputCommand.Move_Right;
                    else
                        inputCommand.commandType = PlayerInputCommand.Move_Left;
                }
                else
                {
                    if (newValue.y > 0)
                        inputCommand.commandType = PlayerInputCommand.Move_Up;
                    else
                        inputCommand.commandType = PlayerInputCommand.Move_Down;
                }

                inputCommand.Rotation = PlayerInputCommand.MoveDirRotateDict[inputCommand.commandType];

                NetClientLocate.Net.Send((ushort)RoomMsgDefine.GamerInputC2s, inputCommand);
            });

            inputCommand = new GamerInputC2s();
            inputCommand.gamerUid = GamerUid;
        }

        public override void UpdateLogic(float pTimeDelta, float pGameTime)
        {
            UpdateActorAILogic(pTimeDelta);
        }

        private void UpdateActorAILogic(float pDeltaTime)
        {
            updateCDTimer.Update(pDeltaTime);
            moveToCampTimer.Update(pDeltaTime);

            if (!needUpdate)
                return;
            needUpdate = false;

            Vector2Int currGridPos = GridPos.Value;
            int currCamp = GameplayGlobal.Data.Map.GetGridData(currGridPos).Camp.Value;

            //在自己的区域内
            if (currCamp == Camp)
            {
                moveToCamp = false;

                Vector2Int newMoveDir = CalcRandomMoveDir();
                moveDir.Value = newMoveDir;
            }
            else
            {
                //移动到占领区域
                if (moveToCamp)
                {
                    if (CalcMoveDirToCamp(out var outMoveDir))
                    {
                        moveDir.Value = outMoveDir;
                    }
                }
                else
                {
                    Vector2Int newMoveDir = CalcRandomMoveDir();
                    moveDir.Value = newMoveDir;
                }
            }
        }

        private Vector2Int CalcRandomMoveDir()
        {
            Vector2Int currMoveDir = moveDir.Value;
            if (currMoveDir == Vector2Int.zero)
                currMoveDir = MoveDirList[0];

            Vector2Int currGridPos = GridPos.Value;
            for (int i = 0; i < MoveDirList.Count; i++)
            {
                Vector2Int tDir = MoveDirList[i];
                Vector2Int nextGridPos = currGridPos + tDir;
                if (GameplayGlobal.Data.Map.CheckPosLegal(nextGridPos))
                {
                    if (RandomHelper.Range())
                    {
                        return tDir;
                    }
                }
            }

            return currMoveDir;
        }

        private bool CalcMoveDirToCamp(out Vector2Int outMoveDir)
        {
            List<GameMapGridData> grids = GameplayGlobal.Data.Map.CampGrids[Camp];
            if (!grids.IsLegal())
            {
                outMoveDir = Vector2Int.zero;
                return false;
            }

            Vector2Int currGridPos = GridPos.Value;
            Vector2Int checkGridPos = grids[0].GridPos;
            int xDis = checkGridPos.x - currGridPos.x;
            int yDis = checkGridPos.y - currGridPos.y;

            if (xDis == 0 && yDis == 0)
            {
                outMoveDir = Vector2Int.zero;
                return false;
            }

            //左右
            if (moveDir.Value.x == 0)
            {
                int xDir = xDis > 0 ? 1 : -1;
                outMoveDir = new Vector2Int(xDir, 0);
                return true;
            }

            //上下
            if (moveDir.Value.y == 0)
            {
                int yDir = yDis > 0 ? 1 : -1;
                outMoveDir = new Vector2Int(0, yDir);
                return true;
            }

            outMoveDir = Vector2Int.zero;
            return false;
        }
    }
}
