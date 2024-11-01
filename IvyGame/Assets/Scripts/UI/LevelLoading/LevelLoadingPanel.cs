using IAUI;
using UnityEngine.UI;
using DG.Tweening;

namespace Game.UI
{
    public class LevelLoadingPanel_Model : UIModel
    {
        public float processTime = 2;
    }

    public class LevelLoadingPanel : UIPanel<LevelLoadingPanel_Model>
    {
        private UIComGlue<Slider> processSlider = new UIComGlue<Slider>("Bottom/Process");
        private Tween silderTween;

        public override void OnShow()
        {
            ShowProcess();
        }

        public override void OnHide()
        {
            silderTween?.Kill();
        }

        private void ShowProcess()
        {
            Slider slider = processSlider.Com;
            slider.value = 0;
            silderTween = slider.DOValue(1, BindModel.processTime);
        }
    }
}
