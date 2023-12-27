using System.Collections.Generic;

namespace LCUI
{
    public class UIStackRule : UIRule
    {
        class PanelStack
        {
            public UIPanelDef panelId;
            public InternalUIPanel panel;

            public PanelStack(UIPanelDef panelId, InternalUIPanel panel)
            {
                this.panelId = panelId;
                this.panel = panel;
            }
        }

        private List<UIShowRule> rules = new List<UIShowRule>() 
        { 
            UIShowRule.Overlay, 
            UIShowRule.Overlay_NoNeedBack,
            UIShowRule.HideOther,
            UIShowRule.HideOther_NoNeedBack,
        };

        private Stack<PanelStack> stack = new Stack<PanelStack>();

        public UIStackRule(UIServer server) : base(server)
        {
        }

        public override List<UIShowRule> CheckRules => rules;

        public override void OnShowPanel(UIPanelDef panelId, InternalUIPanel panel)
        {
            UIPanelCfg panelCnf = UILocate.UI.GetPanelCnf(panelId);
            if (panelCnf.showRule == UIShowRule.Overlay_NoNeedBack || panelCnf.showRule == UIShowRule.HideOther_NoNeedBack)
                return;
            stack.Push(new PanelStack(panelId, panel));
        }

        public override void OnHidePanel(UIPanelDef panelId, InternalUIPanel panel)
        {
            if (stack.Count <= 0)
                return;
            PanelStack panelStack = stack.Peek();
            //栈顶界面关闭
            if (panelStack.panelId == panelId)
            {
                //就剩一个了
                if (stack.Count <= 1)
                {
                    return;
                }
                stack.Pop();
                PanelStack nextPanel = stack.Peek();
                UILocate.UI.Show(nextPanel.panelId);
            }
        }

        public override void Clear()
        {
            stack.Clear();
        }
    }
}
