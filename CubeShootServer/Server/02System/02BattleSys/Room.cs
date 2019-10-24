/****************************************************
	文件：Room.cs
	作者：Solis
	邮箱: zhaotianshinai@gmail.com
	日期：2019/08/28 18:55   	
	功能：房间
*****************************************************/
using System.Collections.Generic;
using System.Threading;
using PEProtocol;


public class Room
{
    private List<ServerSession> clientRoom = new List<ServerSession>();
    private int roleTypeIndex;

    public int takeDamageID = 0;
    public RoleType casterRoleType = RoleType.None;

    public List<ServerSession> GetClientRoom()
    {
        return clientRoom;
    }

    private RoomState roomState = RoomState.WaitingJoin;
    public RoomState RoomState
    {
        get
        {
            return roomState;
        }
        set { roomState = value; }
    }

    private Dictionary<ServerSession, BattleProp> battleDataDic = new Dictionary<ServerSession, BattleProp>();
    private Dictionary<RoleType,int> playerHPDic = new Dictionary<RoleType, int>();

    public Dictionary<string, bool> playerStateDic = new Dictionary<string, bool>();

    private int roomID;


    public void ChangeWeapenType(ServerSession session, int weapenType)
    {
       BattleProp battleProp = null;
       if (battleDataDic.TryGetValue(session, out battleProp))
       {
            battleProp.weapenType = (WeapenType)weapenType;
            battleProp.weapenCfgID = weapenType;
       }
    }

    public RoleType GetPropRoleType(ServerSession session)
    {
        BattleProp battleProp = null;
        if (battleDataDic.TryGetValue(session, out battleProp))
        {
            return battleProp.roleType;
        }
        return RoleType.None;
    }


    public void SetRoomPlayerData()
    {
        foreach (var item in battleDataDic.Values)
        {
            playerHPDic.Add(item.roleType, item.playerHp);
        }
    }

    public int SetSomeOneHurt(RoleType casterRoleType, RoleType targetRoleType, int hurt)
    {
        int playerHp = 0;
        if (playerHPDic.TryGetValue(targetRoleType, out playerHp))
        {
            playerHp -= hurt;
        }
        playerHPDic.Remove(targetRoleType);
        playerHPDic.Add(targetRoleType, playerHp);
        #region Test
        foreach (var item in playerHPDic)
        {
            //TODO  有人挂掉了
            if (item.Value<=0)
            {
                foreach (var item2 in battleDataDic)
                {
                    if (item2.Value.roleType == targetRoleType)
                    {
                        GameMsg gameMsg = new GameMsg
                        {
                            cmd = (int)CMD.YourAreDie,
                            yourAreDie = new YourAreDie
                            {
                                //传谁干掉了你
                                casterRoleType = casterRoleType,
                            }
                        };
                        item2.Key.SendMsg(gameMsg);
                        GameMsg msg = new GameMsg
                        {
                            cmd = (int)CMD.SomeOneDie,
                            someOneDie = new SomeOneDie
                            {
                                roleType = (int)targetRoleType,
                            }
                        };
                        BroadcastMessage(null, msg);
                    }
                }
               
                playerHPDic.Remove(targetRoleType);
                break;
            }
           
        }
        //胜利！
        if (playerHPDic.Count == 1)
        {
            foreach (var item in playerHPDic)
            {
                foreach (var item2 in battleDataDic)
                {
                    if (item.Key == item2.Value.roleType)
                    {
                        PlayerData playerData = cacheSvc.GetPlayerDataBySession(item2.Key);
                        int victoryNum = playerData.victoryNum + 1;
                        playerData.victoryNum = victoryNum;
                        cacheSvc.UpdatePlayerData(playerData.id, playerData);
                        GameMsg gameMsg = new GameMsg
                        {
                            cmd = (int)CMD.YourAreWin,
                            yourAreWin = new YourAreWin
                            {
                                //传胜利场数过去:
                                victoryNum = victoryNum,
                            }
                        };
                        item2.Key.SendMsg(gameMsg);
                        break;
                    }
                }
            }

        }
        #endregion
        return playerHp;
    }

    private int readyCount;
    public int ReadyCount
    {
        get { return readyCount; }
        set { readyCount = value; }
    }

    private CacheSvc cacheSvc;
    public Room(int roomID, CacheSvc cacheSvc)
    {
        if (roomID == int.MaxValue)
        {
            roomID = 0;
        }
        this.roomID = roomID;
        this.cacheSvc = cacheSvc;
    }

    public void AddUser(ServerSession session)
    {
        //防止意外断线重连然后添加同一个人= =
        foreach (ServerSession item in clientRoom)
        {
            if (session == item)
            {
                return;
            }
        }
        clientRoom.Add(session);
        session.Room = this;
        #region 构建战场参数
        BattleProp battleProp = new BattleProp();
        battleProp.roleType = (RoleType)roleTypeIndex;
        battleProp.playerHp = PECommon.PlayerHp;
        battleProp.weapenType = WeapenType.Rifle;
        WeapenCfg weapenCfg = CfgSvc.Instance.GetWeapenCfgData((int)battleProp.weapenType);
        battleProp.weapenCfgID = weapenCfg.id;
        roleTypeIndex++;
        #endregion

        battleDataDic.Add(session, battleProp);

        if (clientRoom.Count >= PECommon.MaxPlayer)
        {
            roomState = RoomState.WaitingBattle;
        }
    }

    public bool GetRoomStateIsWaitngJoin ()
    {
        return roomState == RoomState.WaitingJoin;
    }

    public int GetRoomID()
    {
        return roomID;
    }

    public RspCreateRoom GetOwnerData()
    {
        PlayerData playerData = cacheSvc.GetPlayerDataBySession(clientRoom[0]);
        if (playerData!=null)
        {
            RspCreateRoom rspCreateRoom = new RspCreateRoom
            {
                id = roomID,
                name = playerData.name,
                victoryNum = playerData.victoryNum,
            };
            return rspCreateRoom;
        }
        return null;
    }

    public List<PlayerRoomData> GetAllRoomMemberData()
    {
        List<PlayerRoomData> allDataList = new List<PlayerRoomData>();
        foreach (ServerSession item in clientRoom)
        {
            PlayerData playerData = cacheSvc.GetPlayerDataBySession(item);
            BattleProp battleProp = null;
            battleDataDic.TryGetValue(item, out battleProp);
            bool isReady = false;
            playerStateDic.TryGetValue(playerData.name, out isReady);
            PlayerRoomData playerRoomData = new PlayerRoomData
            {
                isRoomOwner = item == GetOwnerClient(),
                name = playerData.name,
                victoryNum = playerData.victoryNum,
                weapenType = (int)battleProp.weapenType,
                isReady = isReady,
            };
            allDataList.Add(playerRoomData);
        }
        return allDataList;
    }


    public void ChangePlayerState(string name, bool ready)
    {
        playerStateDic.Remove(name);
        playerStateDic.Add(name, ready);
    }

    /// <summary>
    /// 广播消息给房间内所有的客户端，房间加入了一个玩家,除了我们剔除的那个不广播（也就是加入的那个）= =
    /// </summary>
    public void BroadcastMessage(ServerSession excludeClient,GameMsg gameMsg)
    {

        //万一房间有很多人呢？
        byte[] bytes = PENet.PETool.PackNetMsg(gameMsg);
        
        foreach (ServerSession session in clientRoom)
        {
            if (session != excludeClient)
            {
                session.SendMsg(bytes);
            }
        }
    
    }

    public ServerSession GetOwnerClient()
    {
        return clientRoom[0];
    }

    /// <summary>
    /// 房主退出时的关闭
    /// </summary>
    public void CloseRoom()
    {
        foreach (ServerSession session in clientRoom)
        {
            session.Room = null;
        }
        UISys.Instance.RemoveRoom(this);
    }

    public void RemoveUser(ServerSession session)
    {
        session.Room = null;
        clientRoom.Remove(session);
        battleDataDic.Remove(session);
        roleTypeIndex--;
        if (roomState != RoomState.Battle)
        {
            if (clientRoom.Count >= PECommon.MaxPlayer)
            {
                roomState = RoomState.WaitingBattle;
            }
            else
            {
                roomState = RoomState.WaitingJoin;
            }
        }
    }


    public BattleProp GetBatterPropBySession(ServerSession session)
    {
        BattleProp battleProp = null;
        if (battleDataDic.TryGetValue(session , out battleProp))
        {
            return battleProp;
        }
        return null;
    }

    public int GetRoomMemberCount()
    {
        return clientRoom.Count;
    }

   

    /// <summary>
    /// 只有房主退出才会关闭房间,其他人退出移出当前房间集合就行了 （非正常关闭）
    /// </summary>
    public void QuitRoom(ServerSession session)
    {
        GameMsg gameMsg = new GameMsg();
        if (session.Room.RoomState== RoomState.Battle)
        {
            if (session.Room.GetRoomMemberCount() == 2)
            {
                List<ServerSession> winSession = session.Room.GetClientRoom();
                winSession.Remove(session);
                PlayerData playerData =  cacheSvc.GetPlayerDataBySession(winSession[0]);
                gameMsg.cmd = (int)CMD.YourAreWin;
                gameMsg.yourAreWin = new YourAreWin
                {
                    victoryNum = playerData.victoryNum + 1,
                };
                winSession[0].SendMsg(gameMsg);
                CloseRoom();
            }
            else
            {
                clientRoom.Remove(session);
                gameMsg.cmd = (int)CMD.OtherQuitBattle;
                Room room = session.Room;
                gameMsg.otherQuitBattle = new OtherQuitBattle
                {
                    roleType = (int)room.GetBatterPropBySession(session).roleType,
                };
                BroadcastMessage(session, gameMsg);
            }
        }
        else
        {
            roleTypeIndex--;
            if (session.ReadyBattle)
            {
                ReadyCount -= 1;
            }
            roomState = RoomState.WaitingJoin;

            gameMsg.cmd = (int)CMD.OtherQuitRoom;
            gameMsg.otherQuitRoom = new OtherQuitRoom
            {
                name = cacheSvc.GetPlayerDataBySession(session).name,
            };
            BroadcastMessage(session, gameMsg);
        }
      
    }


    /// <summary>
    /// 开始倒计时
    /// </summary>
    public void StartTimer()
    {
        new Thread(RunTimer).Start();
    }


    private void RunTimer()
    {
        GameMsg gameMsg = new GameMsg
        {
            cmd = (int)CMD.ShowTimer
        };
        //先暂停0.5秒等加载面板
        Thread.Sleep(500);
        for (int i = 3; i > 0; i--)
        {
            int time = i;
            gameMsg.showTimer = new ShowTimer
            {
                time = time,
            };
            //给房间内所有的客户端发送计时
            BroadcastMessage(null, gameMsg);
            //停一秒
            Thread.Sleep(1000);
        }
        gameMsg.cmd = (int)CMD.StartPlay;
        BroadcastMessage(null, gameMsg);
    }
}

