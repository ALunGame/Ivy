namespace GameContext
{
    public class AudioSetting 
    {
        public float GlobalVolume = .5f;
        public float BGVolume = 1.0f;
        public float AudioVolume = 1.0f;
        public bool IsMute = false;
    }

    /// <summary>
    /// 设置数据
    /// </summary>
    public class SettingData : ArchiveData
    {
        public AudioSetting Audio = new AudioSetting();
    }
}
