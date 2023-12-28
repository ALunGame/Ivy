using MemoryPack;
using System;

namespace Cnf
{
    
    /// <summary>
    /// 物品信息
    /// </summary>
    [MemoryPackable]
    public partial class TestInfo
    {
        
        /// <summary>
        /// 物品Id
        /// </summary>
        public int itemId;

        /// <summary>
        /// 物品数量
        /// </summary>
        public int itemCnt;

        [MemoryPackConstructor]
        public TestInfo(){}

        public TestInfo(int itemId,int itemCnt)
        {
            
            this.itemId=itemId;

            this.itemCnt=itemCnt;

        }

    }

}


