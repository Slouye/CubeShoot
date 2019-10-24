/****************************************************
    文件：LoginWnd.cs
	作者：Solis
    邮箱: zhaotianshinai@gmail.com
    日期：2019/7/30 17:20:20
	功能：登录注册界面
*****************************************************/

using PEProtocol;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : BasePanel
{
    public InputField iptAcct;
    public InputField iptPass;
    public Button btnEnter;


    protected override void InitPanel()
    {
        base.InitPanel();

        //获取本地存储的账号密码
        if (PlayerPrefs.HasKey("Acct") && PlayerPrefs.HasKey("Pass"))
        {
            iptAcct.text = PlayerPrefs.GetString("Acct");
            iptPass.text = PlayerPrefs.GetString("Pass");
        }
        else
        {
            iptAcct.text = "";
            iptPass.text = "";
        }
    }


    /// <summary>
    /// 点击进入游戏
    /// </summary>
    public void ClickEnterBtn()
    {
        audioSvc.PlayUIAudio(Constants.UILoginBtn);

        string account = iptAcct.text;
        string password = iptPass.text;
        if (account != "" && password != "")
        {
            //更新本地存储的账号密码
            PlayerPrefs.SetString("Acct", account);
            PlayerPrefs.SetString("Pass", password);

            //TODO 发送网络消息，请求登录
            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.ReqLogin,
                reqLogin = new ReqLogin
                {
                    Acct = account,
                    Pass = password,
                }
            };
            netSvc.SendMsg(msg);

        }
        else
        {
            GameRoot.AddTips("账号或密码为空");
        }
    }

}