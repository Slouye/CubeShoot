/****************************************************
    文件：FallState.cs
	作者：Solis
    邮箱: zhaotianshinai@gmail.com
    日期：2019/9/22 23:58:5
	功能：Nothing
*****************************************************/

using UnityEngine;

public class FallState : IState
{
    public void Enter(EntityBase entityBase, params object[] para)
    {
        entityBase.currentAniState = AniState.Fall;
        PECommon.Log("EnterFall");
    }

    public void Process(EntityBase entityBase, params object[] para)
    {
        entityBase.PlayAudio();
        //entityBase.controller.CharacterJump();
        PECommon.Log("ProcessrFall");
    }

    public void Exit(EntityBase entityBase, params object[] para)
    {
       
        PECommon.Log("ExitFall");
        BattleSys.Instance.ChangeJumpBtnInteractable(true);
    }

}