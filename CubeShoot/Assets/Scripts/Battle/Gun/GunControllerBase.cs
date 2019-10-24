/****************************************************
    文件：GunControllerBase.cs
	作者：947064269
    邮箱: 947064269@qq.com
    日期：2019/9/8 10:1:52
	功能：枪械父类控制器
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GunControllerBase : MonoBehaviour {


    [SerializeField] protected WeapenType weapenType;
    [SerializeField] protected int weapenShootRate;
    [SerializeField] protected int weapenReload;
    [SerializeField] protected int weapenDamage;
    [SerializeField] protected int weapenFrontBullet;
    [SerializeField] protected int weapenBackBullet;
    [SerializeField] protected float weapenRocoil;            //枪械后座上升的度数
    [SerializeField] protected float weapenRocoilTime;            //枪械后座的回弹时间
    [SerializeField] protected string BulletPrefab;            
    [SerializeField] protected string FireEffect;            
    [SerializeField] protected string ShellEffect;            

    private bool isFirst = true;
    //射击的计时ID
    protected int shootTaskID = -10;

    protected double lastFireTime;
    protected double currentFireTime;

    private int currentFrontBullet;
    public int CurrentFrontBullet
    {
        get { return currentFrontBullet; }
        set { currentFrontBullet = value;
            //因为枪械初始化完毕之后才初始化面板
            if (!isFirst)
            {
                BattleSys.Instance.SetTxtBullet(currentFrontBullet, currentBackBullet);
            }
        }
    }
    private int currentBackBullet;
    public int CurrentBackBullet
    {
        get { return currentBackBullet; }
        set {  currentBackBullet = value; }
    }

    protected NetSvc netSvc;

    public void Init(WeapenCfg weapenCfg)
    {
        switch (weapenCfg.weapenType)
        {
            case "Rifle":
                weapenType = WeapenType.Rifle;
                break;
            case "Sniper":
                weapenType = WeapenType.Sniper;
                break;
        }
        weapenShootRate = weapenCfg.weapenShootRate;
        weapenReload = weapenCfg.weapenReload;
        weapenFrontBullet = weapenCfg.weapenFrontBullet;
        weapenBackBullet = weapenCfg.weapenBackBullet;
        weapenDamage = weapenCfg.weapenDamage;
        weapenRocoil = weapenCfg.weapenRocoil;
        weapenRocoilTime = weapenCfg.weapenRocoilTime;
        BulletPrefab = weapenCfg.BulletPrefab;
        FireEffect = weapenCfg.FireEffect;
        ShellEffect = weapenCfg.ShellEffect;

        CurrentFrontBullet = weapenFrontBullet;
        isFirst = false;
        CurrentBackBullet = weapenBackBullet;


        netSvc = NetSvc.Instance;

        Init();
    }

    private GunViewBase m_GunViewBase;
    private Ray ray;
    private RaycastHit hit;

    public bool canShoot = true;   //是否可以开枪
    public bool isShoot = false;   //是否正在开枪
    public bool isHold = false;            //是否开镜中 
    protected bool isLoadBullet = false;
    //是否射击天空
    protected int shootSky;

    public GunViewBase M_GunViewBase { get { return m_GunViewBase; } set { m_GunViewBase = value; } }

    public Ray MyRay { get { return ray; } set { ray = value; } }

    public RaycastHit Hit { get { return hit; } set { hit = value; } }



    void Update()
    {
        ShootReady();
        if (isShoot)
        {
            Shoot();
        }
    }


    /// <summary>
    /// 开关镜
    /// </summary>
    public void SwitchGlass()
    {
        if (!isHold)
        {//开镜
            isHold = true;
            M_GunViewBase.EnterHoldPose(weapenType, 0.2f, 40);
            BattleSys.Instance.Aim(true);
        }
        else
        {//关镜
            isHold = false;
            M_GunViewBase.ExitHoldPose(weapenType, 0.2f, 60);
            BattleSys.Instance.Aim(false);
        }

    }

    /// <summary>
    /// 射击准备
    /// </summary>
    protected void ShootReady()
    {
        //射线（起始位置，方向）
        Vector2 v = new Vector2(Screen.width / 2 * 1.0f, Screen.height / 2 * 1.0f);
        if (Physics.Raycast(Camera.main.ScreenPointToRay(v), out hit))
        {
        }
        else
        {
            hit.point = Vector3.zero;
        }
    }

    /// <summary>
    /// （对象池）协创延迟
    /// </summary>
    public IEnumerator Delay(ObjectPool pool, GameObject go, float tiem)
    {
        yield return new WaitForSeconds(tiem);
        pool.AddObject(go);
        Debug.Log("放入");
    }


    /// <summary>
    /// 对象池拿出子弹初始化
    /// </summary>
    public void PoolCreateBulleInit(GameObject tempBullet)
    {
        tempBullet.transform.position = m_GunViewBase.GunPoint.position;
        tempBullet.transform.rotation = m_GunViewBase.GunPoint.rotation;
    }

    protected virtual void Init() { }

    public virtual void Shoot()
    {
        if (canShoot)
        {
            PECommon.Log("Rilfe Shoot!!!!!!!!!!!!!!!!!!");
            canShoot = false;
            CurrentFrontBullet--;
            PECommon.Log("当前子弹数:" + CurrentFrontBullet);
            if (CurrentFrontBullet <= -1)
            {
                CurrentFrontBullet = 0;
                return;
            }
            if (isLoadBullet)
            {
                return;
            }
            SendNetMessage();
            shootTaskID = TimerSvc.Instance.AddTimeTask((int tid) =>
            {
                canShoot = true;
            }, 1000 / weapenShootRate);
        }

    }

    protected virtual void SendNetMessage() { }

    public virtual void ReloadBullet()
    {
        if (CurrentFrontBullet < weapenFrontBullet)
        {
            if (CurrentBackBullet > 0)
            {
                PECommon.Log("正在换子弹");
                if (shootTaskID != -10)
                {
                    TimerSvc.Instance.RemoveTake(shootTaskID);
                    shootTaskID = -10;
                }
                isLoadBullet = true;
                canShoot = false;
                isHold = false;
                BattleSys.Instance.Aim(false);
                M_GunViewBase.ExitHoldPose(weapenType, 0.2f, 60);
                BattleSys.Instance.SetLoadBulletState(true);
                BattleSys.Instance.ShowReloadBulletAni(weapenReload);
                TimerSvc.Instance.AddTimeTask((int tid) =>
                {
                    if (CurrentBackBullet >= weapenFrontBullet)
                    {
                        CurrentBackBullet = CurrentBackBullet - (weapenFrontBullet - CurrentFrontBullet);
                        CurrentFrontBullet = weapenFrontBullet;
                    }
                    else if (CurrentFrontBullet + CurrentBackBullet > weapenFrontBullet)
                    {
                        CurrentBackBullet = CurrentFrontBullet + CurrentBackBullet - weapenFrontBullet;
                        CurrentFrontBullet = weapenFrontBullet;
                    }
                    else
                    {
                        int tempCurrentBackBullet = CurrentBackBullet;
                        CurrentBackBullet = 0;
                        CurrentFrontBullet = CurrentFrontBullet + tempCurrentBackBullet;
                    }
                    isLoadBullet = false;
                    canShoot = true;
                    BattleSys.Instance.HideReloadBulletAni();
                    BattleSys.Instance.SetLoadBulletState(false);
                    PECommon.Log("换弹完毕");
                }, weapenReload, PETimeUnit.Second);
            }
        }
    }


    /// <summary>
    /// 枪械颤抖动画
    /// </summary>
    /// <returns></returns>
    public IEnumerator GunRecoilAnimation(Transform pivot_Transform)
    {
        //定义停止时间
        float stopTime = Time.time + weapenRocoilTime;  //根据武器自身
        //定义开始时位置尾巴的旋转角度
        Quaternion startRot = Quaternion.Euler(new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0));
        //定义旋转范围
        float range = 1.0f;
        //无用ref参数 用于SmoothDamp第三个参数
        float vel = 0;
        //如国当前时间小于停止时间就执行
        while (Time.time < stopTime)
        {
            //定义旋转角度
            Quaternion rot = Quaternion.Euler(new Vector3(Random.Range(-range, range), Random.Range(-range, range), 0)) * startRot;
            //设置旋转角度
            pivot_Transform.localRotation = rot;
            //平滑阻尼
            range = Mathf.SmoothDamp(range, 0, ref vel, stopTime - Time.time);
            yield return null;
        }
        //修正颤抖后枪的位置
        m_GunViewBase.m_Weapon.DOLocalRotate(Vector3.zero, 0.2f);
    }


    public void SetGunFaceWall(bool isFace)
    {
        if (isFace)
        {
            PECommon.Log("枪怼到墙了");
        }
        else
        {
            m_GunViewBase.m_Weapon.eulerAngles = Vector3.zero;
            PECommon.Log("枪回到正常位置");
        }
      
    }
}
