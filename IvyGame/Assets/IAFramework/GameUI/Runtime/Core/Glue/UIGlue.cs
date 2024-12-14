using static UnityEngine.Rendering.DebugUI;

namespace IAUI
{
    public class UIGlue
    {
        protected InternalUIPanel _Panel;

        public UIGlue(InternalUIPanel pPanel)
        {
            this._Panel = pPanel;
            this._Panel.AddGlue(this);
        }

        public UIGlue()
        {
            
        }

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