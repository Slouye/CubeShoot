/****************************************************
	文件：NetSvc.cs
	作者：Solis
	邮箱: zhaotianshinai@gmail.com
	日期：2019/07/30 16:39   	
	功能：网络服务
*****************************************************/
using System.Collections.Generic;
using PENet;
using PEProtocol;

class NetSvc
{
    private static NetSvc instance;
    public static NetSvc Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new NetSvc();
            }
            return instance;
        }
    }

    private static readonly string obj = "lock";
    private Queue<PackMsg> msgPackQue = new Queue<PackMsg>();

    public void Init()
    {
        PESocket<ServerSession, GameMsg> server = new PESocket<ServerSession, GameMsg>();
        server.StartAsServer(SvcCfg.ServerIP, SvcCfg.ServerPort);

        PETool.LogMsg("NetSvc Init Down");
    }

    /// <summary>
    /// 添加到消息队列
    /// </summary>
    public void AddMsgQue(PackMsg pack)
    {
        lock (obj)
        {
            msgPackQue.Enqueue(pack);
        }
    }

    /// <summary>
    /// 逐步处理消息
    /// </summary>
    public void Update()
    {
        if (msgPackQue.Count>0)
        {
            lock (obj)
            {
                PackMsg pack = msgPackQue.Dequeue();
                HandOutMsg(pack);
            }
        }
    }

    /// <summary>
    /// 分发消息
    /// </summary>
    /// <param name="gameMsg"></param>
    private void HandOutMsg(PackMsg pack)
    {
        switch ((CMD)pack.msg.cmd)
        {
            case CMD.None:
                break;
            case CMD.ReqLogin:
                UISys.Instance.ReqLogin(pack);
                break;
            case CMD.ReqReName:
                UISys.Instance.ReqReName(pack);
                break;
            case CMD.ReqCreateRoom:
                UISys.Instance.ReqCreateRoom(pack);
                break;
            case CMD.ReqRoomList:
                UISys.Instance.ReqRoomList(pack);
                break;
            case CMD.ReqJoinRoom:
                UISys.Instance.ReqJoinRoom(pack);
                break;
            case CMD.ReqQuitRoom:
                UISys.Instance.ReqQuitRoom(pack);
                break;
            case CMD.ReqReadyBattle:
                UISys.Instance.ReqReadyBattle(pack);
                break;
            case CMD.ReqChangeWeapen:
                UISys.Instance.ReqChangeWeapen(pack);
                break;
            case CMD.ReqBattle:
                BattleSys.Instance.ReqBattle(pack);
                break;
            case CMD.ReqBattleEnd:
                BattleSys.Instance.ReqBattleEnd(pack);
                break;
            case CMD.ReqSyncMove:
                BattleSys.Instance.ReqSyncMove(pack);
                break;
            case CMD.ReqTakeDamage:
                BattleSys.Instance.ReqTakeDamage(pack);
                break;
            case CMD.ReqSyncRot:
                BattleSys.Instance.ReqSyncRot(pack);
                break;
            case CMD.ReqSyncBullet:
                BattleSys.Instance.ReqSyncBullet(pack);
                break;
            case CMD.ReqSyncLaser:
                BattleSys.Instance.ReqSyncLaser(pack);
                break;
            case CMD.ReqQuitGame:
                BattleSys.Instance.ReqQuitGame(pack);
                break;

        }
    }
 
}

