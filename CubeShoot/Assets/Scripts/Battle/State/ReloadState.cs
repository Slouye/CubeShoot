/****************************************************
    文件：ReloadState.cs
	作者：Solis
    邮箱: zhaotianshinai@gmail.com
    日期：2019/9/8 18:38:6
	功能：Nothing
*****************************************************/

using UnityEngine;

public class ReloadState : IState
{
    public void Enter(EntityBase entityBase, params object[] para)
    {
        entityBase.currentAniState = AniState.Reload;
       
    }

    public void Process(EntityBase entityBase, params object[] para)
    {
        entityBase.PlayAudio();
    }

    public void Exit(EntityBase entityBase, params object[] para)
    {
        

    }

}