using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MusicMgr : BaseManager<MusicMgr>
{
    //唯一的背景音乐组件
    public static AudioSource bkMusic = null;
    //音乐大小
    private float bkValue = 1;

    //音效依附对象
    private GameObject soundObj = null;
    //音效列表
    private List<AudioSource> soundList = new List<AudioSource>();
    //音效大小
    private float soundValue = 1;

    public MusicMgr()
    {
        MonoMgr.GetInstance().AddUpdateListener(Update);
    }

    private void Update()
    {
        for( int i = soundList.Count - 1; i >=0; --i )
        {
            if(soundList[i]==null||!soundList[i].isPlaying)
            {
                GameObject.Destroy(soundList[i]);
                soundList.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="name"></param>
    public void PlayBkMusic()
    {
        bkMusic.Play();
    }

    /// <summary>
    /// 暂停背景音乐
    /// </summary>
    public void PauseBKMusic()
    {
        if (bkMusic == null)
            return;
        bkMusic.Pause();
    }

    /// <summary>
    /// 停止背景音乐
    /// </summary>
    public void StopBKMusic()
    {
        if (bkMusic == null)
            return;
        bkMusic.Stop();
    }

    /// <summary>
    /// 改变背景音乐 音量大小
    /// </summary>
    /// <param name="v"></param>
    public void ChangeBKValue(float v)
    {
        bkValue = v;
        if (bkMusic == null)
            return;
        bkMusic.volume = bkValue;
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    public void PlaySound()
    {
        bkMusic.Play();
    }

    /// <summary>
    /// 改变音效声音大小
    /// </summary>
    /// <param name="value"></param>
    public void ChangeSoundValue( float value )
    {
        soundValue = value;
        for (int i = 0; i < soundList.Count; ++i)
            soundList[i].volume = value;
    }

    /// <summary>
    /// 停止音效
    /// </summary>
    public void StopSound(AudioSource source)
    {
        if( soundList.Contains(source) )
        {
            soundList.Remove(source);
            source.Stop();
            GameObject.Destroy(source);
        }
    }
}
