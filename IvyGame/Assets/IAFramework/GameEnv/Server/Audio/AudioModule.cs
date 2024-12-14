using Cysharp.Threading.Tasks;
using DG.Tweening;
using IAEngine;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace IAFramework
{
    internal class AudioModule : MonoBehaviour
    {
        public const string AudioPlayPoolName = "AudioPlayPool";

        private AudioSource BGMAudioSource;

        private int EffectAudioDefaultQuantity = 20;

        private Transform audioPlayRoot;
        // 场景中生效的所有特效音乐播放器
        private List<AudioSource> audioPlayList;

        #region 音量、播放控制

        //全局音量
        [SerializeField, Range(0, 1)]
        private float globalVolume;
        public float GlobalVolume
        {
            get => globalVolume;
            set
            {
                if (globalVolume == value) return;
                globalVolume = value;
                UpdateAllAudioPlay();
            }
        }

        //背景音乐音量
        [SerializeField]
        [Range(0, 1)]
        private float bgVolume;
        public float BGVolume
        {
            get => bgVolume;
            set
            {
                if (bgVolume == value) return;
                bgVolume = value;
                UpdateBGAudioPlay();
            }
        }

        //特效音乐音量
        [SerializeField]
        [Range(0, 1)]
        private float audioVolume;
        public float AudioVolume
        {
            get => audioVolume;
            set
            {
                if (audioVolume == value) return;
                audioVolume = value;
                UpdateEffectAudioPlay();
            }
        }

        //静音
        [SerializeField]
        private bool isMute = false;
        public bool IsMute
        {
            get => isMute;
            set
            {
                if (isMute == value) return;
                isMute = value;
                UpdateMute();
            }
        }

        //背景音乐暂停
        [SerializeField]
        private bool isPause = false;
        public bool IsPause
        {
            get => isPause;
            set
            {
                if (isPause == value) return;
                isPause = value;
                UpdatePause();
            }
        }

        /// <summary>
        /// 更新全部播放器类型
        /// </summary>
        private void UpdateAllAudioPlay()
        {
            UpdateBGAudioPlay();
            UpdateEffectAudioPlay();
        }

        /// <summary>
        /// 更新背景音乐
        /// </summary>
        private void UpdateBGAudioPlay()
        {
            BGMAudioSource.volume = bgVolume * globalVolume;
        }

        /// <summary>
        /// 更新特效音乐播放器
        /// </summary>
        private void UpdateEffectAudioPlay()
        {
            if (audioPlayList == null) return;
            // 倒序遍历
            for (int i = audioPlayList.Count - 1; i >= 0; i--)
            {
                if (audioPlayList[i] != null)
                {
                    SetEffectAudioPlay(audioPlayList[i]);
                }
                else
                {
                    audioPlayList.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// 设置特效音乐播放器
        /// </summary>
        private void SetEffectAudioPlay(AudioSource audioPlay, float spatial = -1)
        {
            audioPlay.mute = isMute;
            audioPlay.volume = audioVolume * globalVolume;
            if (spatial != -1)
            {
                audioPlay.spatialBlend = spatial;
            }
            if (isPause)
            {
                audioPlay.Pause();
            }
            else
            {
                audioPlay.UnPause();
            }
        }

        /// <summary>
        /// 更新全局音乐静音情况
        /// </summary>
        private void UpdateMute()
        {
            BGMAudioSource.mute = isMute;
            UpdateEffectAudioPlay();
        }

        /// <summary>
        /// 更新背景音乐暂停
        /// </summary>
        private void UpdatePause()
        {
            if (isPause)
            {
                BGMAudioSource.Pause();
            }
            else
            {
                BGMAudioSource.UnPause();
            }

        } 

        #endregion

        public void Init()
        {
            audioPlayList = new List<AudioSource>(EffectAudioDefaultQuantity);
            audioPlayRoot = new GameObject("AudioPlayRoot").transform;
            audioPlayRoot.SetParent(transform);

            GameObject bgmGo = new GameObject("BGM");
            bgmGo.transform.SetParent(transform);
            AudioSource bgmAudio = bgmGo.AddComponent<AudioSource>();
            bgmAudio.spatialBlend = -1;
            BGMAudioSource = bgmAudio;
        }

        #region 背景音乐

        private CancellationTokenSource doFadeTaskCancleToken;

        /// <summary>
        /// 播放背景音乐
        /// </summary>
        /// <param name="pClip"></param>
        /// <param name="pLoop"></param>
        /// <param name="pVolume"></param>
        /// <param name="pFadeOutTime"></param>
        /// <param name="pFadeInTime"></param>
        public void PlayBGM(AudioClip pClip, float pVolume = -1, float pFadeOutTime = 0, float pFadeInTime = 0)
        {
            if (pVolume != -1)
            {
                BGVolume = pVolume;
            }

            if (doFadeTaskCancleToken != null)
                doFadeTaskCancleToken.Cancel();
            doFadeTaskCancleToken = new CancellationTokenSource();
            DoVolumeFade(pClip, pFadeOutTime, pFadeInTime).Forget();
        }

        private async UniTaskVoid DoVolumeFade(AudioClip pClip, float pFadeOutTime = 0, float pFadeInTime = 0)
        {
            float currTime = 0;
            if (pFadeOutTime <= 0)
                pFadeOutTime = 0.0001f;
            if (pFadeInTime <= 0)
                pFadeInTime = 0.0001f;

            // 降低音量，也就是淡出
            while (currTime < pFadeOutTime)
            {
                await UniTask.WaitForEndOfFrame(doFadeTaskCancleToken.Token);

                if (!isPause) 
                    currTime += Time.deltaTime;

                float ratio = Mathf.Lerp(1, 0, currTime / pFadeOutTime);
                BGMAudioSource.volume = bgVolume * globalVolume * ratio;
            }

            BGMAudioSource.clip = pClip;
            BGMAudioSource.Play();
            currTime = 0;

            // 提高音量，也就是淡入
            while (currTime < pFadeInTime)
            {
                await UniTask.WaitForEndOfFrame(doFadeTaskCancleToken.Token);

                if (!isPause) 
                    currTime += Time.deltaTime;

                float ratio = Mathf.InverseLerp(0, 1, currTime / pFadeInTime);
                BGMAudioSource.volume = bgVolume * globalVolume * ratio;
            }
        }

        private CancellationTokenSource bgWithClipsTaskCancleToken;
        /// <summary>
        /// 播放背景音乐组
        /// </summary>
        /// <param name="clips"></param>
        /// <param name="volume"></param>
        /// <param name="fadeOutTime"></param>
        /// <param name="fadeInTime"></param>
        public void PlayBGMS(AudioClip[] pClips, float pVolume = -1, float pFadeOutTime = 0, float pFadeInTime = 0)
        {
            if (bgWithClipsTaskCancleToken != null)
                bgWithClipsTaskCancleToken.Cancel();
            bgWithClipsTaskCancleToken = new CancellationTokenSource();
        }

        private async UniTaskVoid DoPlayBGAudioWithClips(AudioClip[] pClips, float pVolume = -1, float pFadeOutTime = 0, float pFadeInTime = 0)
        {
            if (pVolume != -1)
            {
                BGVolume = pVolume;
            }

            int currIndex = 0;
            while (true)
            {
                AudioClip clip = pClips[currIndex];
                PlayBGM(clip, pVolume, pFadeOutTime, pFadeInTime);

                float time = clip.length;
                // 时间只要还好，一直检测
                while (time > 0)
                {
                    await UniTask.WaitForEndOfFrame(bgWithClipsTaskCancleToken.Token);
                    if (!isPause) 
                        time -= Time.deltaTime;
                }
                // 到达这里说明倒计时结束，修改索引号，继续外侧While循环
                currIndex++;
                if (currIndex >= pClips.Length) 
                    currIndex = 0;
            }
        }

        /// <summary>
        /// 停止背景音乐
        /// </summary>
        public void StopBGM()
        {
            if (doFadeTaskCancleToken != null)
                doFadeTaskCancleToken.Cancel();

            if (bgWithClipsTaskCancleToken != null)
                bgWithClipsTaskCancleToken.Cancel();

            BGMAudioSource.Stop();
            BGMAudioSource.clip = null;
        }

        /// <summary>
        /// 暂停背景音乐
        /// </summary>
        public void PauseBGM()
        {
            IsPause = true;
        }

        /// <summary>
        /// 恢复背景音乐
        /// </summary>
        public void UnPauseBGM()
        {
            IsPause = false;
        }

        #endregion

        #region 特效音乐

        /// <summary>
        /// 获取音乐播放器
        /// </summary>
        /// <returns></returns>
        private AudioSource GetAudioPlay()
        {
            if (!CachePool.HasGameObjectPool(AudioPlayPoolName))
            {
                CachePool.CreateGameObjectPool(AudioPlayPoolName, () =>
                {
                    GameObject audioGo = new GameObject("audioGo");
                    AudioSource source = audioGo.AddComponent<AudioSource>();
                    return audioGo;
                });
            }

            // 从对象池中获取播放器
            GameObject audioPlay = CachePool.GetGameObject(AudioPlayPoolName, audioPlayRoot);
            AudioSource audioSource = audioPlay.GetComponent<AudioSource>();
            SetEffectAudioPlay(audioSource);
            audioPlayList.Add(audioSource);
            return audioSource;
        }

        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="pClip"></param>
        /// <param name="pGo"></param>
        /// <param name="pVolumeScale"></param>
        /// <param name="pCallBack"></param>
        public void PlayAudio(AudioClip pClip, float pVolumeScale = 1, Action pCallBack = null)
        {
            // 初始化音乐播放器
            AudioSource audioSource = GetAudioPlay();
            // 播放一次音效
            audioSource.PlayOneShot(pClip, pVolumeScale);
            audioSource.gameObject.name = pClip.name;
            DoRecycleAudioPlay(audioSource, pClip, pCallBack).Forget();
        }

        private async UniTaskVoid DoRecycleAudioPlay(AudioSource audioSource, AudioClip clip, Action callBak)
        {
            // 延迟 Clip的长度（秒）
            await UniTask.WaitForSeconds(clip.length);

            // 放回池子
            if (audioSource != null)
            {
                audioPlayList.Remove(audioSource);
                CachePool.PushGameObject(AudioPlayPoolName, audioSource.gameObject);
                callBak?.Invoke();
            }
        }


        #endregion
    }
}