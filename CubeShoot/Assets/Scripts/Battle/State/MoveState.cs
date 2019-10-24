/****************************************************
	文件：MoveState.cs
	作者：Solis
	邮箱: zhaotianshinai@gmail.com
	日期：2019/08/18 19:36   	
	功能：移动状态
*****************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class MoveState : IState
{
    public void Enter(EntityBase entityBase, params object[] para)
    {
        entityBase.currentAniState = AniState.Move;
        PECommon.Log("EnterMove");
    }

    public void Process(EntityBase entityBase, params object[] para)
    {
        entityBase.PlayAudio();
        PECommon.Log("ProcessrMove");
    }

    public void Exit(EntityBase entityBase, params object[] para)
    {
        PECommon.Log("ExitMove");
    }

}