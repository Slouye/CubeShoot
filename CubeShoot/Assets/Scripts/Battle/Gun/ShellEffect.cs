/****************************************************
    文件：ShellEffect.cs
	作者：Solis
    邮箱: zhaotianshinai@gmail.com
    日期：2019/9/19 23:57:22
	功能：Nothing
*****************************************************/

using UnityEngine;

public class ShellEffect : MonoBehaviour 
{

    private Rigidbody m_Rigidbody;
    private AudioSource m_AudioSource;

    private void Awake()
    {
        m_Rigidbody = gameObject.GetComponent<Rigidbody>();
        m_AudioSource = gameObject.GetComponent<AudioSource>();
    }

    public void ShellFly(Vector3 dir , float force)
    {
        m_Rigidbody.AddForce(dir * force);
    }

    public void PlayAudio()
    {
        TimerSvc.Instance.AddTimeTask((int tid) =>
        {
            if (m_AudioSource.isPlaying)
            {
                m_AudioSource.Stop();
            }
            m_AudioSource.Play();
        }, 300);
    }
}