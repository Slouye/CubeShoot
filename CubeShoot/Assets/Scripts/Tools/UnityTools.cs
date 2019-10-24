/****************************************************
    文件：UnityTools.cs
	作者：Solis
    邮箱: zhaotianshinai@gmail.com
    日期：2019/9/15 15:16:46
	功能：Nothing
*****************************************************/

using UnityEngine;
using PEProtocol;
using System;

public static class UnityTools
{
    /// <summary>
    /// MyV3解析成V3数值
    /// </summary>
    public static Vector3 GetV3Value(MyVector3 myVector3)
    {
        float x = myVector3.x;
        float y = myVector3.y;
        float z = myVector3.z;
        return new Vector3(x, y, z);
    }
    /// <summary>
    /// V3转换成MyV3
    /// </summary>
    public static MyVector3 GetMyV3Value(Vector3  v3)
    {
        MyVector3 myVector3 = new MyVector3
        {
            x = (float)Math.Round(v3.x, 2),
            y = (float)Math.Round(v3.y, 2),
            z = (float)Math.Round(v3.z, 2),
        };
        return myVector3;
    }

    /// <summary>
    /// 查找最上层父物体
    /// </summary
    public static Transform FindUpParent(Transform son)
    {
        if (son.parent == null)
            return son;
        else
            return FindUpParent(son.parent);
    }


    /// <summary>
    /// 转换为小数点后两位
    /// </summary>
    public static Vector3 RoundTwo(Vector3 v3)
    {
        Vector3 myVector3 = new Vector3
        {
            x = (float)Math.Round(v3.x, 2),
            y = (float)Math.Round(v3.y, 2),
            z = (float)Math.Round(v3.z, 2),
        };
        return myVector3;
    }


    /// <summary>
    /// 根据人物类型返回是否需要增加180度
    /// </summary>
    /// <param name="roleType"></param>
    /// <returns></returns>
    public static CameraState AheadOrBack(RoleType roleType)
    {
        if (roleType == RoleType.Red || roleType == RoleType.Purple || roleType == RoleType.Black || roleType == RoleType.Orange || roleType == RoleType.White)
        {
            return CameraState.Ahead;
        }
        else if (roleType == RoleType.Blue || roleType == RoleType.Yellow || roleType == RoleType.Green)
        {
            return CameraState.Back;
        }
        return CameraState.None;

    }
}
