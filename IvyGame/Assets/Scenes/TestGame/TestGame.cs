using Game.Network.Server;
using IAEngine;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace Game.Network.Server.Test
{
    [Serializable]
    internal class PlayerInfo
    {
        [SerializeField]
        public GameObject PlayerGo;

        [SerializeField]
        public MeshRenderer PlayerCaptureSp;

        [HideInInspector]
        public ServerPlayer Player;

        public void SpawnPlayer(int uid, string name, byte camp, ServerRect rect)
        {
            Player = new ServerPlayer(uid, name, camp);
            Player.SetPos(rect.min.x, rect.min.y);
            PlayerGo.transform.position = new Vector3(rect.min.x, 0, rect.min.y);
        }

        public void Reborn(ServerRect rect)
        {
            Player.SetPos(rect.min.x, rect.min.y);
            PlayerGo.transform.position = new Vector3(rect.min.x, 0, rect.min.y);
        }

        public void Reborn(ServerPoint point)
        {
            Player.SetPos(point.x, point.y);
            PlayerGo.transform.position = new Vector3(point.x, 0, point.y);
        }
    }

    internal class TestGame : MonoBehaviour
    {
        [SerializeField]
        private Transform GridRoot;

        [SerializeField]
        private List<MeshRenderer> GridCampSp = new List<MeshRenderer>();

        [SerializeField]
        private Vector2Int AreaSize = new Vector2Int();

        [SerializeField]
        private PlayerInfo[] Players = new PlayerInfo[2];

        private ServerGameRoom Room;

        private MeshRenderer[,] SpArea;
        private Dictionary<int,List<MeshRenderer>> PlayerCaptureSpDict = new Dictionary<int,List<MeshRenderer>>();

        private void Start()
        {
            Room = new ServerGameRoom();
            Room.Create((byte)AreaSize.x, (byte)AreaSize.y,1);

            Room.Map.Evt_PointCampChange += OnGrdCampChange;
            Room.Map.Evt_KillPlayer += OnKillPlayer;
            Room.Map.Evt_AddPlayerPathPoint += OnAddPlayerCaptureRecord;
            Room.Map.Evt_RemovePlayerPathPoint += OnRemovePlayerCaptureRecord;
            Room.Map.Evt_RemovePlayerPath += OnRemovePlayerCaptureRecord;

            SpArea = new MeshRenderer[AreaSize.x, AreaSize.y];
            for (int x = 0; x < AreaSize.x; x++)
            {
                for (int y = 0; y < AreaSize.y; y++)
                {
                    Vector3 pos = new Vector3(x, 0, y);
                    MeshRenderer spGo = Instantiate(GridCampSp[0]);
                    spGo.gameObject.transform.position = pos;
                    spGo.gameObject.SetActive(true);
                    spGo.transform.SetParent(GridRoot);
                    spGo.gameObject.name = $"{x},{y}";
                    SpArea[x, y] = spGo;
                }
            }

            for (int i = 0; i < 2; i++)
            {
                byte camp = (byte)(i + 1);

                ServerRect rect = Room.Map.CreateCampRect(5, 5, 1)[0];
                Room.Map.ChangRectCamp(rect, camp);

                int playerUid = i + 1;
                Players[i].SpawnPlayer(playerUid, $"{playerUid}", camp, rect);
                PlayerCaptureSpDict.Add(playerUid, new List<MeshRenderer>());

                Room.AddPlayer(Players[i].Player);
            }
        }

        private void Update()
        {
            UpdatePlayer1Iput();
            UpdatePlayer2Iput();
        }

        public Vector2Int movePlayer1Pos = new Vector2Int();
        private void UpdatePlayer1Iput()
        {
            if (Input.GetKeyUp(KeyCode.W))
            {
                movePlayer1Pos.y = 1;
            }
            else
            {
                if (Input.GetKeyUp(KeyCode.S))
                {
                    movePlayer1Pos.y = -1;
                }
                else
                {
                    movePlayer1Pos.y = 0;
                }
            }

            if (Input.GetKeyUp(KeyCode.A))
            {
                movePlayer1Pos.x = -1;
            }
            else
            {
                if (Input.GetKeyUp(KeyCode.D))
                {
                    movePlayer1Pos.x = 1;
                }
                else
                {
                    movePlayer1Pos.x = 0;
                }
            }

            if (movePlayer1Pos != Vector2Int.zero)
            {
                PlayerInfo playerInfo = Players[0];
                ServerPlayer Player1 = playerInfo.Player;
                byte newPosX = (byte)(Player1.Pos.x + movePlayer1Pos.x);
                byte newPosY = (byte)(Player1.Pos.y + movePlayer1Pos.y);
                if (Room.TestPlayerMove(Player1.Uid, newPosX, newPosY))
                {
                    playerInfo.PlayerGo.transform.position = new Vector3(newPosX, 0, newPosY);
                }
            }
        }

        public Vector2Int movePlayer2Pos = new Vector2Int();
        private void UpdatePlayer2Iput()
        {
            if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                movePlayer2Pos.y = 1;
            }
            else
            {
                if (Input.GetKeyUp(KeyCode.DownArrow))
                {
                    movePlayer2Pos.y = -1;
                }
                else
                {
                    movePlayer2Pos.y = 0;
                }
            }

            if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                movePlayer2Pos.x = -1;
            }
            else
            {
                if (Input.GetKeyUp(KeyCode.RightArrow))
                {
                    movePlayer2Pos.x = 1;
                }
                else
                {
                    movePlayer2Pos.x = 0;
                }
            }

            if (movePlayer2Pos != Vector2Int.zero)
            {
                PlayerInfo playerInfo = Players[1];
                ServerPlayer Player2 = playerInfo.Player;
                byte newPosX = (byte)(Player2.Pos.x + movePlayer2Pos.x);
                byte newPosY = (byte)(Player2.Pos.y + movePlayer2Pos.y);
                if (Room.TestPlayerMove(Player2.Uid, newPosX, newPosY))
                {
                    playerInfo.PlayerGo.transform.position = new Vector3(newPosX, 0, newPosY);
                }
            }
        }

        private void OnGrdCampChange(byte posX, byte posY, byte camp)
        {
            //Color color = Color.white;
            if (camp != 0)
            {
                MeshRenderer oldMesh = SpArea[posX, posY];
                MeshRenderer newMesh = Instantiate(GridCampSp[camp]);
                newMesh.gameObject.transform.position = oldMesh.transform.position;
                newMesh.transform.SetParent(GridRoot);
                newMesh.gameObject.name = oldMesh.gameObject.name;
                SpArea[posX, posY] = newMesh;
                Destroy(oldMesh.gameObject);
            }
        }

        private void OnKillPlayer(int diePlayerUid, int killPlayerUid)
        {
            Debug.Log($"OnKillPlayer:死亡玩家{diePlayerUid} 击杀者玩家{killPlayerUid}");
            RebornPlayer(diePlayerUid,Room.GetPlayer(killPlayerUid));
        }

        private void RebornPlayer(int diePlayerUid,ServerPlayer killer)
        {
            PlayerInfo playerInfo = Players[diePlayerUid - 1];
            ServerPlayer player = playerInfo.Player;

            //找到占领区域
            ServerPoint point = Room.Map.GetRandomPointInCamp(player.Camp);
            if (point != null)
            {
                playerInfo.Reborn(point);
            }
            else
            {
                List<ServerRect> rects = Room.Map.CreateCampRect(5, 5, 1, killer.Camp);
                if (rects.IsLegal())
                {
                    ServerRect rect = rects[0];
                    Room.Map.ChangRectCamp(rect, player.Camp);
                    playerInfo.Reborn(rect);
                }
            }
        }

        private void OnAddPlayerCaptureRecord(int playerUid, byte posX, byte posY)
        {
            MeshRenderer sp = Instantiate(Players[playerUid - 1].PlayerCaptureSp);
            sp.gameObject.transform.position = new Vector3(posX, 0.2f, posY);
            sp.gameObject.SetActive(true);
            sp.gameObject.name = $"x;{posX}y:{posY}";
            PlayerCaptureSpDict[playerUid].Add(sp);
        }

        private void OnRemovePlayerCaptureRecord(int playerUid, byte posX, byte posY)
        {
            string key = $"x;{posX}y:{posY}";
            
            MeshRenderer sp = null;
            List<MeshRenderer> splist = PlayerCaptureSpDict[playerUid];
            for (int i = 0; i < splist.Count; i++)
            {
                if (splist[i].name == key)
                {
                    Destroy(splist[i]);
                    splist.RemoveAt(i);
                }
            }
        }

        private void OnRemovePlayerCaptureRecord(int playerUid)
        {
            if (PlayerCaptureSpDict.ContainsKey(playerUid))
            {
                List<MeshRenderer> splist = PlayerCaptureSpDict[playerUid];
                foreach (MeshRenderer s in splist)
                {
                    Destroy(s.gameObject);
                }
                PlayerCaptureSpDict[playerUid].Clear();
            }
        }
    }
}