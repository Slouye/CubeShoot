/****************************************************
    文件：EndBattlePanel.cs
	作者：Solis
    邮箱: zhaotianshinai@gmail.com
    日期：2019/9/15 14:22:21
	功能：Nothing
*****************************************************/

using PEProtocol;
using UnityEngine;
using UnityEngine.UI;

public class EndBattlePanel : BasePanel
{

    public Text txtDefeated;
    public Text txtVictory;
    public Image imgVictory;
    public Image imgDefeated;
    public Button btnBackRoom;


    protected override void InitPanel()
    {
        base.InitPanel();
        SetActive(txtDefeated, false);
        SetActive(txtVictory, false);
        SetActive(imgVictory, false);
        SetActive(imgDefeated, false);
    }

    public void Lose(RoleType casterRoleType)
    {
        PECommon.Log("你输了");
        SetActive(imgDefeated);
        SetActive(txtDefeated);
        SetText(txtDefeated, casterRoleType.ToString() + "干掉了你");
    }


    public void Win(int victoryNum)
    {
        SetActive(imgVictory);
        SetActive(txtVictory);
        SetText(txtVictory, "胜利场数：" + victoryNum);
    }

    public void ReqBattleEnd()
    {
        GameMsg gameMsg = new GameMsg
        {
            cmd = (int)CMD.ReqBattleEnd,
             reqBattleEnd = new ReqBattleEnd
             {
                 roleType = (int)BattleSys.Instance.GetCurrentRoleType(),
             }
        };
        netSvc.SendMsg(gameMsg);
    }

}