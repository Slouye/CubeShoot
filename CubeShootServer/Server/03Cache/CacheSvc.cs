/****************************************************
	文件：CacheSvc.cs
	作者：Solis
	邮箱: zhaotianshinai@gmail.com
	日期：2019/08/01 1:33   	
	功能：数据缓存服务
*****************************************************/

using PENet;
using PEProtocol;
using System.Collections.Generic;

public class CacheSvc
{
    private static CacheSvc instance;
    public static CacheSvc Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new CacheSvc();
            }
            return instance;
        }
    }

    private Dictionary<string, ServerSession> onlinePlayerDic = new Dictionary<string, ServerSession>();
    private Dictionary<ServerSession, PlayerData> onlineCacheDataDic = new Dictionary<ServerSession, PlayerData>();

    private DBMgr dbMgr;

    public void Init()
    {
       
        dbMgr = DBMgr.Instance;
        PETool.LogMsg("DBMgr Init Down");
    }


    /// <summary>
    /// 该玩家是否在线
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool isOnline(string name)
    {
        return onlinePlayerDic.ContainsKey(name);
    }

    /// <summary>
    /// 账号密码是否正常，正确返回一个PlayerData,错误返回空，不存在创建默认账号、
    /// 不存在，创建默认的账号密码 （第三方登录）
    /// </summary>
    /// <returns></returns>
    public PlayerData GetPlayerData(string acct, string pass)
    {
        //TODO
        return dbMgr.QueryPlayerData(acct, pass);
    }

    /// <summary>
    /// 根据建立的socket连接返回玩家数据
    /// </summary>
    /// <param name="session"></param>
    /// <returns></returns>
    public PlayerData GetPlayerDataBySession(ServerSession session)
    {
        if (onlineCacheDataDic.TryGetValue(session,out PlayerData playerData))
        {
            return playerData;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 缓存已经上线的玩家资料
    /// </summary>
    public void CacheAccountData(string acct, ServerSession session, PlayerData playerData) 
    {
        onlinePlayerDic.Add(acct, session);
        onlineCacheDataDic.Add(session, playerData);
    }

    /// <summary>
    /// 获得所有在线玩家的Session
    /// </summary>
    public List<ServerSession> GetAllOnlineSession()
    {
        List<ServerSession> list = new List<ServerSession>();
        foreach (var item in onlinePlayerDic)
        {
            list.Add(item.Value);
        }
        return list;
    }

    /// <summary>
    /// 获得所有的在线玩家的Session和数据。
    /// </summary>
    public Dictionary<ServerSession, PlayerData> GetOnlineCacheDataDic()
    {
        return onlineCacheDataDic;
    }

    /// <summary>
    /// 查询名字是否存在
    /// </summary>
    public bool IsNameExist(string name)
    {
        return dbMgr.QueryNameRepeat(name);

    }


    /// <summary>
    /// 根据玩家ID更新数据库的数据
    /// </summary>
    /// <param name="playerID"></param>
    /// <param name="playerData"></param>
    /// <returns></returns>
    public bool UpdatePlayerData( int playerID,PlayerData playerData)
    {
        return dbMgr.UpdatePlayerData(playerID, playerData);
    }


    /// <summary>
    /// 清除一个在线玩家的数据(通过玩家的账号和Session)
    /// </summary> 
    public void ClearOnlinePlayerDate(string acct, ServerSession session)
    {
        onlinePlayerDic.Remove(acct);
        bool succ = onlineCacheDataDic.Remove(session);
    }

    /// <summary>
    /// 清除一个在线玩家的数据(通过Session)
    /// </summary> 
    public void ClearOnlinePlayerDate(ServerSession session)
    {
        //遍历所有在线的玩家
        foreach (var item in onlinePlayerDic)
        {
            if (item.Value == session)
            {
                onlinePlayerDic.Remove(item.Key);
                break;
            }
        }
        bool succ = onlineCacheDataDic.Remove(session);
    }



}

