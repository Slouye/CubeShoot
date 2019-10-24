/****************************************************
	文件：BattleSys.cs
	作者：Solis
	邮箱: zhaotianshinai@gmail.com
	日期：2019/08/17 22:57   	
	功能：战斗业务系统
*****************************************************/

using PEProtocol;
using System.Collections.Generic;

public class BattleSys
{

    private static BattleSys instance;
    public static BattleSys Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new BattleSys();
            }
            return instance;
        }
    }

    private CacheSvc cacheSvc;
    private CfgSvc cfgSvc;

    public void Init()
    {
        cacheSvc = CacheSvc.Instance;
        cfgSvc = CfgSvc.Instance;
    }


    public void ReqBattle(PackMsg pack)
    {
        ReqBattle reqBattle = pack.msg.reqBattle;
        PlayerData playerData = cacheSvc.GetPlayerDataBySession(pack.session);
        GameMsg gameMsg = new GameMsg
        {
            cmd = (int)CMD.RspBattle,
        };
      
        pack.session.SendMsg(gameMsg);
    }

    public void ReqBattleEnd(PackMsg pack)
    {
        ReqBattleEnd reqBattleEnd = pack.msg.reqBattleEnd;
        Room room = pack.session.Room;
        if (room!=null)
        {
            //房主挂了还不能关房间，
            if (room.GetRoomMemberCount()==1)
            {
                //移除房间
                room.CloseRoom();
            }
            else
            {
                room.RemoveUser(pack.session);
                GameMsg msg = new GameMsg
                {
                    cmd = (int)CMD.OtherQuitBattle,
                    otherQuitBattle = new OtherQuitBattle
                    {
                        roleType = reqBattleEnd.roleType,
                    }

                };
                room.BroadcastMessage(pack.session, msg);
            }
        }
    

        GameMsg gameMsg = new GameMsg
        {
            cmd = (int)CMD.RspBattleEnd,
        };
        pack.session.SendMsg(gameMsg);
    }

   

    public void ReqSyncMove(PackMsg pack)
    {
        ReqSyncMove reqSyncPos = pack.msg.reqSyncMove;
        Room room = pack.session.Room;
        if (room!=null)
        {
            GameMsg gameMsg = new GameMsg
            {
                cmd = (int)CMD.OtherSyncMove,
                otherSyncMove = new OtherSyncMove
                {
                    roleType = room.GetPropRoleType(pack.session),
                    pos = reqSyncPos.pos,
                    rot = reqSyncPos.rot,
                },
            };
            room.BroadcastMessage(pack.session, gameMsg);
        }
      
    }

    public void ReqSyncRot(PackMsg pack)
    {
        ReqSyncRot reqSyncRot = pack.msg.reqSyncRot;
        Room room = pack.session.Room;
        if (room==null)
        {
            return;
        }
        GameMsg gameMsg = new GameMsg
        {
            cmd = (int)CMD.OtherSyncRot,
            otherSyncRot = new OtherSyncRot
            {
                roleType = room.GetPropRoleType(pack.session),
                rot = reqSyncRot.rot,
            },
        };
        room.BroadcastMessage(pack.session, gameMsg);
    }


    public void ReqTakeDamage(PackMsg pack)
    {

        ReqTakeDamage reqTakeDamage = pack.msg.reqTakeDamage;
        if (reqTakeDamage.hurt == 0)
        {
            return;
        }
        Room room = pack.session.Room;
        if (reqTakeDamage.damageID <= room.takeDamageID)
        {
            return;
        }
        int currentHP = room.SetSomeOneHurt(reqTakeDamage.casterRoleType, reqTakeDamage.targetRoleType, reqTakeDamage.hurt);
        GameMsg gameMsg = new GameMsg
        {
           cmd = (int)CMD.RspTakeDamage,
           rspTakeDamage = new RspTakeDamage
           {
               roleType = reqTakeDamage.targetRoleType,
               hp = currentHP,
               hitPos = reqTakeDamage.hitPos,
           }
        };
        room.BroadcastMessage(null, gameMsg);
        room.takeDamageID = reqTakeDamage.damageID;
        //room.casterRoleType = reqTakeDamage.casterRoleType;
    }

    public void ReqSyncBullet(PackMsg pack)
    {
        ReqSyncBullet reqSyncBullet = pack.msg.reqSyncBullet;
        Room room = pack.session.Room;
        GameMsg gameMsg = new GameMsg
        {
            cmd = (int)CMD.AllSyncBullet,
            allSyncBullet = new AllSyncBullet
            {
                roleType = room.GetPropRoleType(pack.session),
                prefabName = reqSyncBullet.prefabName,
                shellEffect = reqSyncBullet.shellEffect,
                fireEffect = reqSyncBullet.fireEffect,
                originPos = reqSyncBullet.originPos,
                originRot = reqSyncBullet.originRot,
                dir = reqSyncBullet.dir,
                force = reqSyncBullet.force,
                damage = cfgSvc.GetWeapenCfgData(room.GetBatterPropBySession(pack.session).weapenCfgID).weapenDamage,
                shootSky = reqSyncBullet.shootSky,
            },
        };
        //PECommon.Log(gameMsg.allSyncBullet.originPos.x.ToString());
        //PECommon.Log(gameMsg.allSyncBullet.originPos.y.ToString());
        //PECommon.Log(gameMsg.allSyncBullet.originPos.z.ToString());
        //PECommon.Log(gameMsg.allSyncBullet.originRot.x.ToString());
        //PECommon.Log(gameMsg.allSyncBullet.originRot.y.ToString());
        //PECommon.Log(gameMsg.allSyncBullet.originRot.z.ToString());
        //先转化为字节码，万一房间里有很多人呢。
        room.BroadcastMessage(null, gameMsg);
    }


    public void ReqSyncLaser(PackMsg pack)
    {
        ReqSyncLaser reqSyncLaser = pack.msg.reqSyncLaser;
        Room room = pack.session.Room;
        GameMsg gameMsg = new GameMsg
        {
            cmd = (int)CMD.AllSyncLaser,
            allSyncLaser = new AllSyncLaser
            {
                roleType = room.GetPropRoleType(pack.session),
                originPos = reqSyncLaser.originPos,
                endPos = reqSyncLaser.endPos,
                damage = cfgSvc.GetWeapenCfgData(room.GetBatterPropBySession(pack.session).weapenCfgID).weapenDamage,
                shootSky = reqSyncLaser.shootSky,
                fireEffect = reqSyncLaser.fireEffect,
            },
        };
        //先转化为字节码，万一房间里有很多人呢。
        room.BroadcastMessage(null, gameMsg);
    }


    public void ReqQuitGame(PackMsg pack)
    {
        if (pack.session.Room == null)
        {
            return;
        }
        pack.session.Room.QuitRoom(pack.session);
        GameMsg gameMsg = new GameMsg
        {
            cmd = (int)CMD.RspQuitGame
        };
        pack.session.SendMsg(gameMsg);
    }
    


}

