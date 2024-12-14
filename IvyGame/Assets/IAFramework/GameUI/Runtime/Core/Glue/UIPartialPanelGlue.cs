using IAEngine;
using IAFramework;
using UnityEngine;

namespace IAUI
{
    public class InternalUIPartialPanelGlue : UIGlue
    {
        /// <summary>
        /// 分布面板预制体
        /// </summary>
        protected string _PrefabName;

        /// <summary>
        /// 分布面板预制体绑定路径
        /// </summary>
        protected string _PrefabBindPath;

        /// <summary>
        /// 分布面板节点路径
        /// </summary>
        protected string _PanelPath;
        
        /// <summary>
        /// 跟随父界面生命周期
        /// </summary>
        protected bool _FollowParent;

        protected Transform _PartialPanelTrans;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefabName">分布面板预制体名</param>
        /// <param name="prefabBindPath">分布面板预制体绑定路径</param>
        /// <param name="followParent">跟随父界面生命周期</param>
        public InternalUIPartialPanelGlue(string prefabName, string prefabBindPath, bool followParent = true)
        {
            _PanelPath  = "";
            _PrefabName = prefabName;
            _PrefabBindPath = prefabBindPath;
            _FollowParent = followParent;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="panelPath">分布面板节点路径,父界面的相对路径</param>
        /// <param name="followParent">跟随父界面生命周期</param>
        public InternalUIPartialPanelGlue(string panelPath, bool followParent = true)
        {
            _PrefabName = "";
            _PanelPath = panelPath;
            _FollowParent = followParent;
        }

        /// <summary>
        /// 获得分布界面节点
        /// </summary>
        /// <returns></returns>
        protected Transform GetPartialPanelTrans()
        {
            if (_PartialPanelTrans == null)
            {
                if (string.IsNullOrEmpty(_PanelPath))
                {
                    GameObject tGo = GameEnv.Asset.CreateGo(_PrefabName);
                    if (_Panel.transform.Find(_PrefabBindPath,out Transform bindTrans))
                    {
                        tGo.transform.SetParent(bindTrans);
                        tGo.transform.Reset();
                    }

                    _PartialPanelTrans = tGo.transform;
                }
                else
                {
                    if (!_Panel.transform.Find(_PanelPath,out _PartialPanelTrans))
                    {
                        UILocate.Log.LogError("分布视图出错，路径上没有对应节点",_PanelPath);
                    }
                }
            }

            return _PartialPanelTrans;
        }


        protected void Clear()
        {
            _PartialPanelTrans = null;
        }
    }
    
    /// <summary>
    /// 分布面板胶水
    /// </summary>
    public class UIPartialPanelGlue<T> : InternalUIPartialPanelGlue where T : InternalUIPanel,new()
    {
        private T _PartPanel;

        public T PartPanel { get => _PartPanel; set => _PartPanel = value; }

        public UIPartialPanelGlue(string prefabName, string prefabBindPath, bool followParent = true) : base(prefabName, prefabBindPath, followParent)
        {
            InitFollowParent();
        }

        public UIPartialPanelGlue(string panelPath, bool followParent = true) : base(panelPath, followParent)
        {
            InitFollowParent();
        }
        
        private void InitFollowParent()
        {
            if (_FollowParent)
            {
                if (_PartPanel == null)
                {
                    _PartPanel = new T();
                }
            }
        }

        public override void OnAfterAwake(InternalUIPanel panel)
        {
            base.OnAfterAwake(panel);
            if (_FollowParent)
            {
                UIPanelCreater.CreateUIPanelTrans(_PartPanel, GetPartialPanelTrans());
            }
        }

        public override void OnAfterShow(InternalUIPanel panel)
        {
            base.OnAfterShow(panel);
            if (_FollowParent)
            {
                _PartPanel.Show();
            }
        }

        public override void OnHide(InternalUIPanel panel)
        {
            base.OnHide(panel);
            if (_FollowParent)
            {
                _PartPanel.Hide();
            }
        }

        public override void OnDestroy(InternalUIPanel panel)
        {
            base.OnDestroy(panel);
            if (_FollowParent)
            {
                _PartPanel.Destroy();
                Clear();
            }
        }

        public TModel GetPanelModel<TModel>() where TModel : UIModel
        {
            if (_PartPanel == null)
            {
                _PartPanel = new T();
                UIPanelCreater.CreateUIPanelTrans(_PartPanel, GetPartialPanelTrans());
            }
            return (TModel)_PartPanel.Model;
        }

        /// <summary>
        /// 主动调用显示
        /// </summary>
        public void Show()
        {
            if (_PartPanel == null)
            {
                _PartPanel = new T();
                UIPanelCreater.CreateUIPanelTrans(_PartPanel, GetPartialPanelTrans());
            }
            _PartPanel.Show();
        }
        
        
        /// <summary>
        /// 主动调用隐藏
        /// </summary>
        public void Hide()
        {
            if (_PartPanel == null)
                return;
            _PartPanel.Hide();
        }
        
        /// <summary>
        /// 主动调用销毁
        /// </summary>
        public void Destroy()
        {
            if (_PartPanel == null)
                return;
            _PartPanel.Destroy();
            Clear();
        }
    }
}