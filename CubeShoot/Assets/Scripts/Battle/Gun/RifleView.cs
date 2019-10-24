/****************************************************
    文件：RifleView.cs
	作者：947064269
    邮箱: 947064269@qq.com
    日期：2019/9/8 14:57:18
	功能：步枪组件资源类
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class RifleView : GunViewBase {

    protected override void InitHoldPoseValue()
    {
        StartPos = M_Transform.localPosition;
        StartRot = M_Transform.localRotation.eulerAngles;
        EndPos = new Vector3(0, 0.08f, -0.267f);
        EndRot = new Vector3(0f, 0f, 0f);
    }

    protected override void FindGunPoint()
    {
        GunPoint = M_Transform.Find(PathDefine.gunTransName + "/" + PathDefine.rifleGunPointTransName).transform;
    }

    protected override void Init()
    {
        m_Weapon = transform.Find(PathDefine.gunTransName + "/" + Constants.RifleGunName).transform;
    }
}
