/****************************************************
    文件：RemoteController.cs
	作者：Solis
    邮箱: zhaotianshinai@gmail.com
    日期：2019/9/9 16:55:1
	功能：Nothing
*****************************************************/

using System.Collections.Generic;
using UnityEngine;

public class RemoteController : ControllerBase 
{

    private CameraState cameraState = CameraState.None;

    public override void SetRoleType(RoleType roleType)
    {
        this.roleType = roleType;
        cameraState = UnityTools.AheadOrBack(roleType);
        for (int i = 0; i < transform.childCount; i++)
        {
            modelItemList.Add(transform.GetChild(i));
        }
    }


    /// <summary>
    /// 更新模型的动作(远程模型)
    /// </summary>
    public void UpdateModelAction(Vector3 rot)
    {
        //+180转向
        if (cameraState == CameraState.Back)
        {
            rot.y += 180;
        }
        modelItemList[0].transform.localEulerAngles = rot;
        modelItemList[1].transform.localEulerAngles = rot;
        modelItemList[2].transform.localEulerAngles = new Vector3(modelItemList[2].transform.localEulerAngles.x, rot.y, modelItemList[2].transform.localEulerAngles.z);
    }

}