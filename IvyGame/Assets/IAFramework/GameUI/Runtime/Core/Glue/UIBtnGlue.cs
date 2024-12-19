using System;
using UnityEngine;

namespace IAUI
{
    public class UIBtnGlue : UIGlue
    {
        private string _ComPath;
        private Transform _ComTrans;
        private Action _BtnCallBack;
        
        public UIBtnGlue(string pComPath, Action pBtnCallBack)
        {
            _BtnCallBack = pBtnCallBack;
            _ComPath = pComPath; 
        }

        public override void OnBeforeAwake(InternalUIPanel panel)
        {
            base.OnBeforeAwake(panel);
            RefreshBind();
        }

        public override void OnBeforeShow(InternalUIPanel panel)
        {
            base.OnBeforeShow(panel);
            BtnUtil.SetClick(_ComTrans,null,_BtnCallBack);
        }

        public override void OnHide(InternalUIPanel panel)
        {
            base.OnHide(panel);
            BtnUtil.ClearClick(_ComTrans,null);
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

        /// <summary>
        /// 主动点击
        /// </summary>
        public void Click()
        {
            _BtnCallBack?.Invoke();
        }
    }
}