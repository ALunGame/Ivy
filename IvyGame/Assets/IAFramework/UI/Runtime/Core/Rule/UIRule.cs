using System.Collections.Generic;

namespace LCUI
{
    public abstract class UIRule
    {
        protected UIServer _Server;

        public UIRule(UIServer server)
        {
            _Server = server;
        }

        public abstract List<UIShowRule> CheckRules { get; }

        public void ShowPanel(UIPanelDef panelId, InternalUIPanel panel)
        {
            if (!CheckRules.Contains(UILocate.UI.GetPanelCnf(panelId).showRule))
            {
                return;
            }
            OnShowPanel(panelId, panel);
        }

        public virtual void OnShowPanel(UIPanelDef panelId,InternalUIPanel panel)
        {

        }

        public void HidePanel(UIPanelDef panelId, InternalUIPanel panel)
        {
            if (!CheckRules.Contains(UILocate.UI.GetPanelCnf(panelId).showRule))
            {
                return;
            }
            OnHidePanel(panelId, panel);
        }

        public virtual void OnHidePanel(UIPanelDef panelId, InternalUIPanel panel)
        {

        }

        public virtual void Clear()
        {

        }
    } 
}
