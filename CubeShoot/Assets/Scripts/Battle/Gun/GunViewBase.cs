/****************************************************
    文件：GunViewBase.cs
	作者：947064269
    邮箱: 947064269@qq.com
    日期：2019/9/8 10:2:13
	功能：枪械组件资源父类
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using DG.Tweening;
public class GunViewBase : MonoBehaviour {
    private Transform m_Transform;
    private Animator m_Animator;
    private Camera m_EnvCamera;

    public Transform m_Weapon;

    //开镜动画位置优化
    private Vector3 startPos;
    private Vector3 startRot;
    private Vector3 endPos;
    private Vector3 endRot;

    //private Transform gunStar;          //准星
    private Transform gunPoint;         //枪口位置

    private Camera fovCamera;

    private GameObject prefab_GunStar;  //准星预制体

    public Transform M_Transform { get { return m_Transform; } }
  
    public Animator M_Animator { get { return m_Animator; } }

    public Vector3 StartPos { get { return startPos; } set { startPos = value; } }
    public Vector3 StartRot { get { return startRot; } set { startRot = value; } }
    public Vector3 EndPos { get { return endPos; } set { endPos = value; } }
    public Vector3 EndRot { get { return endRot; } set { endRot = value; } }

    public Camera FovCamera { get { return fovCamera; } set { fovCamera = value; } }

    //public Transform GunStar { get { return gunStar; } }
    public Transform GunPoint { get { return gunPoint; } set { gunPoint = value; } }

    protected virtual void Awake()
    {
        m_Transform = gameObject.GetComponent<Transform>();
        m_Animator = gameObject.GetComponent<Animator>();
        fovCamera = transform.Find(PathDefine.gunTransName + "/FovCamera").GetComponent<Camera>();
        //prefab_GunStar = Resources.Load<GameObject>("Gun/GunStar");
        //gunStar = GameObject.Instantiate<GameObject>(prefab_GunStar, GameObject.Find("Canvas/MainPanel").transform).transform;

        InitHoldPoseValue();
        FindGunPoint();
        Init();
    }


    ////激活时调用
    //private void OnEnable()
    //{
    //    ShowStar();
    //}

    ////不激活时调用
    //private void OnDisable()
    //{
    //    HideStar();
    //}

    /// <summary>
    /// 进入开镜状态-->动作优化
    /// </summary>
    public void EnterHoldPose(WeapenType weapenType, float time = 0.2f, float fov = 40)
    {

        if (weapenType==WeapenType.Rifle)
        {//步枪
         //startPos = Camera.main.transform.localPosition;
         //Camera.main.transform.parent = m_Weapon.transform;
         //Camera.main.transform.localPosition = EndPos;
         //Camera.main.transform.localRotation = m_Weapon.transform.rotation;
         //Camera.main.fieldOfView = fov;

         //无视掉武器层
            Camera.main.cullingMask = Camera.main.cullingMask & ~(1 << LayerMask.NameToLayer("GunModel"));
            fovCamera.gameObject.SetActive(true);
            BattleSys.Instance.SetImgSight(PathDefine.AimSightImg);
        }
        else if (weapenType == WeapenType.Sniper)
        {//狙击枪
            BattleSys.Instance.SetSniperAim(true);
            Camera.main.fieldOfView = 20;
        }
        //m_GunStar.SetActive(false);
    }

    /// <summary>
    /// 退出开镜状态-->动作优化
    /// </summary>
    public void ExitHoldPose(WeapenType weapenType, float time = 0.2f, float fov = 60)
    {
        if (weapenType == WeapenType.Rifle)
        {//步枪
         //Camera.main.transform.parent = null;
         //Camera.main.transform.localPosition = startPos;
         //Camera.main.fieldOfView = fov;

         //检测武器层
            Camera.main.cullingMask = Camera.main.cullingMask | (1 << LayerMask.NameToLayer("GunModel"));
            fovCamera.gameObject.SetActive(false);
            BattleSys.Instance.SetImgSight(PathDefine.GunSightImg);
        }
        else if (weapenType == WeapenType.Sniper)
        {//狙击枪
            BattleSys.Instance.SetSniperAim(false);
            Camera.main.fieldOfView = fov;
        }
        //m_GunStar.SetActive(true);
    }


    protected virtual void Init() { }
    /// <summary>
    /// 开镜关镜动作4个字段设置
    /// </summary>
    protected virtual void InitHoldPoseValue() { }

    /// <summary>
    /// 查找枪口
    /// </summary>
    protected virtual void FindGunPoint() { }


}
