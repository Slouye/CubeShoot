/****************************************************
    文件：BornState.cs
	作者：Solis
    邮箱: zhaotianshinai@gmail.com
    日期：2019/9/8 18:35:8
	功能：Nothing
*****************************************************/

public class BornState : IState
{
    public void Enter(EntityBase entityBase, params object[] para)
    {
        entityBase.currentAniState = AniState.Born;
        entityBase.canControl = false;
        PECommon.Log("EnterBorn");
    }

    public void Process(EntityBase entityBase, params object[] para)
    {
        PECommon.Log("ProcessBorn");
        entityBase.PlayAudio();
        TimerSvc.Instance.AddTimeTask((int tid) =>
        {
            entityBase.canControl = true;
            entityBase.Idle();
        }, 3000);

    }

    public void Exit(EntityBase entityBase, params object[] para)
    {
        BattleSys.Instance.playerControlPanel.JumpBtn.interactable = true;
        PECommon.Log("ExitBorn");
    }


}