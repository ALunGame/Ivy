namespace IAUI
{
    public class UIPanel<T> : InternalUIPanel where T : UIModel, new()
    {

        private T bindModel;

        /// <summary>
        /// 绑定数据
        /// </summary>
        public ref T BindModel
        { 
            get 
            {
                if (bindModel == null)
                {
                    bindModel = new T();
                }
                return ref bindModel; 
            } 
        }

        public override UIModel Model => BindModel;

        /// <summary>
        /// 创建时初始化
        /// </summary>
        public sealed override void Awake()
        {
            base.Awake();
            for (int i = 0; i < Glues.Count; i++)
            {
                Glues[i].OnAwake(this);
            }
            OnAwake();
        }

        /// <summary>
        /// 显示
        /// </summary>
        public sealed override void Show()
        {
            base.Show();
            for (int i = 0; i < Glues.Count; i++)
            {
                Glues[i].OnBeforeShow(this);
            }
            OnShow();
            BindModel.RefreshBindable();
            for (int i = 0; i < Glues.Count; i++)
            {
                Glues[i].OnAfterShow(this);
            }
        }

        /// <summary>
        /// 隐藏
        /// </summary>
        public sealed override void Hide()
        {
            base.Hide();
            BindModel.ClearEvent();
            for (int i = 0; i < Glues.Count; i++)
            {
                Glues[i].OnHide(this);
            }
            OnHide();
        }

        public sealed override void Destroy()
        {
            base.Destroy();
            BindModel.ClearEvent();
            for (int i = 0; i < Glues.Count; i++)
            {
                Glues[i].OnDestroy(this);
            }
            OnDestroy();
        }

        /// <summary>
        /// 尝试通过UIMgr关闭界面
        /// 1，如果这个界面不是通过UI管理打开的，这个操作就是无效的
        /// </summary>
        public bool Close()
        {
            if (UILocate.UI == null)
            {
                return false;
            }
            return UILocate.UI.Hide(this);
        }

        #region Virtual

        /// <summary>
        /// 初始化时
        /// </summary>
        public virtual void OnAwake()
        {

        }

        /// <summary>
        /// 显示
        /// </summary>
        public virtual void OnShow()
        {

        }

        /// <summary>
        /// 隐藏
        /// </summary>
        public virtual void OnHide()
        {

        }

        /// <summary>
        /// 隐藏
        /// </summary>
        public virtual void OnDestroy()
        {

        }



        #endregion
    }
}
