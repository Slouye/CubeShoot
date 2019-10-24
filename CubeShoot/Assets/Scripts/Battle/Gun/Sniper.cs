/****************************************************
    �ļ���Sniper.cs
	���ߣ�947064269
    ����: 947064269@qq.com
    ���ڣ�2019/9/9 9:23:42
	���ܣ��ѻ�ǹ��
*****************************************************/

using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sniper : GunControllerBase {
    private SniperView m_SniperView;
    //private Transform bulletParent;
    //ǹе������Y�����
    private float cameraOffsetY;

    /// <summary>
    /// ��ʼ��
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
        //ǹе����  ,��Ҫֹͣ��ǰ����Э�̡������¿���
        StopCoroutine("GunRecoilAnimation");
        StartCoroutine("GunRecoilAnimation", m_SniperView.m_Weapon);
        cameraOffsetY = 0;
        currentFireTime = TimerSvc.Instance.GetNowTime();
        //�����ӵ���ƫ�ƣ�����Э�����ƫ��
        if (currentFireTime - lastFireTime < weapenRocoilTime * 1000)
        {
            PECommon.Log("��������Ч��");
            cameraOffsetY = Random.Range(1f, weapenRocoil);
        }
        lastFireTime = TimerSvc.Instance.GetNowTime();
        if (BattleSys.Instance.battleMgr!=null)
        {
            if (BattleSys.Instance.battleMgr.playerEntity!=null)
            {
                //����ǹģ����
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
        PECommon.Log("���ͷ��伤����Ϣ");
    }



}
