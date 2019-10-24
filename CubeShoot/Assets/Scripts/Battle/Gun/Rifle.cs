/****************************************************
    文件：Rifle.cs
	作者：947064269
    邮箱: 947064269@qq.com
    日期：2019/9/8 14:56:26
	功能：步枪类
*****************************************************/

using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Rifle : GunControllerBase {
    private RifleView m_RifleView;

    //枪械后坐力Y轴度数
    private float cameraOffsetY;
    private float cameraOffsetX;

    /// <summary>
    /// 初始化
    /// </summary>
    protected override void Init()
    {
        m_RifleView = (RifleView)M_GunViewBase;
        Transform[] childTrans = m_RifleView.m_Weapon.GetComponentsInChildren<Transform>();
        for (int i =0;i< childTrans.Length;i++)
        {
            childTrans[i].gameObject.layer = 8;  //设置到枪模层
        }
    }



    protected override void SendNetMessage()
    {
        Vector3 forceDir = Vector3.zero;
        //枪械颤抖  ,需要停止当前颤抖协程。再重新开启
        StopCoroutine("GunRecoilAnimation");
        StartCoroutine("GunRecoilAnimation", m_RifleView.m_Weapon);
        cameraOffsetY = 0;
        cameraOffsetX = 0;
        currentFireTime = TimerSvc.Instance.GetNowTime();
        //发射子弹的偏移，网络协议加上偏移
        if (currentFireTime - lastFireTime < weapenRocoilTime * 1000)
        {
            PECommon.Log("后坐力起效果");
            cameraOffsetY = Random.Range(1f, weapenRocoil);
            cameraOffsetX = Random.Range(-weapenRocoil, weapenRocoil) * 0.3f;
        }
        lastFireTime = TimerSvc.Instance.GetNowTime();
        if (BattleSys.Instance.battleMgr != null)
        {
            if (BattleSys.Instance.battleMgr.playerEntity != null)
            {
                //人物枪模上升
                BattleSys.Instance.battleMgr.playerEntity.SetDir(new Vector2(-360 + cameraOffsetX, -360 + cameraOffsetY));
            }
        }
     
        if (Hit.point != Vector3.zero)
        {
            forceDir = m_RifleView.GunPoint.forward + (Hit.point - m_RifleView.GunPoint.position);
            shootSky = 0;
        }
        else
        {
            //forceDir = new Vector3(m_RifleView.m_Weapon.rotation.x, m_RifleView.m_Weapon.rotation.y, 1);
            PECommon.Log("向天上射：" + forceDir);
            forceDir = m_RifleView.GunPoint.forward;
            shootSky = 1;
        }
        MyVector3 originPos = UnityTools.GetMyV3Value(m_RifleView.GunPoint.position);
        MyVector3 originRot = UnityTools.GetMyV3Value(m_RifleView.GunPoint.eulerAngles);
        MyVector3 dir = UnityTools.GetMyV3Value(forceDir);
        GameMsg gameMsg = new GameMsg
        {
            cmd = (int)CMD.ReqSyncBullet,
            reqSyncBullet = new ReqSyncBullet
            {
                //prefabName = Constants.BulletPrefab,
                prefabName = BulletPrefab,
                fireEffect = FireEffect,
                shellEffect = ShellEffect,
                originPos = originPos,
                originRot = originRot,
                dir = dir,
                force = Constants.BulletForce,
                shootSky = shootSky,
            }
            //子弹的预制体，子弹的方向，子弹的力，子弹的初始位置。
        };
        netSvc.SendMsg(gameMsg);

        PECommon.Log("发送发射子弹消息");
    }


}
