/****************************************************
    文件：BattleMgr.cs
	作者：Solis
    邮箱: zhaotianshinai@gmail.com
    日期：2019/8/18 14:1:29
	功能：战场管理器
*****************************************************/

using PEProtocol;
using System.Collections.Generic;
using UnityEngine;

public class BattleMgr : MonoBehaviour
{
    private ResSvc resSvc;
    private TimerSvc timerSvc;

    private StateMgr stateMgr;

    private Dictionary<RoleType, ControllerBase> controllerDic = new Dictionary<RoleType, ControllerBase>();

    private Dictionary<RoleType, LineRenderer> lineRendererDic = new Dictionary<RoleType, LineRenderer>();

    private PlayerController playerController;

    public PlayerEntity playerEntity;

    private int lastShootBulletID;

    private Transform bulletParent;
    private Transform effectParent;
    private ObjectPool bulletPool;
    private ObjectPool laserBulletPool;
    private ObjectPool hitEffectPool;
    private ObjectPool fireEffectPool;
    private ObjectPool shellEffectPool;

    private int damageID;

    /// <summary>
    /// 可以优化代码结构。
    /// </summary>
    /// <param name="battlePropDic"></param>
    public void Init(Dictionary<int, BattleProp> battlePropDic)
    {
        resSvc = ResSvc.Instance;
        timerSvc = TimerSvc.Instance;

        //初始化各管理器
        stateMgr = gameObject.AddComponent<StateMgr>();
        stateMgr.Init();

        bulletParent = GameObject.Find("Temp/Bullets").transform;
        effectParent = GameObject.Find("Temp/Effects").transform;
        bulletPool = gameObject.AddComponent<ObjectPool>();
        laserBulletPool = gameObject.AddComponent<ObjectPool>();
        hitEffectPool = gameObject.AddComponent<ObjectPool>();
        fireEffectPool = gameObject.AddComponent<ObjectPool>();
        shellEffectPool = gameObject.AddComponent<ObjectPool>();
   
        //找到自己的战场参数
        foreach (var item in battlePropDic)
        {
            RoleType roleType = (RoleType)item.Key;
            ControllerBase controllerBase = null;
            GameObject playerGO = null;
            if (roleType == BattleSys.Instance.GetCurrentRoleType())
            {
                playerGO = resSvc.LoadPrefab(PathDefine.PlayerPath);
                #region 设置摄像机以及添加玩家控制器
                switch (roleType)
                {
                    case RoleType.Red:
                        Camera.main.transform.position = new Vector3(0, 1f, -30f);
                        Camera.main.transform.localEulerAngles = new Vector3(0, 0, 0);
                        break;
                    case RoleType.Blue:
                        Camera.main.transform.position = new Vector3(0, 1f, 30f);
                        Camera.main.transform.localEulerAngles = new Vector3(0, 180, 0);
                        break;
                    case RoleType.Yellow:
                        Camera.main.transform.position = new Vector3(-30, 1, 30f);
                        Camera.main.transform.localEulerAngles = new Vector3(0, 180, 0);
                        break;
                    case RoleType.Green:
                        Camera.main.transform.position = new Vector3(30, 1, 30f);
                        Camera.main.transform.localEulerAngles = new Vector3(0, 180, 0);
                        break;
                    case RoleType.Purple:
                        Camera.main.transform.position = new Vector3(-30, 1, -30f);
                        Camera.main.transform.localEulerAngles = new Vector3(0, 0, 0);
                        break;
                    case RoleType.Black:
                        Camera.main.transform.position = new Vector3(30, 1, -30f);
                        Camera.main.transform.localEulerAngles = new Vector3(0, 0, 0);
                        break;
                    case RoleType.Orange:
                        Camera.main.transform.position = new Vector3(22, 1, -5f);
                        Camera.main.transform.localEulerAngles = new Vector3(0, 0, 0);
                        break;
                    case RoleType.White:
                        Camera.main.transform.position = new Vector3(-25, 1, -5f);
                        Camera.main.transform.localEulerAngles = new Vector3(0, 0, 0);
                        break;
                }
                playerController = playerGO.AddComponent<PlayerController>();
                #endregion
                playerController.SetRoleType(roleType);
                controllerDic.Add(roleType, playerController);


                #region 设置实体数据
                playerEntity = new PlayerEntity(this,stateMgr);

                playerEntity.SetCtrl(playerController);
                playerEntity.SetBattleProps(battlePropDic.TryGet(item.Key));
                //考虑了扩展游戏到AI
                playerEntity.SetRoleType(roleType);
                playerEntity.Born();
                #endregion

                controllerBase = playerController;

            }
            else
            {
                //设置远程角色
                playerGO = resSvc.LoadPrefab(PathDefine.PlayerPath);
                if (playerGO.GetComponent<CharacterController>()!=null)
                {
                    Destroy(playerGO.GetComponent<CharacterController>());
                }
                RemoteController remoteController = playerGO.AddComponent<RemoteController>();
                remoteController.SetRoleType(roleType);
                controllerDic.Add(roleType, remoteController);


                #region 设置实体数据
                RemoteEntity remoteEntity = new RemoteEntity(this, stateMgr);

                remoteEntity.SetCtrl(remoteController);
                remoteEntity.SetBattleProps(battlePropDic.TryGet(item.Key));
                //考虑了扩展游戏到AI
                //remoteEntity.SetRoleType(roleType);
                remoteEntity.Born();
                #endregion

                controllerBase = remoteController;
            }

            switch (roleType)
            {
                case RoleType.Red:
                    playerGO.transform.position = new Vector3(0, 0f, -30f); ;
                    playerGO.transform.localEulerAngles = new Vector3(0, 0, 0);
                    playerGO.name = "Red";
                    break;
                case RoleType.Blue:
                    playerGO.transform.position = new Vector3(0, 0, 30f);
                    playerGO.transform.localEulerAngles = new Vector3(0, 180, 0);
                    playerGO.name = "Blue";
                    break;
                case RoleType.Yellow:
                    playerGO.transform.position = new Vector3(-30, 0, 30f);
                    playerGO.transform.localEulerAngles = new Vector3(0, 180, 0);
                    playerGO.name = "Yellow";
                    break;
                case RoleType.Green:
                    playerGO.transform.position = new Vector3(30, 0, 30f);
                    playerGO.transform.localEulerAngles = new Vector3(0, 180, 0);
                    playerGO.name = "Green";
                    break;
                case RoleType.Purple:
                    playerGO.transform.position = new Vector3(-30, 0, -30f);
                    playerGO.transform.localEulerAngles = new Vector3(0, 0, 0);
                    playerGO.name = "Purple";
                    break;
                case RoleType.Black:
                    playerGO.transform.position = new Vector3(30, 0, -30f);
                    playerGO.transform.localEulerAngles = new Vector3(0, 0, 0);
                    playerGO.name = "Black";
                    break;
                case RoleType.Orange:
                    playerGO.transform.position = new Vector3(22, 0, -5f);
                    playerGO.transform.localEulerAngles = new Vector3(0, 0, 0);
                    playerGO.name = "Orange";
                    break;
                case RoleType.White:
                    playerGO.transform.position = new Vector3(-25, 0, -5f);
                    playerGO.transform.localEulerAngles = new Vector3(0, 0, 0);
                    playerGO.name = "White";
                    break;
            }

            #region 添加枪支
            Transform gunTrans = controllerBase.transform.Find(PathDefine.gunTransName).GetComponent<Transform>();
            GameObject go = null;
            switch (battlePropDic.TryGet(item.Key).weapenType)
            {
                case WeapenType.Rifle:
                    go = resSvc.LoadPrefab(PathDefine.RiflePrefab);
                    go.name = "Rifle";
                    go.transform.SetParent(gunTrans);
                    go.transform.localPosition = new Vector3(0, 0, -0.1f);
                    go.transform.localEulerAngles = Vector3.zero;
                    go.transform.localScale = Vector3.one;
                    break;
                case WeapenType.Sniper:
                    go = resSvc.LoadPrefab(PathDefine.SniperPrefab);
                    go.name = "Sniper";
                    LineRenderer line = go.AddComponent<LineRenderer>();
                    line.material = Resources.Load<Material>("ResMaterials/Gun/Lin");
                    line.startWidth = 0.3f;
                    line.endWidth = 0.3f;
                    line.enabled = false;
                    lineRendererDic.Add(roleType, line);
                    go.transform.SetParent(gunTrans);
                    go.transform.localPosition = new Vector3(0, 0, -0.1f);
                    go.transform.localRotation = Quaternion.Euler(Vector3.zero);
                    go.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
                    break;
            }
            #endregion
        



        }

        //设置我的武器
        playerEntity.SetPlayerWeapen();

        PECommon.Log("Init BattleMgr Done.");
      
    }

    public void SetSelfPlayerMove(Vector2 distance)
    {
        if (playerEntity==null)
        {
            return;
        }

        //设置玩家移动
        playerEntity.SetMove(distance);

    }
    public void SetSelfPlayerRot(Vector2 dir)
    {
        playerEntity.SetDir(dir);
    }


    public Vector2 GetMoveInput()
    {
        return BattleSys.Instance.GetPlayerMoveInput();
    }

    public ControllerBase GetController(RoleType roleType)
    {
        ControllerBase controller = null;
        if (controllerDic.TryGetValue(roleType, out controller))
        {
            return controller;
        }

        return null;
    }

    /// <summary>
    /// 更新人物动作
    /// </summary>
    public void UpdateModelAction()
    {
        playerController.UpdateModelAction();
    }

    public void SetChatacterShootBullet(AllSyncBullet bulletInfo)
    {
        PECommon.Log("在自己的客户端上实例化一个子弹。");

        Vector3 pos = UnityTools.GetV3Value(bulletInfo.originPos);
        Vector3 rot = UnityTools.GetV3Value(bulletInfo.originRot);
        Vector3 dir = UnityTools.GetV3Value(bulletInfo.dir);
        //-----------------------发射子弹
        GameObject tempBullet = null;
        if (bulletPool.Data())
        {
            tempBullet = bulletPool.GetObject();
            tempBullet.transform.position = pos;
            tempBullet.transform.eulerAngles = rot;
            tempBullet.gameObject.SetActive(true);
        }
        else
        {
            tempBullet = GameObject.Instantiate(Resources.Load<GameObject>(bulletInfo.prefabName), pos, Quaternion.Euler(rot), bulletParent);
            tempBullet.name = "Bullet";
        }

        if (tempBullet.GetComponent<Bullet>()==null)
        {
            tempBullet.AddComponent<Bullet>();
        }
        Bullet bullet = tempBullet.GetComponent<Bullet>();
        PECommon.Log("这颗子弹的ID从" + bullet.damageID + "改变到" + (bullet.damageID + 1));
        if (bullet.damageID <= lastShootBulletID)
        {
            bullet.damageID = lastShootBulletID + 1;
        }
        else
        {
            bullet.damageID += 1;
        }
        lastShootBulletID = bullet.damageID;
        bullet.SetCasterRoleType(bulletInfo.roleType);
        bullet.PlayAudio();
        if (bulletInfo.shootSky == 1)
        {
            bulletInfo.force *= 10;
        }
        bullet.Shoot(dir, bulletInfo.force, bulletInfo.damage, bulletPool);
        
        //-------------------------------开火特效
        GameObject fireEffect = null;
        if (fireEffectPool.Data())
        {
            fireEffect = fireEffectPool.GetObject();
            fireEffect.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            fireEffect.transform.position = pos;
            //fireEffect.transform.eulerAngles = rot;
            fireEffect.gameObject.SetActive(true);
        }
        else
        {
            fireEffect = GameObject.Instantiate(Resources.Load<GameObject>(bulletInfo.fireEffect), pos, Quaternion.identity, effectParent);
            fireEffect.name = "FireEffect";
            fireEffect.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        }
        timerSvc.AddTimeTask((int tid) => 
        {
            fireEffect.gameObject.SetActive(false);
            fireEffectPool.AddObject(fireEffect);
        },500);
        //-------------------------------弹壳特效
        GameObject shellEffect = null;
        Vector3 newPos = new Vector3(pos.x, pos.y, pos.z - 0.5f);
        if (shellEffectPool.Data())
        {
            shellEffect = shellEffectPool.GetObject();
            shellEffect.transform.position = newPos;
            shellEffect.transform.eulerAngles = rot;
            shellEffect.GetComponent<ShellEffect>().ShellFly(new Vector3(0, 1, Random.Range(0.05f, 0.2f)), 300);
            shellEffect.gameObject.SetActive(true);
        }
        else
        {
            shellEffect = GameObject.Instantiate(Resources.Load<GameObject>(bulletInfo.shellEffect), newPos, Quaternion.Euler(rot), effectParent);
            shellEffect.name = "ShellEffect";
            shellEffect.GetComponent<ShellEffect>().ShellFly(new Vector3(0,1,Random.Range(0.05f,0.2f)), 300);
        }
        if (shellEffect.GetComponent<ShellEffect>()!=null)
        {
            shellEffect.GetComponent<ShellEffect>().PlayAudio();
        }
        timerSvc.AddTimeTask((int tid) =>
        {
            shellEffect.gameObject.SetActive(false);
            shellEffectPool.AddObject(shellEffect);
        }, 500);

    }

    public void SetChatacterShootLaser(AllSyncLaser laserInfo)
    {
        PECommon.Log("在自己的客户端上显示一条激光。1111111111111111111111111");

        Vector3 originPos = UnityTools.GetV3Value(laserInfo.originPos);
        Vector3 endPos = UnityTools.GetV3Value(laserInfo.endPos);

        LineRenderer line = lineRendererDic.TryGet(laserInfo.roleType);
        line.enabled = true;
     
        GameObject tempLaserBullet = null;
        if (laserBulletPool.Data())
        {
            tempLaserBullet = laserBulletPool.GetObject();
            tempLaserBullet.transform.position = originPos;
            tempLaserBullet.gameObject.SetActive(true);
        }
        else
        {
            tempLaserBullet = GameObject.Instantiate(Resources.Load<GameObject>(PathDefine.LaserBullet), originPos, Quaternion.identity, bulletParent);
            tempLaserBullet.name = "LaserBullet";
        }
        if (tempLaserBullet.GetComponent<Bullet>() == null)
        {
            tempLaserBullet.AddComponent<Bullet>();
        }
        Bullet bullet = tempLaserBullet.GetComponent<Bullet>();
        PECommon.Log("这颗子弹的ID从" + bullet.damageID + "改变到" + (bullet.damageID + 1));
        if (bullet.damageID <= lastShootBulletID)
        {
            bullet.damageID = lastShootBulletID + 1;
        }
        else
        {
            bullet.damageID += 1;
        }
        lastShootBulletID = bullet.damageID;
        bullet.SetCasterRoleType(laserInfo.roleType);
        bullet.PlayAudio();
        if (laserInfo.shootSky == 1)
        {
            line.useWorldSpace = false;
            line.SetPosition(0, Vector3.forward);
            line.SetPosition(1, endPos);
            timerSvc.AddTimeTask((int tid) =>
            {
                laserBulletPool.AddObject(tempLaserBullet);
            }, 500);
        }
        else
        {
            line.useWorldSpace = true;
            line.SetPosition(0, originPos);
            line.SetPosition(1, endPos);
            bullet.Shoot(endPos - originPos, 3000, laserInfo.damage, laserBulletPool);
        }
        //-------------------------------开火特效
        GameObject fireEffect = null;
        if (fireEffectPool.Data())
        {
            fireEffect = fireEffectPool.GetObject();
            fireEffect.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            fireEffect.transform.position = originPos;
            fireEffect.gameObject.SetActive(true);
        }
        else
        {
            fireEffect = GameObject.Instantiate(Resources.Load<GameObject>(laserInfo.fireEffect), originPos, Quaternion.identity, effectParent);
            fireEffect.name = "FireEffect";
            fireEffect.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        }
        timerSvc.AddTimeTask((int tid) =>
        {
            fireEffect.gameObject.SetActive(false);
            fireEffectPool.AddObject(fireEffect);
        }, 500);
        TimerSvc.Instance.AddTimeTask((int tid) =>
        {
            line.enabled = false;
        }, 200);


    }


    public void ShowHitEffect(Vector3 hitPos)
    {
        GameObject tempParticle = null;
        if (hitEffectPool.Data())
        {
            tempParticle = hitEffectPool.GetObject();
            tempParticle.transform.position = hitPos;
            tempParticle.transform.localScale = Vector3.one;
            tempParticle.gameObject.SetActive(true);
        }
        else
        {
            tempParticle = GameObject.Instantiate(Resources.Load<GameObject>(PathDefine.HitEffect), hitPos, Quaternion.identity, effectParent);
            tempParticle.name = "Particle";
        }
        timerSvc.AddTimeTask((int tid) =>
        {
            tempParticle.SetActive(false);
            hitEffectPool.AddObject(tempParticle);
        }, 1200);
    }

    public void SetGunFaceWall(bool isFace)
    {
        playerEntity.SetGunFaceWall(isFace);
    }


    public void DestroyPlayerModel(RoleType roleType)
    {
        ControllerBase controllerBase =  controllerDic.TryGet(roleType);
        if (controllerBase!=null)
        {
            Destroy(controllerBase.gameObject);
            controllerDic.Remove(roleType);
        }
    }

    public void JumpCompelate()
    {
        playerEntity.JumpCompelate();
    }
    public void FallCompelate()
    {
        playerEntity.FallCompelate();
    }
}