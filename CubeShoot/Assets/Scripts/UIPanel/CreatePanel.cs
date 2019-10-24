/****************************************************
    文件：CreatePanel.cs
	作者：Solis
    邮箱: zhaotianshinai@gmail.com
    日期：2019/9/7 12:47:2
	功能：Nothing
*****************************************************/

using PEProtocol;
using UnityEngine;
using UnityEngine.UI;

public class CreatePanel : BasePanel 
{

    public InputField IFName;

    public void ClickRandBtn()
    {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);

        string rdName = resSvc.GetRDNameData(false);
        IFName.text = rdName;
    }


    public void ClickEnterBtn()
    {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        //如果有输入名字
        if (IFName.text != "")
        {
            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.ReqReName,
                reqReName = new ReqReName
                {
                    name = IFName.text,
                }
            };
            netSvc.SendMsg(msg);
        }
        else
        {
            GameRoot.AddTips("当前名字不符合规范");
        }
    }

}