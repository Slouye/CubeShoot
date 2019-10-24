/****************************************************
    文件：ControllerBase.cs
	作者：Solis
    邮箱: zhaotianshinai@gmail.com
    日期：2019/9/8 20:50:9
	功能：Nothing
*****************************************************/

using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ControllerBase : MonoBehaviour 
{

    public RoleType roleType = RoleType.Blue;

    protected List<Transform> modelItemList = new List<Transform>();

    public CharacterController ctrl;
    public AudioSource audioSource;
    public Transform hpRoot;

    protected bool isMove = false;
    protected bool isRot = false;

    private int timerCount;

    public bool isCommit;

    private Vector2 dis = Vector2.zero;
    public Vector2 Dis
    {
        get
        {
            return dis;
        }

        set
        {
            if (value == Vector2.zero)
            {
                isMove = false;
            }
            else
            {
                isMove = true;
            }

            dis = value;
        }
    }

    private Vector2 dir = Vector2.zero;
    public Vector2 Dir
    {
        get
        {
            return dir;
        }

        set
        {
            if (value == Vector2.zero)
            {
                isRot = false;
            }
            else
            {
                isRot = true;
                dir = value;
            }
            
        }
    }

    protected Transform camTrans;

    protected TimerSvc timerSvc;
    protected ResSvc resSvc;
    protected NetSvc netSvc;
    protected AudioSvc audioSvc;
    protected Dictionary<string, GameObject> fxDic = new Dictionary<string, GameObject>();

    protected virtual void Start()
    {
        timerSvc = TimerSvc.Instance;
        resSvc = ResSvc.Instance;
        netSvc = NetSvc.Instance;
        audioSvc = AudioSvc.Instance;

        ctrl = gameObject.GetComponent<CharacterController>();
        audioSource = gameObject.GetComponent<AudioSource>();

    }

    public virtual void SetRoleType(RoleType roleType)
    {

    }

    //向上跳跃
    public virtual void CharacterJump()
    {
        PECommon.Log("人物跳跃");
    }

    //方向跳跃
    public virtual void DirJump()
    {
        PECommon.Log("人物跳跃");
    }

    public void PlayAudio()
    {
        audioSvc.PlayAudioInGameObject(audioSource);
    }
}