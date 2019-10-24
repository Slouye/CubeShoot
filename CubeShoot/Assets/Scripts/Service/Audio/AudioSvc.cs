/****************************************************
    文件：AudioSvc.cs
	作者：Solis
    邮箱: zhaotianshinai@gmail.com
    日期：2019/7/30 17:20:20
	功能：声音播放服务
*****************************************************/

using UnityEngine;

public class AudioSvc : MonoBehaviour
{
    public static AudioSvc Instance = null;
    public AudioSource bgAudio;
    public AudioSource uiAudio;
    public AudioClip[] stateAudio;
    public AudioClip[] uiClickAudio;

    public void InitSvc()
    {
        stateAudio = Resources.LoadAll<AudioClip>("ResAudio/State");
        uiClickAudio = Resources.LoadAll<AudioClip>("ResAudio/UI");
        Instance = this;
        PECommon.Log("Init AudioSvc...");
    }


    public void PlayBGMusic(string name, bool isLoop = true)
    {
        AudioClip audio = ResSvc.Instance.LoadAudio(PathDefine.AudioPath + "/" + name, true);
        if (bgAudio.clip == null || bgAudio.clip.name != audio.name)
        {
            bgAudio.clip = audio;
            bgAudio.loop = isLoop;
            bgAudio.Play();
        }
    }

    public void PlayUIAudio(string name)
    {
        AudioClip audio = ResSvc.Instance.LoadAudio(PathDefine.AudioPath + "/" + name, true);
        if (audio != null)
        {
            uiAudio.clip = audio;
            uiAudio.Play();
        }
    }

    public void PlayAudioInPlayer(string name)
    {
        AudioSource audioSource = GameObject.FindGameObjectWithTag("Player").GetComponent<AudioSource>();
        audioSource.clip = ResSvc.Instance.LoadAudio(PathDefine.AudioPath + "/" + name, true);
        audioSource.Stop();
        audioSource.Play();
    }

    public void SetBGAidioVolume(float volume)
    {
        bgAudio.volume = volume;
    }

    public void SetUIAidioVolume(float volume)
    {
        uiAudio.volume = volume;
    }

    public void PlayAudioInGameObject(AudioSource audioSource)
    {
        audioSource.clip = stateAudio[Random.Range(0, stateAudio.Length)];
        audioSource.Stop();
        audioSource.Play();
    }
    public void PlayAudioInUI(AudioSource audioSource)
    {
        audioSource.clip = uiClickAudio[Random.Range(0, uiClickAudio.Length)];
        audioSource.Stop();
        audioSource.Play();
    }

}