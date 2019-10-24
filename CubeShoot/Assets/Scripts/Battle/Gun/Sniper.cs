/****************************************************
    文件：Sniper.cs
	作者：947064269
    邮箱: 947064269@qq.com
    日期：2019/9/9 9:23:42
	功能：狙击枪类
*****************************************************/

using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sniper : GunControllerBase {
    private SniperView m_SniperView;
    //private Transform bulletParent;
    //枪械后坐力Y轴度数
    private float cameraOffsetY;

    /// <summary>
    /// 初始化
    /// </summary>
    protected override void Init()
    {
        m_SniperView = (SniperView)M_GunViewBase;
        Transform[] childTrans = m_SniperView.m_Weapon.GetComponentsInChildren<Transform>();
        for (int i = 0; i < childTrans.Length; i++)
        {
            childTrans[i].gameObject.layer = 8;
        }
    }



    protected override void SendNetMessage()
    {
        //枪械颤抖  ,需要停止当前颤抖协程。再重新开启
        StopCoroutine("GunRecoilAnimation");
        StartCoroutine("GunRecoilAnimation", m_SniperView.m_Weapon);
        cameraOffsetY = 0;
        currentFireTime = TimerSvc.Instance.GetNowTime();
        //发射子弹的偏移，网络协议加上偏移
        if (currentFireTime - lastFireTime < weapenRocoilTime * 1000)
        {
            PECommon.Log("后坐力起效果");
            cameraOffsetY = Random.Range(1f, weapenRocoil);
        }
        lastFireTime = TimerSvc.Instance.GetNowTime();
        if (BattleSys.Instance.battleMgr!=null)
        {
            if (BattleSys.Instance.battleMgr.playerEntity!=null)
            {
                //人物枪模上升
                BattleSys.Instance.battleMgr.playerEntity.SetDir(new Vector2(0, -360 + cameraOffsetY));
            }
        }
     
        MyVector3 originPos = UnityTools.GetMyV3Value(m_SniperView.GunPoint.position);
        MyVector3 endPos = null;
        if (Hit.point != Vector3.zero)
        {
            shootSky = 0;
            endPos = UnityTools.GetMyV3Value(Hit.point);
        }
        else
        {
            shootSky = 1;
            endPos = UnityTools.GetMyV3Value(m_SniperView.GunPoint.position);
            endPos.z += 100;
        }
        GameMsg gameMsg = new GameMsg
        {
            cmd = (int)CMD.ReqSyncLaser,
            reqSyncLaser = new ReqSyncLaser
            {
                originPos = originPos,
                endPos = endPos,
                shootSky = shootSky,
                fireEffect = FireEffect,
            }
        };
        netSvc.SendMsg(gameMsg);
        PECommon.Log("发送发射激光消息");
    }



}
