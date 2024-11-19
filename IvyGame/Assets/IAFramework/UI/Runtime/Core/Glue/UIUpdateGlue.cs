using System;

namespace IAUI
{
    public class UIUpdateGlue : UIGlue
    {
        private Action<float, float> updateFunc;

        public UIUpdateGlue()
        {
        }

        public void SetFunc(Action<float, float> pUpdateFunc)
        {
            updateFunc = pUpdateFunc;
        }
        
        public override void OnAfterShow(InternalUIPanel panel)
        {
            base.OnAfterShow(panel);
            UILocate.UICenter.RegUpdateFunc(updateFunc);
        }

        public override void OnHide(InternalUIPanel panel)
        {
            base.OnHide(panel);
            UILocate.UICenter.RemoveUpdateFunc(updateFunc);
        }
    }
}