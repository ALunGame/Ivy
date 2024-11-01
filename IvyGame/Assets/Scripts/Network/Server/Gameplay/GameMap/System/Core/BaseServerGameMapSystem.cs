using Gameplay.GameMap;

namespace Game.Network.Server
{
    internal class BaseServerGameMapSystem
    {
        public SevrerGameMapCtrl MapCtrl { get; set; }

        public virtual void Init(SevrerGameMapCtrl pMapCtrl)
        {
            MapCtrl = pMapCtrl;
        }

        public virtual void Clear()
        {

        }

        public virtual void OnBeforeEnterMap()
        {

        }

        public virtual void OnEnterMap()
        {

        }

        public virtual void OnStartGame()
        {

        }

        /// <summary>
        /// 更新逻辑
        /// </summary>
        /// <param name="pDeltaTime">间隔时间</param>
        /// <param name="pGameTime">游戏运行时间</param>
        public virtual void OnUpdate(float pDeltaTime, float pGameTime)
        {

        }

        public virtual void OnBeforeExitMap()
        {

        }

        public virtual void OnExitMap()
        {

        }
    }
}
