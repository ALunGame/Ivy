using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game
{

    public class PlayerCfg
    {
        public string PrefabName;

        public string PathPrefabName;
    }

    public static class TempConfig
    {
        public static Vector2Int MapSize = new Vector2Int(50, 50);

        public static Vector2 GridSize = new Vector2(1, 1);

        public static int RebornTime = 3;

        public static int GameTotalTime = 90;

        public static Dictionary<int, Color> CampColorDict = new Dictionary<int, Color>()
        {
            {0,Color.white },
            {1,Color.red},
            {2,Color.green},
        };
    }
}
