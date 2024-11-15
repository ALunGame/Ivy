using Gameplay;
using IAEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGridsCheck : MonoBehaviour
{
    public MapGrids MapGrids;
    public RectInt CampRect;
    public LineRenderer Line;

    private Dictionary<int, Dictionary<int, int>> CampPosDict = new Dictionary<int, Dictionary<int, int>>();

    // Start is called before the first frame update
    void Start()
    {
        CampRect = new RectInt(0,0,0,0);
        MapGrids.CreateMap(new Vector2Int(100, 100));
    }

    // Update is called once per frame
    void Update()
    {
        MapGrids.UpdateLogic(Time.deltaTime, Time.realtimeSinceStartup);
        if (Input.GetMouseButton(0))
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,
               Camera.main.transform.position.z > 0 ? Camera.main.transform.position.z : -Camera.main.transform.position.z));//屏幕坐标转换世界坐标
            Debug.Log("通过修改Z轴获取到的世界坐标是" + worldPos);

            Vector2Int pos = new Vector2Int((int)worldPos.x, (int)worldPos.z);
            if (CampPosDict.ContainsKey(pos.x) && CampPosDict[pos.x].ContainsKey(pos.y))
            {
                return;
            }
            if (!CampPosDict.ContainsKey(pos.x))
                CampPosDict.Add(pos.x, new Dictionary<int, int>());
            CampPosDict[pos.x].Add(pos.y, 1);

            MapGrids.ChangeGridsCamp(new List<Vector2Int>() { pos }, 1);

            RectEx.UpdateRectOnAddPoint(ref CampRect, pos);
        }

        if (Input.GetMouseButton(1))
        {
            CalcCampLines();
        }
    }

    private void OnDrawGizmosSelected()
    {
        IAToolkit.GizmosHelper.DrawBounds(new Vector3(CampRect.center.x, 0, CampRect.center.y), new Vector3(CampRect.size.x, 1, CampRect.size.y), Color.blue);
    }

    private void CalcCampLines()
    {
        List<int> xList = CampPosDict.Keys.ToList();
        xList.Sort();

        int xMin = xList[0];
        int xMax = xList[xList.Count - 1];

        for (int x = xMin; x <= xMax; x++)
        {
            
        }
    }
}
