/****************************************************
    �ļ���SniperView.cs
	���ߣ�947064269
    ����: 947064269@qq.com
    ���ڣ�2019/9/9 9:26:8
	���ܣ��ѻ�ǹ�����Դ��
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperView : GunViewBase {

    private LineRenderer m_LineRenderer;
    public LineRenderer M_LineRenderer
    {
        get { return m_LineRenderer; }
        set { m_LineRenderer = value;
            m_LineRenderer.startWidth = 0.15f;
            m_LineRenderer.endWidth = 0.15f;
        }
    }

    protected override void InitHoldPoseValue()
    {
        StartPos = M_Transform.localPosition;
        StartRot = M_Transform.localRotation.eulerAngles;
        EndPos = new Vector3(-0.065f, -1.85f, 0.25f);
        EndRot = new Vector3(2.8f, 1.3f, 0.08f);
    }

    protected override void FindGunPoint()
    {
        GunPoint = M_Transform.Find(PathDefine.gunTransName + "/" + PathDefine.sniperGunPointTransName);
    }

    protected override void Init()
    {
        m_Weapon = transform.Find(PathDefine.gunTransName + "/" + Constants.SniperGunName).transform;
  
    }
}
