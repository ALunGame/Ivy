using Cysharp.Threading.Tasks;
using Game.Helper;
using Game.UI;
using Gameplay.GameMap.Actor;
using Gameplay.GameMap.System;
using Gameplay.Map;
using Gameplay.System;
using IAConfig;
using IAFramework;
using IAUI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Gameplay.GameMap
{
    public enum GameMapState
    {
        /// <summary>
        /// 卸载旧地图
        /// </summary>
        UnLoad = 0,

        /// <summary>
        /// 加载新的
        /// </summary>
        Load,

        /// <summary>
        /// 加载完成
        /// </summary>
        Finish,
    }

    public class GameMapCtrl : GameplayProcess
    {
        public int MapId { get; private set; }

        public Vector2Int MapSize { get; private set; }

        public GameMapState MapState { get; private set; }

        public Transform MapRootTrans { get; private set; }

        public Transform MapTrans { get; private set; }

        public float LoadingTime = 2;

        private List<BaseGameMapSystem> systems = new List<BaseGameMapSystem>()
        {
            new GameActorSystem(),
            new GameEnvSystem(),
            new GameGamerSystem(),
            new GameCameraSystem()
        };

        public override void OnInit()
        {
            MapRootTrans = GameObject.Find("Game").transform;

            foreach (BaseGameMapSystem system in systems)
                system.Init(this);

            ActorEvent.Init();
            SystemEvent.Init();
        }

        public override void OnUpdateLogic(float pDeltaTime, float pGameTime)
        {
            if (MapState == GameMapState.Finish)
            {
                foreach (BaseGameMapSystem system in systems)
                    system.OnUpdate(pDeltaTime, pGameTime);
            }
        }

        public override void OnClear()
        {
            foreach (BaseGameMapSystem system in systems)
                system.Clear();

            ActorEvent.Clear();
            SystemEvent.Clear();
        }

        public override void OnEnterMap(int pMapId)
        {
            if (!Config.MapCfg.ContainsKey(pMapId))
            {
                Debug.LogError($"进入地图出错，没有该配置{pMapId}");
                return;
            }

            ActorEvent.Clear();
            SystemEvent.Clear();

            //打开界面
            LevelLoadingPanel_Model uiModel = UILocate.UI.GetPanelModel<LevelLoadingPanel_Model>(UIPanelDef.LevelLoadingPanel);
            uiModel.processTime = LoadingTime;
            UILocate.UI.Show(UIPanelDef.LevelLoadingPanel);
            UILocate.UI.DestroyAllPanel(UIPanelDef.LevelLoadingPanel);

            //删除上一个地图
            if (MapTrans != null)
            {
                MapState = GameMapState.UnLoad;

                for (int systemIndex = 0; systemIndex < systems.Count; systemIndex++)
                    systems[systemIndex].OnBeforeExitMap();

                GameObject.Destroy(MapTrans.gameObject);

                for (int systemIndex = 0; systemIndex < systems.Count; systemIndex++)
                    systems[systemIndex].OnExitMap();
            }

            MapState = GameMapState.Load;

            //进入新的
            MapCfg cfg = Config.MapCfg[pMapId];
            MapId = pMapId;
            MapSize = new Vector2Int((int)cfg.gridSize.x, (int)cfg.gridSize.y);

            for (int systemIndex = 0; systemIndex < systems.Count; systemIndex++)
                systems[systemIndex].OnBeforeEnterMap();

            GameObject mapGo = GameEnv.Asset.CreateGo(cfg.prefab);
            mapGo.transform.SetParent(MapRootTrans);
            MapTrans = mapGo.transform;

            for (int systemIndex = 0; systemIndex < systems.Count; systemIndex++)
                systems[systemIndex].OnEnterMap();

            TweenHelper.DoDelayFunc(() =>
            {
                MapState = GameMapState.Finish;
                UILocate.UI.Hide(UIPanelDef.LevelLoadingPanel);

                UILocate.UI.Show(UIPanelDef.FightPanel);
            }, LoadingTime + 0.2f);
        }

        public override void OnStartGame(int pGameLevelId)
        {
            for (int systemIndex = 0; systemIndex < systems.Count; systemIndex++)
                systems[systemIndex].OnStartGame();
        }

        public T GetSystem<T>() where T : BaseGameMapSystem
        {
            foreach (var item in systems)
            {
                if (item is T)
                {
                    return (T)item;
                }
            }

            return null;
        }

        public void OnClick(PointerEventData pEventData)
        {
            Vector3 worldPos = UIEventHelper.PointerToWorldPos(pEventData);

            GameActorSystem actorSystem = GameplayCtrl.Instance.GameMap.GetSystem<GameActorSystem>();
            if (actorSystem.CheckClickActor(worldPos, pEventData))
            {
                return;
            }

            //ActorMoveSystem moveSystem = GameplayCtrl.Instance.LevelCtrl.GetSystem<ActorMoveSystem>();
            //moveSystem.MoveTo(MapHelper.GetPlayerActor(), UIEventHelper.PointerToWorldPos(pEventData));
        }
    }
}
