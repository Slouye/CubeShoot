/****************************************************
	文件：IdleState.cs
	作者：Solis
	邮箱: zhaotianshinai@gmail.com
	日期：2019/08/18 19:34   	
	功能：站立状态
*****************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class IdleState : IState
{

    public void Enter(EntityBase entityBase, params object[] para)
    {
        entityBase.currentAniState = AniState.Idle;

        entityBase.SetMove(Vector2.zero);
        PECommon.Log("EnterIdle");
    }

    public void Process(EntityBase entityBase, params object[] para)
    {
        entityBase.PlayAudio();
        if (entityBase.GetMoveInput() != Vector2.zero)
        {
            entityBase.SetMove(entityBase.GetMoveInput());
            entityBase.Move();
        }
     
        PECommon.Log("ProcessIdle");
    }

    public void Exit(EntityBase entityBase, params object[] para)
    {
        PECommon.Log("ExitIdle");

    }

}

