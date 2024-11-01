using IAConfig;
using System.Collections.Generic;

namespace Game.Network.Server
{
    internal enum SevrerGameMapState
    {
        /// <summary>
        /// 等待
        /// </summary>
        Wait = 0,

        /// <summary>
        /// 进入
        /// </summary>
        Enter,

        /// <summary>
        /// 离开
        /// </summary>
        Exit,
    }

    internal class SevrerGameMapCtrl : ServerGameplayProcess
    {
        public int MapId { get; private set; }

        public SevrerGameMapState MapState { get; private set; }

        private List<BaseServerGameMapSystem> systems = new List<BaseServerGameMapSystem>()
        {
            new SevrerGamerSystem(),
        };

        public override void OnInit()
        {
            MapState = SevrerGameMapState.Wait;

            foreach (BaseServerGameMapSystem system in systems)
                system.Init(this);
        }

        public override void OnUpdateLogic(float pDeltaTime, float pGameTime)
        {
            if (MapState == SevrerGameMapState.Enter)
            {
                foreach (BaseServerGameMapSystem system in systems)
                    system.OnUpdate(pDeltaTime, pGameTime);
            }
        }

        public override void OnClear()
        {
            foreach (BaseServerGameMapSystem system in systems)
                system.Clear();
        }

        public override void OnEnterMap(int pMapId)
        {
            if (!Config.MapCfg.ContainsKey(pMapId))
            {
                NetServerLocate.Log.LogError($"进入地图出错，没有该配置{pMapId}");
                return;
            }

            foreach (BaseServerGameMapSystem system in systems)
                system.OnBeforeEnterMap();

            MapState = SevrerGameMapState.Enter;

            foreach (BaseServerGameMapSystem system in systems)
                system.OnEnterMap();
        }

        public override void OnStartGame(int pGameLevelId)
        {
            foreach (BaseServerGameMapSystem system in systems)
                system.OnStartGame();
        }

        public void ExitMap()
        {
            foreach (BaseServerGameMapSystem system in systems)
                system.OnBeforeExitMap();

            MapState = SevrerGameMapState.Exit;

            foreach (BaseServerGameMapSystem system in systems)
                system.OnExitMap();
        }
    }
}
