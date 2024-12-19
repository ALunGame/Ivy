using UnityEngine;

namespace IAUI
{
    public class InternalUIComGlue : UIGlue
    {
        protected string _ComPath;
        protected Transform _ComTrans;

        public InternalUIComGlue(string comPath)
        {
            _ComPath = comPath; 
        }

        public override void OnBeforeAwake(InternalUIPanel panel)
        {
            base.OnBeforeAwake(panel);
            RefreshBind();
        }

        public override void OnDestroy(InternalUIPanel panel)
        {
            _ComTrans = null;
        }

        private void RefreshBind()
        {
            if (_Panel == null || _Panel.transform == null)
            {
                Logger.UI?.LogError("组件绑定失败，界面被销毁！！");
                return;
            }
            if (string.IsNullOrEmpty(_ComPath))
            {
                _ComTrans = _Panel.transform;
                return;
            }
            Transform trans = _Panel.transform.Find(_ComPath);
            if (trans == null)
            {
                Logger.UI?.LogError("组件绑定失败，路径非法！！", _ComPath);
                return;
            }
            _ComTrans = trans;
        }

    }

    /// <summary>
    /// 与Unity组件的胶水
    /// </summary>
    public class UIComGlue<T> : InternalUIComGlue where T : Component
    {
        private T _Com;

        public UIComGlue(string comPath) : base(comPath)
        {
        }

        /// <summary>
        /// 组件
        /// </summary>
        public T Com 
        { 
            get 
            {
                if (_Com == null)
                {
                    _Com = _ComTrans.GetComponent<T>();
                }
                return _Com;
            } 
        }

        public override void OnBeforeAwake(InternalUIPanel panel)
        {
            base.OnBeforeAwake(panel);
            _Com = _ComTrans.GetComponent<T>();
        }

        public override void OnDestroy(InternalUIPanel panel)
        {
            _Com = null;
        }
    }
}
