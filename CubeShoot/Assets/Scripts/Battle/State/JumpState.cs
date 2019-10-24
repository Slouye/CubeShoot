/****************************************************
    文件：JumpState.cs
	作者：Solis
    邮箱: zhaotianshinai@gmail.com
    日期：2019/9/23 0:10:27
	功能：Nothing
*****************************************************/

using UnityEngine;

public class JumpState : IState
{
    public void Enter(EntityBase entityBase, params object[] para)
    {
        entityBase.currentAniState = AniState.Jump;
        //移动跳跃
        if (entityBase.currentAniState == AniState.Move)
        {
            entityBase.controller.DirJump();
        }
        else
        {
            entityBase.controller.CharacterJump();
        }
        PECommon.Log("EnterJump");
    }

    public void Process(EntityBase entityBase, params object[] para)
    {
        entityBase.PlayAudio();
        PECommon.Log("ProcessrJump");
    }

    public void Exit(EntityBase entityBase, params object[] para)
    {
        PECommon.Log("ExitJump");
    }

}