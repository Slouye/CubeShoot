/****************************************************
    文件：RotState.cs
	作者：Solis
    邮箱: zhaotianshinai@gmail.com
    日期：2019/9/11 12:56:16
	功能：Nothing
*****************************************************/

using UnityEngine;

public class RotState : IState
{
    public void Enter(EntityBase entityBase, params object[] para)
    {
        entityBase.currentAniState = AniState.Rot;
        PECommon.Log("EnterRot");
    }

    public void Process(EntityBase entityBase, params object[] para)
    {
        PECommon.Log("ProcessRot");
    }

    public void Exit(EntityBase entityBase, params object[] para)
    {

        PECommon.Log("ExitRot");
    }
}