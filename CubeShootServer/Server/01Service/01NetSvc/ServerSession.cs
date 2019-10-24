/****************************************************
	文件：ServerSession.cs
	作者：Solis
	邮箱: zhaotianshinai@gmail.com
	日期：2019/07/30 16:39   	
	功能：一个网络连接会话
*****************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PEProtocol;
using PENet;

/// <summary>
/// 打包消息类。
/// </summary>
public class PackMsg
{
    public ServerSession session;
    public GameMsg msg;
    public PackMsg(ServerSession session, GameMsg msg)
    {
        this.session = session;
        this.msg = msg;
    }
}

public class ServerSession : PESession<GameMsg>
{

    public int sessionId { get; private set; }

    private Room room;
    public Room Room
    {
        set { room = value; }
        get { return room; }
    }
    private bool readyBattle;
    public bool ReadyBattle
    {
        get { return readyBattle; }
        set { readyBattle = value; }
    }

    protected override void OnConnected()
    {
        sessionId = ServerRoot.Instance.GetSessionId();
    }

    protected override void OnReciveMsg(GameMsg msg)
    {
        NetSvc.Instance.AddMsgQue(new PackMsg(this,msg));
    }

    protected override void OnDisConnected()
    {
        if (Room != null)
        {
            Room.QuitRoom(this);
        }
        UISys.Instance.OffinePlayer(this);
    }
}

