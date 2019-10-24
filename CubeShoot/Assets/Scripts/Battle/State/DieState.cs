/****************************************************
    文件：DieState.cs
	作者：Solis
    邮箱: zhaotianshinai@gmail.com
    日期：2019/8/21 3:49:45
	功能：Nothing
*****************************************************/

using UnityEngine;

public class DieState : IState
{
    public void Enter(EntityBase entityBase, params object[] para)
    {
        entityBase.currentAniState = AniState.Die;
       
    }

    public void Process(EntityBase entityBase, params object[] para)
    {
        entityBase.PlayAudio();
        entityBase.canControl = false;
        entityBase.StopFire();
        //entityBase.SetAction(Constants.AniActionDie);
        //entityBase.GetController().ctrl.enabled = false;
        //if (entityBase.entityType == EntityType.Monster)
        //{
        //    TimerSvc.Instance.AddTimeTask((int tid) =>
        //    {
        //        entityBase.SetActive(false);
        //    }, 5000);
        //}

    }

    public void Exit(EntityBase entityBase, params object[] para)
    {
    }

   
}