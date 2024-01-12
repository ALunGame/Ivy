using System.Collections.Generic;

namespace IAEngine
{
    public static class RandomHelper
    {
        /// <summary>
        /// 获得一个打乱的字典
        /// </summary>
        /// <param name="pMin"></param>
        /// <param name="pMax"></param>
        /// <returns></returns>
        public static Dictionary<int,int> GetRandomNumList(int pMin, int pMax)
        {
            Dictionary<int, int> rsMap = new Dictionary<int, int>();
            for (int i = pMin; i <= pMax; i++)
            {
                rsMap.Add(i, i);
            }

            int num, tmp;
            for (int i = pMin; i <= pMax; i++)
            {
                num = UnityEngine.Random.Range(pMin, pMax);
                tmp = rsMap[i];
                rsMap[i] = rsMap[num];
                rsMap[num] = tmp;
            }

            return rsMap;
        }
    }
}
