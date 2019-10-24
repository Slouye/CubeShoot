/****************************************************
	文件：GameMsg.cs
	作者：Solis
	邮箱: zhaotianshinai@gmail.com
	日期：2019/07/30 16:38   	
	功能：协议类库
*****************************************************/
using System;
using System.Collections.Generic;
using PENet;

namespace PEProtocol
{
    [Serializable]
    public class GameMsg : PEMsg
    {
        public ReqLogin reqLogin;
        public RspLogin rspLogin;

        public ReqReName reqReName;
        public RspReName rspReName;

        public ReqBattle reqBattle;
        public RspBattle rspBattle;

        public ReqBattleEnd reqBattleEnd;
        public RspBattleEnd rspBattleEnd;

        public ReqCreateRoom reqCreateRoom;
        public RspCreateRoom rspCreateRoom;

        public ReqRoomList reqRoomList;
        public RspRoomList rspRoomList;

        public ReqJoinRoom reqJoinRoom;
        public RspJoinRoom rspJoinRoom;
        public OtherJoinRoom otherJoinRoom;

        public ReqQuitRoom reqQuitRoom;
        public OwnerQuitRoom  ownerQuitRoom;
        public OtherQuitRoom otherQuitRoom;

        public ReqReadyBattle reqReadyBattle;
        public RspReadyBattle rspReadyBattle;
        public StartBattle startBattle;
        public NoFullReady noFullReady;
        public OtherChangeReadyState otherChangeReadyState;

        public ShowTimer showTimer;
        public StartPlay startPlay;

        public ReqSyncMove reqSyncMove;
        public OtherSyncMove otherSyncMove;

        public ReqTakeDamage reqTakeDamage;
        public RspTakeDamage rspTakeDamage;

        public ReqChangeWeapen reqChangeWeapen;
        public OtherChangeWeapen otherChangeWeapen;

        public OtherQuitBattle otherQuitBattle;

        public ReqSyncRot reqSyncRot;
        public OtherSyncRot otherSyncRot;

        public YourAreDie  yourAreDie;
        public YourAreWin yourAreWin;

        public ReqSyncBullet reqSyncBullet;
        public AllSyncBullet allSyncBullet;

        public ReqSyncLaser reqSyncLaser;
        public AllSyncLaser allSyncLaser;
        public SomeOneDie someOneDie;

        public ReqQuitGame reqQuitGame;
        public RspQuitGame rspQuitGame;

    }

    #region 登录相关
    [Serializable]
    public class ReqLogin
    {
        public string Acct;
        public string Pass;
    }

    [Serializable]
    public class RspLogin
    {
        public PlayerData playerData;
    }

    [Serializable]
    public class PlayerData
    {
        public int id;
        public string name;
        public int victoryNum;
        public int hp;
        public int roleType;
    }

    [Serializable]
    public class ReqReName
    {
        public string name;
    }

    [Serializable]
    public class RspReName
    {
        public string name;
    }
    #endregion

  

    #region 战斗相关
    [Serializable]
    public class ReqBattle
    {
        public int id;
    }
    [Serializable]
    public class RspBattle
    {
        public int id;
    }

    [Serializable]
    public class ReqBattleEnd
    {
        public int roleType;
    }
    [Serializable]
    public class RspBattleEnd
    {
        
    }

    [Serializable]
    public class ReqCreateRoom
    {
        public int id;   
    }
    [Serializable]
    public class RspCreateRoom
    {
        public RoleType roleType;
        public int id;
        public string name;
        public int victoryNum;
    }

    [Serializable]
    public class ReqRoomList
    {
        public int id;
    }
    [Serializable]
    public class RspRoomList
    {
        public List<RspCreateRoom> rspCreateRoomList;
    }

    [Serializable]
    public class ReqJoinRoom
    {
        public int id;
    }
    [Serializable]
    public class RspJoinRoom
    {
        public List<PlayerRoomData> memberDataList;
        public RoleType roleType;
    }
    [Serializable]
    public class PlayerRoomData
    {
        public bool isRoomOwner;
        public string name;
        public int victoryNum;
        public int weapenType;
        public bool isReady;
    }

    [Serializable]
    public class OtherJoinRoom
    {
        public PlayerRoomData playerRoomData;
    }

    [Serializable]
    public class ReqQuitRoom
    {
        public int id;
    }
    [Serializable]
    public class OwnerQuitRoom
    {
        public int id;
    }
    [Serializable]
    public class OtherQuitRoom
    {
        public string name;
    }
    [Serializable]
    public class ReqReadyBattle
    {
        public bool isReady;
    }
    [Serializable]
    public class RspReadyBattle
    {
        public bool isReady;
    }

    [Serializable]
    public class StartBattle
    {
        public Dictionary<int, BattleProp> battlePropDic;
    }
    [Serializable]
    public class BattleProp
    {
        public RoleType roleType;
        public WeapenType weapenType;
        public int weapenCfgID;
        public int playerHp;
    }

    [Serializable]
    public class NoFullReady
    {

    }
    [Serializable]
    public class OtherChangeReadyState
    {
        public string name;
        public bool isReady;
    }
    [Serializable]
    public class ShowTimer
    {
        public float time;
    }
    [Serializable]
    public class StartPlay
    {
        
    }
    [Serializable]
    public class ReqSyncMove
    {
        public MyVector3 pos;
        public MyVector3 rot;
        public float rotBodyY;
    }
    [Serializable]
    public class MyVector3
    {
        public float x;
        public float y;
        public float z;
    }
    [Serializable]
    public class OtherSyncMove
    {
        public RoleType roleType;
        public MyVector3 pos;
        public MyVector3 rot;
        public float rotBodyY;
    }


    [Serializable]
    public class ReqTakeDamage
    {
        public int damageID;
        public RoleType casterRoleType;
        public RoleType targetRoleType; 
        public int hurt;
        public MyVector3 hitPos;
    }

    [Serializable]
    public class RspTakeDamage
    {
        public RoleType roleType;
        public int hp;
        public MyVector3 hitPos;
    }
    [Serializable]
    public class ReqChangeWeapen
    {
        public int weapenType;
    }
    [Serializable]
    public class OtherChangeWeapen
    {
        public string name;
        public int weapenType;
    }

    [Serializable]
    public class OtherQuitBattle
    {
        public int roleType;
    }

    [Serializable]
    public class ReqSyncRot
    {
        public MyVector3 rot;
    }
    [Serializable]
    public class OtherSyncRot
    {
        public RoleType roleType;
        public MyVector3 rot;
    }
    [Serializable]
    public class YourAreDie
    {
        public RoleType casterRoleType;
    }
    [Serializable]
    public class YourAreWin
    {
        public int victoryNum;
    }
    [Serializable]
    public class SomeOneDie
    {
        public int roleType;
    }
    [Serializable]
    public class ReqSyncBullet
    {
        public string prefabName;
        public string fireEffect;
        public string shellEffect;
        public MyVector3 originPos;
        public MyVector3 originRot;
        public MyVector3 dir;
        public int force;
        public int shootSky;
    }
    [Serializable]
    public class AllSyncBullet
    {
        public RoleType roleType;
        public string prefabName;
        public string fireEffect;
        public string shellEffect;
        public MyVector3 originPos;
        public MyVector3 originRot;
        public MyVector3 dir;
        public int force;
        public int damage;
        public int shootSky;
    }
    [Serializable]
    public class ReqSyncLaser
    {
        public MyVector3 originPos;
        public MyVector3 endPos;
        public int shootSky;
        public string fireEffect;
    }
    [Serializable]
    public class AllSyncLaser
    {
        public RoleType roleType;
        public MyVector3 originPos;
        public MyVector3 endPos;
        public int damage;
        public int shootSky;
        public string fireEffect;
    }

    [Serializable]
    public class ReqQuitGame
    {

    }
    [Serializable]
    public class RspQuitGame
    {

    }

    #endregion


    public enum ErrorCode
    {
        None =0,
        //用户已经在线
        AcctIsOnline,
        WrongPassword,
        NameIsExist,
        UpdateDBError,
        ClientDataError,
        GetBattleMapFail,
        RoomNull,
        RoomFull,
        RoomError,
        LackRoomMember,
    }

    public enum CMD
    {
        None = 0,
        //登录 100
        ReqLogin = 101,
        RspLogin = 102,

        ReqReName = 103,
        RspReName = 104,

        //其他 200


        //战斗相关
        ReqBattle = 301,
        RspBattle = 302,

        ReqBattleEnd = 303,
        RspBattleEnd = 304,

        ReqCreateRoom = 305,
        RspCreateRoom = 306,

        ReqRoomList = 307,
        RspRoomList = 308,

        ReqJoinRoom = 309,
        RspJoinRoom = 310,
        //别人进入了房间
        OtherJoinRoom = 311,

        ReqQuitRoom = 312,
        OwnerQuitRoom = 313,
        OtherQuitRoom = 314,

        ReqReadyBattle = 315,
        RspReadyBattle = 316,

        StartBattle = 317,
        NoFullReady = 318,
        OtherChangeReadyState = 319,
        ShowTimer = 320,
        StartPlay = 321,

        ReqSyncMove = 322,
        OtherSyncMove = 323,
        ReqTakeDamage = 324,
        RspTakeDamage = 325,

        ReqChangeWeapen = 326,

        OtherQuitBattle = 327,
        ReqSyncRot = 328,
        OtherSyncRot = 329,

        YourAreDie = 330,
        YourAreWin = 331,

        ReqSyncBullet = 332,
        AllSyncBullet = 333,

        ReqSyncLaser = 334,
        AllSyncLaser = 335,

        OtherChangeWeapen = 336,
        SomeOneDie = 337,
        ReqQuitGame = 338,
        RspQuitGame = 339,
    }


    public class SvcCfg
    {
        //public const string ServerIP = "192.168.31.239";
        //public const string ServerIP = "192.168.31.248";
        //public const string ServerIP = "192.168.31.78";
        public const string ServerIP = "172.20.10.5";
        //public const string ServerIP = "172.20.10.5";
        public const int ServerPort = 25252;

    }
}
