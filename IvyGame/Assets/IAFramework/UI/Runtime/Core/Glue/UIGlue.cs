namespace LCUI
{
    public class UIGlue
    {
        protected InternalUIPanel _Panel;

        public virtual void OnAwake(InternalUIPanel panel)
        {
            this._Panel = panel;
        }

        public virtual void OnBeforeShow(InternalUIPanel panel)
        {
        }

        public virtual void OnAfterShow(InternalUIPanel panel)
        {
        }

        public virtual void OnHide(InternalUIPanel panel)
        {

        }

        public virtual void OnDestroy(InternalUIPanel panel)
        {

        }
    }
}