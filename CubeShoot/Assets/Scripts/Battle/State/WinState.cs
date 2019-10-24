/****************************************************
    文件：WinState.cs
	作者：Solis
    邮箱: zhaotianshinai@gmail.com
    日期：2019/9/22 23:53:57
	功能：Nothing
*****************************************************/

using UnityEngine;

public class WinState : IState
{
    public void Enter(EntityBase entityBase, params object[] para)
    {
        entityBase.currentAniState = AniState.Win;

    }

    public void Process(EntityBase entityBase, params object[] para)
    {
        entityBase.PlayAudio();
        entityBase.canControl = false;
        entityBase.StopFire();
    }

    public void Exit(EntityBase entityBase, params object[] para)
    {
    }
}