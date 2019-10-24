/****************************************************
    文件：ClientSession.cs
	作者：Solis
    邮箱: zhaotianshinai@gmail.com
    日期：2019/9/4 23:6:21
	功能：Nothing
*****************************************************/

using PENet;
using PEProtocol;

public class ClientSession : PESession<GameMsg>
{
    protected override void OnConnected()
    {
        PECommon.Log("连接上服务器端");
    }

    protected override void OnReciveMsg(GameMsg msg)
    {
        PECommon.Log("服务器端回应" + ((CMD)msg.cmd).ToString());
        NetSvc.Instance.AddMsgQue(msg);
    }

    protected override void OnDisConnected()
    {
        PECommon.Log("断开连接");
    }
}