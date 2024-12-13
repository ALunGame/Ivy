using IAUI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AudioBeatChecker : MonoBehaviour
{
    double dspSongStartTime; //开始播放的时间偏移
    float songPosition;
    

    //每经过一拍需要消耗的时间
    public float secPerBeat;

    [Header("每分钟节拍数")]
    public float BPM;
    public AudioSource musicSource; //装载音乐的 AudioSource

    public Slider slider;

    public RectTransform clickTip;

    UnityEvent OnBeat;

    int currBeat;

    int lastBeat; //用来记录上一个经过的 beat 的计数器
    float currentBeatPosition;

    int nextBeat; 
    float nextBeatPosition;

    float currentBeat;
    float fRange;

    void Update()
    {
        songPosition = (float)(AudioSettings.dspTime - dspSongStartTime); //核心：获取当前播放位置
        currBeat = (int)(songPosition / secPerBeat);

        nextBeat = currBeat + 1;
        nextBeatPosition = nextBeat * secPerBeat;

        UpdateInfos();
        UpdateProcess();

        if (UnityEngine.Input.GetMouseButtonUp(0))
        {
            OnClick();
        }
    }

    private void UpdateInfos()
    {
        TextUtil.SetText(transform, "Infos/NextBeat/Value", nextBeat.ToString());
        TextUtil.SetText(transform, "Infos/NextBeatPos/Value", nextBeatPosition.ToString());
        TextUtil.SetText(transform, "Infos/CurrPos/Value", songPosition.ToString());
    }

    private void UpdateProcess()
    {
        int tBeatLoopCnt = (int)(currBeat / 4.0f);
        float value = (songPosition - (tBeatLoopCnt * 4 * secPerBeat)) / (4 * secPerBeat);
        slider.value = value;   
    }

    void UpdateClockSetting()
    {
        //更新 secPerBeat
        secPerBeat = 1f / (BPM * musicSource.pitch / 60f);
        lastBeat = 0;

        //获取新的播放位置
        songPosition = (float)(AudioSettings.dspTime - dspSongStartTime);
        currentBeat = songPosition / secPerBeat;

        //调整 songPosition 的偏移
        var newSongPosition = secPerBeat * currentBeat;
        dspSongStartTime += songPosition - newSongPosition;
    }

    public void OnClick()
    {
        int tBeatLoopCnt = (int)(currBeat / 4.0f);
        float value = (songPosition - (tBeatLoopCnt * 4 * secPerBeat)) / (4 * secPerBeat);
        clickTip.anchoredPosition = new Vector2(value * 600, -25);


        TextUtil.SetText(transform, "Infos/InputRange/Value", (nextBeatPosition - songPosition).ToString());

        Debug.LogError($"Click-->{value}::{value * 600}");
    }

    public void Play()
    {
        musicSource.Play();
        dspSongStartTime = AudioSettings.dspTime;

        UpdateClockSetting();
    }
}
