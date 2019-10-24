/****************************************************
	文件：LoginSys.cs
	作者：Solis
	邮箱: zhaotianshinai@gmail.com
	日期：2019/07/30 16:40   	
	功能：登录业务系统
*****************************************************/
using PEProtocol;
using System.Collections.Generic;

public class UISys
{
    private static UISys instance;
    public static UISys Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new UISys();
            }
            return instance;
        }
    }

    private CacheSvc cacheSvc;
    private TimerSvc timerSvc;

    public List<Room> listRoom = new List<Room>();

    private int alreadyRoom;

    public void Init()
    {
        cacheSvc = CacheSvc.Instance;
        timerSvc = TimerSvc.Instance;
        PECommon.Log("UISys Init Down");
    }

    public void ReqLogin(PackMsg pack)
    {
        GameMsg gameMsg = new GameMsg
        {
            cmd = (int)CMD.RspLogin,
        };
        //当前账号是否已经上线
        ReqLogin reqLogin = pack.msg.reqLogin;
        //已经上线，不准顶号
        if (cacheSvc.isOnline(reqLogin.Acct))
        {
            gameMsg.err = (int)ErrorCode.AcctIsOnline;
        }
        //没上线：上线
        else
        {
            PlayerData pd = cacheSvc.GetPlayerData(reqLogin.Acct, reqLogin.Pass);
            //账号是否正确
            if (pd==null)
            {
                gameMsg.err = (int)ErrorCode.WrongPassword;
            }
            //正确
            else
            {
                gameMsg.rspLogin = new RspLogin
                {
                    playerData = pd
                };

                //缓存账号资料
                cacheSvc.CacheAccountData(reqLogin.Acct, pack.session, pd);
            }
        }
        //回应客户端
        pack.session.SendMsg(gameMsg);

    }

    /// <summary>
    /// 更改名字
    /// </summary>
    /// <param name="pack"></param>
    public void ReqReName(PackMsg pack)
    {
        string name = pack.msg.reqReName.name;
         GameMsg gameMsg = new GameMsg
        {
            cmd = (int)CMD.RspReName
        };
        //名字存在
        if (cacheSvc.IsNameExist(name))
        {
            gameMsg.err = (int)ErrorCode.NameIsExist;
        }
        else
        {
            //更新缓存
            PlayerData playerData = cacheSvc.GetPlayerDataBySession(pack.session);
            playerData.name = name;
            //如果升级数据库不成功
            if (!cacheSvc.UpdatePlayerData(playerData.id,playerData))
            {
                gameMsg.err = (int)ErrorCode.UpdateDBError;
            }
            else
            {
                gameMsg.rspReName = new RspReName
                {
                    name = playerData.name,
                };
            }
        }



        pack.session.SendMsg(gameMsg);
    }



    public void ReqCreateRoom(PackMsg pack)
    {
        PlayerData playerData = cacheSvc.GetPlayerDataBySession(pack.session);
        //房间号等于最新的连接数TODO
        alreadyRoom++;
        //(防止房主挂掉了退出了但是别人没退这个房间没销毁，再创房间别人进不来)
        Room room = new Room(alreadyRoom, cacheSvc);
        //Room room = new Room(playerData.id, cacheSvc);
        room.AddUser(pack.session);
        listRoom.Add(room);
    
        GameMsg gameMsg = new GameMsg
        {
            cmd = (int)CMD.RspCreateRoom,
            rspCreateRoom = new RspCreateRoom
            {
                roleType = room.GetBatterPropBySession(pack.session).roleType,
                id = room.GetRoomID(),
                name = playerData.name,
                victoryNum = playerData.victoryNum,
            },

        };

        pack.session.SendMsg(gameMsg);
    }

    public void ReqRoomList(PackMsg pack)
    {
        List<RspCreateRoom> ownerDataList = new List<RspCreateRoom>();
        for (int i = 0; i < listRoom.Count; i++)
        {
            if (listRoom[i].GetRoomStateIsWaitngJoin())
            {
                ownerDataList.Add(listRoom[i].GetOwnerData());
            }
        }

        GameMsg gameMsg = new GameMsg
        {
            cmd = (int)CMD.RspRoomList,
            rspRoomList = new RspRoomList
            {
                rspCreateRoomList = ownerDataList
            }
        };
        pack.session.SendMsg(gameMsg);
    }

    public void ReqJoinRoom(PackMsg pack)
    {
        GameMsg gameMsg = new GameMsg
        {
            cmd = (int)CMD.RspJoinRoom
        };
        ReqJoinRoom reqJoinRoom = pack.msg.reqJoinRoom;
        Room room = GetRoomByID(reqJoinRoom.id);
        if (room == null)
        {
            gameMsg.err = (int)ErrorCode.RoomNull;
        }
        else if (room.GetRoomStateIsWaitngJoin() == false)
        {
            gameMsg.err = (int)ErrorCode.RoomFull;
        }
        else
        {
            room.AddUser(pack.session);
            gameMsg.rspJoinRoom = new RspJoinRoom
            {
                memberDataList = room.GetAllRoomMemberData(),
                roleType = room.GetBatterPropBySession(pack.session).roleType,
            };
            //在房间内广播除自己以外的。
            //TODO
            PlayerData playerData = cacheSvc.GetPlayerDataBySession(pack.session);
            PlayerRoomData playerRoomData = new PlayerRoomData
            {
                isRoomOwner = false,
                name = playerData.name,
                victoryNum = playerData.victoryNum,
                weapenType = 0,
                isReady = false,
            };
            GameMsg broadGameMsg = new GameMsg
            {
                cmd = (int)CMD.OtherJoinRoom,
                otherJoinRoom = new OtherJoinRoom
                {
                    playerRoomData = playerRoomData,
                }
            };
            room.BroadcastMessage(pack.session, broadGameMsg);
        }

        pack.session.SendMsg(gameMsg);
    }

    public void ReqQuitRoom(PackMsg pack)
    {

        GameMsg gameMsg = new GameMsg();

        bool isRoomOwner = IsRoomOwner(pack.session);
        Room room = pack.session.Room;
        if (isRoomOwner)
        {
            gameMsg.cmd = (int)CMD.OwnerQuitRoom;
            //TODO 是房主的话
            room.BroadcastMessage(null, gameMsg);
            room.CloseRoom();
        }
        else
        {
            pack.session.Room.RemoveUser(pack.session);
            gameMsg.cmd = (int)CMD.OtherQuitRoom;
            gameMsg.otherQuitRoom = new OtherQuitRoom
            {
                
                name = cacheSvc.GetPlayerDataBySession(pack.session).name,
                //memberDataList = room.GetAllRoomMemberData(),
            };
            //TODO 一个玩家退出房间后告诉所有玩家当前房间内所有的玩家信息
            room.BroadcastMessage(pack.session, gameMsg);
            //自己退出。
            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.OwnerQuitRoom
            };
            pack.session.SendMsg(msg);
        }
    }

    public void ReqReadyBattle(PackMsg pack)
    {
        GameMsg gameMsg = new GameMsg();
        ReqReadyBattle reqReadyBattle = pack.msg.reqReadyBattle;
        Room room = pack.session.Room;
        //是房主点击了准备战斗（开始游戏）
        if (room.GetOwnerClient() == pack.session)
        {
            //房间的人都准备了
            //if (room.ReadyCount >= 0)  //临时测试
            if (room.ReadyCount == room.GetRoomMemberCount() - 1)
            {
                if (room.GetRoomMemberCount()==1)
                {
                    gameMsg.err = (int)ErrorCode.LackRoomMember;
                    pack.session.SendMsg(gameMsg);
                    return;
                }
                gameMsg.cmd = (int)CMD.StartBattle;
                gameMsg.startBattle = new StartBattle();
                List<ServerSession> tempList = room.GetClientRoom();
                Dictionary<int, BattleProp> battlePropDic = new Dictionary<int, BattleProp>();
                for (int i = 0;i< tempList.Count;i++)
                {
                    BattleProp battleProp = room.GetBatterPropBySession(tempList[i]);
                    battlePropDic.Add((int)battleProp.roleType, battleProp);
                }
                gameMsg.startBattle.battlePropDic = battlePropDic;
                for (int i = 0; i < tempList.Count; i++)
                {
                    tempList[i].SendMsg(gameMsg);
                }
                room.SetRoomPlayerData();
                room.StartTimer();
                room.RoomState = RoomState.Battle;
            }
            else if (room.ReadyCount >= room.GetRoomMemberCount() - 1)
            {
                gameMsg.err = (int)ErrorCode.RoomError;
                pack.session.SendMsg(gameMsg);
            }
            else
            {
                gameMsg.cmd = (int)CMD.NoFullReady;
                pack.session.SendMsg(gameMsg);
            }
        }
        else
        {
            if (reqReadyBattle.isReady)
            {
                room.ReadyCount += 1;
                pack.session.ReadyBattle = true;
            }
            else
            {
                room.ReadyCount -= 1;
                pack.session.ReadyBattle = false;
            }
            string name = cacheSvc.GetPlayerDataBySession(pack.session).name;
            room.ChangePlayerState(name, reqReadyBattle.isReady);

            gameMsg.cmd = (int)CMD.RspReadyBattle;
            gameMsg.rspReadyBattle = new RspReadyBattle
            {
                isReady = reqReadyBattle.isReady,
            };
            pack.session.SendMsg(gameMsg);
            //告诉其他房间内的用户 此用户的操作。
            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.OtherChangeReadyState,
                otherChangeReadyState = new OtherChangeReadyState
                {
                    name = name,
                    isReady = reqReadyBattle.isReady
                },
            };
            room.BroadcastMessage(pack.session, msg);
        }

    }


    public void ReqChangeWeapen(PackMsg pack)
    {
        ReqChangeWeapen reqChangeWeapen = pack.msg.reqChangeWeapen;
        Room room = pack.session.Room;
        room.ChangeWeapenType(pack.session, reqChangeWeapen.weapenType);
        PlayerData playerData = cacheSvc.GetPlayerDataBySession(pack.session);
        GameMsg gameMsg = new GameMsg
        {
            cmd = (int)CMD.OtherChangeWeapen,
            otherChangeWeapen = new OtherChangeWeapen
            {
                name = playerData.name,
                weapenType = reqChangeWeapen.weapenType,
            },
        };
        room.BroadcastMessage(pack.session, gameMsg);
    }

    public Room GetRoomByID(int roomID)
    {
        foreach (Room room in listRoom)
        {
            if (room.GetRoomID() == roomID)
            {
                return room;
            }
        }
        return null;
    }

    private bool IsRoomOwner(ServerSession session)
    {
        if (session.Room == null)
        {
            return false;
        }
        return session == session.Room.GetOwnerClient();
    }

    /// <summary>
    /// 移除房间
    /// </summary>
    public void RemoveRoom(Room room)
    {
        if (listRoom != null && room != null)
        {
            listRoom.Remove(room);
        }
    }


    /// <summary>
    /// 玩家下线。
    /// </summary>
    public void OffinePlayer(ServerSession session)
    {
        PlayerData playerData = cacheSvc.GetPlayerDataBySession(session);
        if (playerData != null)
        {
            cacheSvc.ClearOnlinePlayerDate(session);
        }

    }
}

