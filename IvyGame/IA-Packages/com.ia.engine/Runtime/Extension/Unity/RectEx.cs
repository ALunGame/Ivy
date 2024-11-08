using UnityEngine;

namespace IAEngine
{
    public static class RectEx
    {
        /// <summary>
        /// 更新区域X轴范围
        /// </summary>
        /// <param name="pRect"></param>
        /// <param name="pPosX"></param>
        public static void UpdateRectX(ref RectInt pRect, int pPosX)
        {
            if (pRect.Equals(new RectInt(0,0,0,0)))
            {
                pRect.min = new Vector2Int(pPosX, pRect.min.y);
                pRect.max = new Vector2Int(pPosX, pRect.max.y);
            }
            else
            {
                if (pPosX < pRect.min.x)
                {
                    pRect.min = new Vector2Int(pPosX, pRect.min.y);
                }
                else if (pPosX > pRect.max.x)
                {
                    pRect.max = new Vector2Int(pPosX, pRect.max.y);
                }
            }
        }

        /// <summary>
        /// 更新区域范围
        /// </summary>
        /// <param name="pRect"></param>
        /// <param name="pPosX"></param>
        public static void UpdateRect(ref RectInt pRect, Vector2Int pPos)
        {
            if (pRect.Equals(new RectInt(0, 0, 0, 0)))
            {
                pRect.min = new Vector2Int(pPos.x, pPos.y);
                pRect.max = new Vector2Int(pPos.x, pPos.y);
            }
            else
            {
                if (pPos.x < pRect.min.x)
                {
                    pRect.min = new Vector2Int(pPos.x, pRect.min.y);
                }
                else if (pPos.x > pRect.max.x)
                {
                    pRect.max = new Vector2Int(pPos.x, pRect.max.y);
                }

                if (pPos.y < pRect.min.y)
                {
                    pRect.min = new Vector2Int(pRect.min.x, pPos.y);
                }
                else if (pPos.y > pRect.max.y)
                {
                    pRect.max = new Vector2Int(pRect.max.x, pPos.y);
                }
            }
        }

        /// <summary>
        /// 更新区域Y轴范围
        /// </summary>
        /// <param name="pRect"></param>
        /// <param name="pPosY"></param>
        public static void UpdateRectY(ref RectInt pRect, int pPosY)
        {
            if (pRect.Equals(new RectInt(0, 0, 0, 0)))
            {
                pRect.min = new Vector2Int(pRect.min.x, pPosY);
                pRect.max = new Vector2Int(pRect.max.x, pPosY);
            }
            else
            {
                if (pPosY < pRect.min.y)
                {
                    pRect.min = new Vector2Int(pRect.min.x, pPosY);
                }
                else if (pPosY > pRect.max.y)
                {
                    pRect.max = new Vector2Int(pRect.max.x, pPosY);
                }
            }
        }

        /// <summary>
        /// 随机取得一个坐标点
        /// </summary>
        /// <param name="pRect"></param>
        /// <returns></returns>
        public static Vector2Int RandomGetPoint(this RectInt pRect)
        {
            int x = UnityEngine.Random.Range(pRect.min.x, pRect.max.x);
            int y = UnityEngine.Random.Range(pRect.min.y, pRect.max.y);
            return new Vector2Int(x, y);
        }
    }
}
