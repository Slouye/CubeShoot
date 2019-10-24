/****************************************************
    文件：ExitGamePanel.cs
	作者：Solis
    邮箱: zhaotianshinai@gmail.com
    日期：2019/9/26 22:24:19
	功能：Nothing
*****************************************************/

using PEProtocol;
using UnityEngine;
using UnityEngine.UI;

public class ExitGamePanel : BasePanel
{
    public Button btnSure;
    public Button btnCancel;


    protected override void InitPanel()
    {
        base.InitPanel();
        if (isFirst)
        {
            RegisterBtn();
            isFirst = false;
        }
    }

    public void RegisterBtn()
    {
        btnSure.onClick.AddListener(() =>
        {
            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.ReqQuitGame,
            };
            netSvc.SendMsg(msg);
        });
        btnCancel.onClick.AddListener(() =>
        {
            UIManager.Instance.PopPanel();
        });
    }


}