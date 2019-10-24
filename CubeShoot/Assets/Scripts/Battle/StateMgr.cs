/****************************************************
    文件：StateMgr.cs
	作者：Solis
    邮箱: zhaotianshinai@gmail.com
    日期：2019/9/8 18:23:59
	功能：Nothing
*****************************************************/

using System.Collections.Generic;
using UnityEngine;

public class StateMgr : MonoBehaviour 
{
    private Dictionary<AniState, IState> fsm = new Dictionary<AniState, IState>();

    public void Init()
    {
        fsm.Add(AniState.Born, new BornState());
        fsm.Add(AniState.Idle, new IdleState());
        fsm.Add(AniState.Move, new MoveState());
        fsm.Add(AniState.Attack, new AttackState());
        fsm.Add(AniState.Reload, new ReloadState());
        fsm.Add(AniState.Hit, new HitState());
        fsm.Add(AniState.Die, new DieState());
        fsm.Add(AniState.Win, new WinState());
        fsm.Add(AniState.Jump, new JumpState());
        fsm.Add(AniState.Fall, new FallState());
        fsm.Add(AniState.Rot, new RotState());

        PECommon.Log("Init StateMgr Done.");
    }

    public void ChangeStatus(EntityBase entity, AniState targetState, params object[] args)
    {
        if (entity.currentAniState == targetState)
        {
            return;
        }

        if (fsm.ContainsKey(targetState))
        {
            if (entity.currentAniState != AniState.None)
            {
                fsm[entity.currentAniState].Exit(entity, args);
            }
            fsm[targetState].Enter(entity, args);
            fsm[targetState].Process(entity, args);
        }
    }

}