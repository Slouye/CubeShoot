/****************************************************
    文件：AttackState.cs
	作者：Solis
    邮箱: zhaotianshinai@gmail.com
    日期：2019/8/19 0:13:37
	功能：Nothing
*****************************************************/

using UnityEngine;

public class AttackState : IState
{
    public void Enter(EntityBase entityBase, params object[] para)
    {
        entityBase.currentAniState = AniState.Attack;
        //entityBase.curtSkillCfg = ResSvc.Instance.GetSkillCfgData((int)para[0]);
        //PECommon.Log("EnterAttack");
    }

    public void Process(EntityBase entityBase, params object[] para)
    {
        entityBase.PlayAudio();
        //if (entityBase.entityType == EntityType.Player)
        //{
        //    entityBase.canRelaseSkill = false;
        //}
        ////PECommon.Log("ProcessAttack");

        //entityBase.SkillAttack((int)para[0]);
    }

    public void Exit(EntityBase entityBase, params object[] para)
    {
        //entityBase.ExitCurtSkill();
        //PECommon.Log("ExitAttack");
    }

   
}