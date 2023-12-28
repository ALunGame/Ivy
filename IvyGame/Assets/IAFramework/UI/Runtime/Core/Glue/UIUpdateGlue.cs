using System;

namespace IAUI
{
    public class UIUpdateGlue : UIGlue
    {
        private Action updateFunc;
        private Action fixedUpdateFunc;

        public UIUpdateGlue()
        {
        }

        public void SetFunc(Action pUpdateFunc, Action pFixedUpdateFunc = null)
        {
            updateFunc = pUpdateFunc;
            fixedUpdateFunc = pFixedUpdateFunc;
        }
        
        public override void OnAfterShow(InternalUIPanel panel)
        {
            base.OnAfterShow(panel);
            UILocate.UICenter.RegUpdateFunc(updateFunc);
            UILocate.UICenter.RegFixedUpdateFunc(fixedUpdateFunc);
        }

        public override void OnHide(InternalUIPanel panel)
        {
            base.OnHide(panel);
            UILocate.UICenter.RemoveUpdateFunc(updateFunc);
            UILocate.UICenter.RemoveFixedUpdateFunc(fixedUpdateFunc);
        }
    }
}