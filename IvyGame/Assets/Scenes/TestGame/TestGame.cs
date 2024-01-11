using Game.Network.Server;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Test
{
    public class TestGame : MonoBehaviour
    {
        [SerializeField]
        private MeshRenderer GridSp;

        [SerializeField]
        private MeshRenderer GridCampSp;

        [SerializeField]
        private Vector2Int AreaSize = new Vector2Int();

        [SerializeField]
        private GameObject PlayerGo;

        [SerializeField]
        private MeshRenderer PlayerCaptureSp;

        [SerializeField]
        private Vector2Int PlayerSpawnPos = new Vector2Int();

        private ServerGameRoom Room;
        private ServerPlayer Player;

        private MeshRenderer[,] SpArea;
        private Dictionary<int,List<MeshRenderer>> PlayerCaptureSpDict = new Dictionary<int,List<MeshRenderer>>();

        private void Start()
        {
            Room = new ServerGameRoom();
            Room.Create((byte)AreaSize.x, (byte)AreaSize.y,1);

            Room.Map.Evt_PointCampChange += OnGrdCampChange;
            Room.Map.Evt_KillPlayer += OnKillPlayer;
            Room.Map.Evt_AddPlayerPathPoint += OnAddPlayerCaptureRecord;
            Room.Map.Evt_RemovePlayerPath += OnRemovePlayerCaptureRecord;

            SpArea = new MeshRenderer[AreaSize.x, AreaSize.y];
            for (int x = 0; x < AreaSize.x; x++)
            {
                for (int y = 0; y < AreaSize.y; y++)
                {
                    Vector3 pos = new Vector3(x, 0, y);
                    MeshRenderer spGo = Instantiate(GridSp);
                    spGo.gameObject.transform.position = pos;
                    spGo.gameObject.SetActive(true);
                    SpArea[x, y] = spGo;
                }
            }

            Player = new ServerPlayer(1, "aaa", 1);
            Player.SetPos((byte)PlayerSpawnPos.x, (byte)PlayerSpawnPos.y);
            PlayerGo.transform.position = new Vector3(PlayerSpawnPos.x,0, PlayerSpawnPos.y);

            PlayerCaptureSpDict.Add(1, new List<MeshRenderer>());
            Room.AddPlayer(Player);
        }


        public Vector2Int movePos = new Vector2Int();

        private void Update()
        {
            float hrValue = Input.GetAxis("Horizontal");
            float vrValue = Input.GetAxis("Vertical");

            if (Input.GetKeyUp(KeyCode.W))
            {
                movePos.y = 1;
            }
            else
            {
                if (Input.GetKeyUp(KeyCode.S))
                {
                    movePos.y = -1;
                }
                else
                {
                    movePos.y = 0;
                }
            }

            


            if (Input.GetKeyUp(KeyCode.A))
            {
                movePos.x = -1;
            }
            else
            {
                if (Input.GetKeyUp(KeyCode.D))
                {
                    movePos.x = 1;
                }
                else
                {
                    movePos.x = 0;
                }
            }




            //movePos.x = Mathf.CeilToInt(hrValue); 
            //movePos.y = Mathf.CeilToInt(vrValue);

            if (movePos != Vector2Int.zero)
            {
                byte newPosX = (byte)(Player.Pos.x + movePos.x);
                byte newPosY = (byte)(Player.Pos.y + movePos.y);
                if (Room.TestPlayerMove(Player.Uid, newPosX, newPosY))
                {
                    PlayerGo.transform.position = new Vector3(newPosX, 0, newPosY);
                }
            }

            
        }

        private void OnGrdCampChange(byte posX, byte posY, byte camp)
        {
            //Color color = Color.white;
            if (camp != 0)
            {
                MeshRenderer oldMesh = SpArea[posX, posY];
                MeshRenderer newMesh = Instantiate(GridCampSp);
                newMesh.gameObject.transform.position = oldMesh.transform.position;
                SpArea[posX, posY] = newMesh;
                Destroy(oldMesh);
            }
        }

        private void OnKillPlayer(int diePlayerUid, int killPlayerUid)
        {

        }

        private void OnAddPlayerCaptureRecord(int playerUid, byte posX, byte posY)
        {
            MeshRenderer sp = Instantiate(PlayerCaptureSp);
            sp.gameObject.transform.position = new Vector3(posX, 0, posY);
            sp.gameObject.SetActive(true);
            PlayerCaptureSpDict[playerUid].Add(sp);
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