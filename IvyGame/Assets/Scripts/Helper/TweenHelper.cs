using DG.Tweening;
using System;

namespace Game.Helper
{
    public class TweenHelper
    {
        public static Tween DoDelayFunc(Action callBack, float delayTime = 0.1f)
        {
            float timeCount = delayTime;
            return DOTween.To(() => timeCount, a => timeCount = a, 0.1f, delayTime).OnComplete(new TweenCallback(delegate
            {
                callBack?.Invoke();
            }));
        }
    }
}
