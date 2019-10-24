/****************************************************
    文件：NetSvc.cs
	作者：Solis
    邮箱: zhaotianshinai@gmail.com
    日期：2019/9/4 23:9:45
	功能：网络服务
*****************************************************/

using UnityEngine;
using PEProtocol;
using PENet;
using System.Collections.Generic;

public class NetSvc : MonoBehaviour
{
    public static NetSvc Instance = null;
    private PESocket<ClientSession, GameMsg> client;

    private static readonly string obj = "lock";
    private Queue<GameMsg> msgQue = new Queue<GameMsg>();

    public void InitSvc()
    {
        Instance = this;
        client = new PESocket<ClientSession, GameMsg>();
        //client.SetLog(true, (string msg, int lv) =>
        //{
        //    switch (lv)
        //    {
        //        case 0:
        //            break;
        //        case 1:
        //            break;
        //        case 2:
        //            break;
        //        case 3:
        //            break;
        //    }
        //});
        client.StartAsClient(SvcCfg.ServerIP, SvcCfg.ServerPort);
        PECommon.Log("Init NetSvc..." + SvcCfg.ServerIP + " : "+SvcCfg.ServerPort);

    }

    public void SendMsg(GameMsg msg)
    {
        if (client.session != null)
        {
            client.session.SendMsg(msg);
        }
        else
        {
            GameRoot.AddTips("服务器未连接");
            //重新启动网络服务
            InitSvc();
        }
    }

    /// <summary>
    /// 添加到消息队列
    /// </summary>
    public void AddMsgQue(GameMsg msg)
    {
        lock (obj)
        {
            msgQue.Enqueue(msg);
        }
    }
    /// <summary>
    /// 逐步处理消息
    /// </summary>
    public void Update()
    {
        if (msgQue.Count > 0)
        {
            lock (obj)
            {
                GameMsg msg = msgQue.Dequeue();
                HandOutMsg(msg);
            }
        }
    }


    /// <summary>
    /// 分发消息
    /// </summary>
    /// <param name="gameMsg"></param>
    private void HandOutMsg(GameMsg msg)
    {
        //如果有错误
        if (msg.err != (int)ErrorCode.None)
        {
            switch ((ErrorCode)msg.err)
            {
                case ErrorCode.ClientDataError:
                    PECommon.Log("客户端数据异常~");
                    GameRoot.AddTips("客户端数据异常");
                    break;
                case ErrorCode.AcctIsOnline:
                    GameRoot.AddTips("账号已经登录");
                    break;
                case ErrorCode.WrongPassword:
                    GameRoot.AddTips("账号密码错误");
                    break;
                case ErrorCode.NameIsExist:
                    GameRoot.AddTips("名字已经被他人占用");
                    break;
                case ErrorCode.UpdateDBError:
                    PECommon.Log("数据库更新异常~有BUG");
                    //不要让用户看到这个BUG，不然就会觉得这个游戏很垃圾= = 
                    GameRoot.AddTips("网络连接不稳定");
                    break;
                case ErrorCode.RoomNull:
                    GameRoot.AddTips("要加入的房间为空");
                    PECommon.Log("要加入的房间为空。");
                    break;
                case ErrorCode.RoomFull:
                    GameRoot.AddTips("要加入的房间满员了");
                    PECommon.Log("要加入的房间满员了。");
                    break;
                case ErrorCode.LackRoomMember:
                    GameRoot.AddTips("缺少玩家，不能开始游戏。");
                    PECommon.Log("要加入的房间满员了。");
                    break;

            }
            return;
        }
        switch ((CMD)msg.cmd)
        {
            case CMD.None:
                break;
            case CMD.RspLogin:
                UISys.Instance.RspLogin(msg);
                break;
            case CMD.RspReName:
                UISys.Instance.RspReName(msg);
                break;
            case CMD.RspCreateRoom:
                UISys.Instance.RspCreateRoom(msg);
                break;
            case CMD.RspRoomList:
                UISys.Instance.RspRoomList(msg);
                break;
            case CMD.RspJoinRoom:
                UISys.Instance.RspJoinRoom(msg);
                break;
            case CMD.OtherJoinRoom:
                UISys.Instance.OtherJoinRoom(msg);
                break;
            case CMD.OwnerQuitRoom:
                UISys.Instance.OwnerQuitRoom(msg);
                break;
            case CMD.StartBattle:
                UISys.Instance.StartBattle(msg);
                break;
            case CMD.NoFullReady:
                UISys.Instance.NoFullReady(msg);
                break;
            case CMD.RspReadyBattle:
                UISys.Instance.RspReadyBattle(msg);
                break;
            case CMD.OtherChangeReadyState:
                UISys.Instance.OtherChangeReadyState(msg);
                break;
            case CMD.OtherChangeWeapen:
                UISys.Instance.OtherChangeWeapen(msg);
                break;
            case CMD.OtherQuitRoom:
                UISys.Instance.OtherQuitRoom(msg);
                break;
            case CMD.OtherQuitBattle:
                BattleSys.Instance.OtherQuitBattle(msg);
                break;
            case CMD.OtherSyncMove:
                BattleSys.Instance.OtherSyncMove(msg);
                break;
            case CMD.OtherSyncRot:
                BattleSys.Instance.OtherSyncRot(msg);
                break;
            case CMD.RspTakeDamage:
                BattleSys.Instance.RspTakeDamage(msg);
                break;
            case CMD.YourAreDie:
                BattleSys.Instance.YourAreDie(msg);
                break;
            case CMD.YourAreWin:
                BattleSys.Instance.YourAreWin(msg);
                break;
            case CMD.RspBattleEnd:
                BattleSys.Instance.RspBattleEnd(msg);
                break;
            case CMD.AllSyncBullet:
                BattleSys.Instance.AllSyncBullet(msg);
                break;
            case CMD.AllSyncLaser:
                BattleSys.Instance.AllSyncLaser(msg);
                break;
            case CMD.StartPlay:
                BattleSys.Instance.StartPlay(msg);
                break;
            case CMD.ShowTimer:
                BattleSys.Instance.ShowTimer(msg);
                break;
            case CMD.SomeOneDie:
                BattleSys.Instance.SomeOneDie(msg);
                break;
            case CMD.RspQuitGame:
                BattleSys.Instance.RspQuitGame(msg);
                break;

        }

    }

}