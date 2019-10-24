/****************************************************
    文件：HitState.cs
	作者：Solis
    邮箱: zhaotianshinai@gmail.com
    日期：2019/8/21 3:44:53
	功能：Nothing
*****************************************************/

using UnityEngine;

public class HitState : IState
{


    public void Enter(EntityBase entityBase, params object[] para)
    {
        entityBase.currentAniState = AniState.Hit;
        PECommon.Log("EnterHit");
    }

    public void Process(EntityBase entityBase, params object[] para)
    {
        PECommon.Log("ProcessHit");
        //播放音效
        entityBase.PlayAudio();

        if (entityBase.GetMoveInput()==Vector2.zero)
       {
            entityBase.Idle();
        }
        else
        {
            entityBase.Move();
        }

    }

    public void Exit(EntityBase entityBase, params object[] para)
    {
        PECommon.Log("ExitHit");
    }

    //private float GetHitAniLength(EntityBase entityBase)
    //{
    //    AnimationClip[] clips = entityBase.GetAniClips();
    //    for (int i = 0; i< clips.Length; i ++)
    //    {
    //       if (clips[i].name == "Hit" || clips[i].name == "hit" || clips[i].name == "HIT")
    //       {
    //            return clips[i].length;
    //       }
    //    }
    //    return 1;
    //}

}