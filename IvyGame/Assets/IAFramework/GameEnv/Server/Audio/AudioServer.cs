using IAServer;
using System;
using UnityEngine;

namespace IAFramework
{
    public class AudioServer : BaseServer
    {
        private AudioModule audioModule;

        public override void OnInit()
        {
            GameObject audioModuleGo = new GameObject("AudioModule");
            audioModule = audioModuleGo.AddComponent<AudioModule>();
            audioModule.Init();

            GameGlobal.Instance.AddChild(audioModuleGo);
        }

        #region 音量、播放控制

        /// <summary>
        /// 全局音量
        /// </summary>
        public float GlobalVolume
        {
            get { return audioModule.GlobalVolume; }
            set { audioModule.GlobalVolume = value; }
        }

        /// <summary>
        /// 背景音乐音量
        /// </summary>
        public float BGVolume
        {
            get { return audioModule.BGVolume; }
            set { audioModule.BGVolume = value; }
        }

        /// <summary>
        /// 特效音乐音量
        /// </summary>
        public float AudioVolume
        {
            get { return audioModule.AudioVolume; }
            set { audioModule.AudioVolume = value; }
        }

        /// <summary>
        /// 静音
        /// </summary>
        public bool IsMute
        {
            get { return audioModule.IsMute; }
            set { audioModule.IsMute = value; }
        }

        /// <summary>
        /// 背景音乐暂停
        /// </summary>
        public bool IsPause
        {
            get { return audioModule.IsPause; }
            set { audioModule.IsPause = value; }
        }

        #endregion

        #region 背景音乐

        /// <summary>
        /// 播放背景音乐
        /// </summary>
        /// <param name="pClipName"></param>
        /// <param name="pLoop"></param>
        /// <param name="pVolume"></param>
        /// <param name="pFadeOutTime"></param>
        /// <param name="pFadeInTime"></param>
        public void PlayBGM(string pClipName, float pVolume = -1, float pFadeOutTime = 0, float pFadeInTime = 0)
        {
            AudioClip clip = GameEnv.Asset.LoadAudioClip(pClipName);
            if (clip == null)
            {
                Debug.LogError($"播放背景音乐失败，没有对应音效:{pClipName}");
                return;
            }

            audioModule.PlayBGM(clip, pVolume, pFadeOutTime, pFadeInTime);
        }

        /// <summary>
        /// 播放背景音乐
        /// </summary>
        /// <param name="pClipNames"></param>
        /// <param name="pVolume"></param>
        /// <param name="pFadeOutTime"></param>
        /// <param name="pFadeInTime"></param>
        public void PlayBGMS(string[] pClipNames, float pVolume = -1, float pFadeOutTime = 0, float pFadeInTime = 0)
        {
            AudioClip[] clips = new AudioClip[pClipNames.Length];
            for (int i = 0; i < pClipNames.Length; i++)
            {
                AudioClip clip = GameEnv.Asset.LoadAudioClip(pClipNames[i]);
                if (clip == null)
                {
                    Debug.LogError($"播放背景音乐失败，没有对应音效:{pClipNames[i]}");
                    return;
                }
                clips[i] = clip;
            }

            audioModule.PlayBGMS(clips, pVolume, pFadeOutTime, pFadeInTime);
        }

        /// <summary>
        /// 停止背景音乐
        /// </summary>
        public void StopBGM()
        {
            audioModule.StopBGM();
        }

        /// <summary>
        /// 暂停背景音乐
        /// </summary>
        public void PauseBGM()
        {
            audioModule.PauseBGM();
        }

        /// <summary>
        /// 恢复背景音乐
        /// </summary>
        public void UnPauseBGM()
        {
            audioModule.UnPauseBGM();
        }

        #endregion

        #region 特效音乐

        /// <summary>
        /// 播放2D音效
        /// </summary>
        /// <param name="pClipName"></param>
        /// <param name="pGo"></param>
        /// <param name="pVolumeScale"></param>
        /// <param name="pCallBack"></param>
        public void PlayAudio(string pClipName, float pVolumeScale = 1, Action pCallBack = null)
        {
            AudioClip clip = GameEnv.Asset.LoadAudioClip(pClipName);
            if (clip == null)
            {
                Debug.LogError($"播放2D音效失败，没有对应音效:{pClipName}");
                return;
            }

            audioModule.PlayAudio(clip, pVolumeScale, pCallBack);
        }

        #endregion
    }
}
