/****************************************************
    文件：BattleSys.cs
	作者：Solis
    邮箱: zhaotianshinai@gmail.com
    日期：2019/8/17 17:33:33
	功能：战斗业务系统
*****************************************************/

using PEProtocol;
using System;
using UnityEngine;
using System.Collections.Generic;


public class BattleSys : SystemRoot
{
    public static BattleSys Instance = null;

    public PlayerControlPanel playerControlPanel;
    public EndBattlePanel endBattlePanel;


    public BattleMgr battleMgr;

    private RoleType currentRoleType;

    public int selfShootNum;

    //玩家血量字典集合
    public Dictionary<RoleType, int> playerHPDic = new Dictionary<RoleType, int>();

    public override void InitSys()
    {
        base.InitSys();

        Instance = this;
        PECommon.Log("Init BattleSys...");
    }


    #region 战斗


    public void OtherSyncMove(GameMsg msg)
    {
        OtherSyncMove otherSyncMove = msg.otherSyncMove;
        MyVector3 myPos = otherSyncMove.pos;
        MyVector3 myRot = otherSyncMove.rot;

        Vector3 pos = new Vector3(myPos.x, myPos.y, myPos.z);
        Vector3 rot = new Vector3(myRot.x, myRot.y, myRot.z);

        if (battleMgr==null)
        {
            return;
        }
        RemoteController remoteController = (RemoteController)battleMgr.GetController((RoleType)otherSyncMove.roleType);
        remoteController.gameObject.transform.position = pos;
        remoteController.UpdateModelAction(rot);

        //PECommon.Log((RoleType)otherSyncMove.roleType + "移动了位置：" + pos + "方向：" + rot);
    }

    public void OtherSyncRot(GameMsg msg)
    {
        OtherSyncRot otherSyncRot = msg.otherSyncRot;
        MyVector3 myRot = otherSyncRot.rot;

        Vector3 rot = new Vector3(myRot.x, myRot.y, myRot.z);

        RemoteController remoteController = (RemoteController)battleMgr.GetController((RoleType)otherSyncRot.roleType);
        remoteController.UpdateModelAction(rot);
        PECommon.Log((RoleType)otherSyncRot.roleType + "移动了方向：" + rot);
    }

    public void RspTakeDamage(GameMsg msg)
    {
        RspTakeDamage rspTakeDamage  = msg.rspTakeDamage;
        ItemHP itemHP = playerControlPanel.playerItemHPDic.TryGet(rspTakeDamage.roleType);
        if (itemHP!=null)
        {
            itemHP.SetBloodBar(itemHP.currentHP, rspTakeDamage.hp);
        }
        if (battleMgr.playerEntity!=null)
        {
            if (battleMgr.playerEntity.GetRoleType() == rspTakeDamage.roleType)
            {
                //更新底部血条
                PECommon.Log("受到伤害。,更新底部血条");
                battleMgr.playerEntity.Hit();
                playerControlPanel.SelfItemHp.SetBloodBar(playerControlPanel.SelfItemHp.currentHP, rspTakeDamage.hp);
            }
        }
        //显示粒子特效
        Vector3 hitPos = UnityTools.GetV3Value(rspTakeDamage.hitPos);
        battleMgr.ShowHitEffect(hitPos);
    }

    public void YourAreDie(GameMsg msg)
    {
        audioSvc.PlayBGMusic(Constants.BGRoom2);
        YourAreDie yourAreDie = msg.yourAreDie;
        if (battleMgr!=null)
        {
            if (battleMgr.playerEntity!=null)
            {
                battleMgr.playerEntity.Die();
                battleMgr.playerEntity = null;
            }
        }
        
        //摄像机环绕
        endBattlePanel = (EndBattlePanel)UIManager.Instance.PushPanel(UIPanelType.EndBattle);
        endBattlePanel.Lose(yourAreDie.casterRoleType);
    }

    public void YourAreWin(GameMsg msg)
    {
        audioSvc.PlayBGMusic(Constants.BGRoom2);
        YourAreWin yourAreWin = msg.yourAreWin;
        battleMgr.playerEntity.Win();
        battleMgr.playerEntity = null;
        GameRoot.Instance.SetPlayerVictoryNum(yourAreWin.victoryNum);
        endBattlePanel = (EndBattlePanel)UIManager.Instance.PushPanel(UIPanelType.EndBattle);
        endBattlePanel.Win(yourAreWin.victoryNum);
    }

    public void RspBattleEnd(GameMsg msg)
    {
        SetSniperAim(false);
        SetImgSight(PathDefine.GunSightImg);
        playerControlPanel.BattleEnd();
        playerHPDic.Clear();
        Destroy(battleMgr.gameObject);
        resSvc.AsyncLoadScene(Constants.SceneRoom, () =>
        {
            UISys.Instance.RoomListPanel = (RoomListPanel)UIManager.Instance.PushPanel(UIPanelType.RoomList);
        });
    }
    public void AllSyncBullet(GameMsg msg)
    {
        AllSyncBullet allSyncBullet = msg.allSyncBullet;
        battleMgr.SetChatacterShootBullet(allSyncBullet);
    }

    public void AllSyncLaser(GameMsg msg)
    {
        AllSyncLaser allSyncLaser = msg.allSyncLaser;
        battleMgr.SetChatacterShootLaser(allSyncLaser);
    }

    public void StartPlay(GameMsg msg)
    {
        if (battleMgr!=null)
        {
            if (battleMgr.playerEntity!=null)
            {
                battleMgr.playerEntity.canControl = true;
            }
        }
     
    }

    public void ShowTimer(GameMsg msg)
    {
        ShowTimer showTimer = msg.showTimer;
        GameRoot.AddTips(showTimer.time.ToString());
    }

    public void OtherQuitBattle(GameMsg msg)
    {
        OtherQuitBattle otherQuitBattle = msg.otherQuitBattle;
        playerControlPanel.RemoveEnemyInDic(((RoleType)otherQuitBattle.roleType).ToString());
        SetOneQuitBattle((RoleType)otherQuitBattle.roleType);
    }
    public void SomeOneDie(GameMsg msg)
    {
        SomeOneDie someOneDie = msg.someOneDie;
        SetOneQuitBattle((RoleType)someOneDie.roleType);
    }

    public void RspQuitGame(GameMsg msg)
    {
        RspBattleEnd(msg);
    }

    public void SetOneQuitBattle(RoleType roleType)
    {

        GameRoot.AddTips(roleType + "退出了战斗");
        //销毁UI
        playerControlPanel.DestroyOneHPBar(roleType);
        //销毁一下预制体。
        if (battleMgr!=null)
        {
            battleMgr.DestroyPlayerModel(roleType);
        }
    }
    

    #endregion


    /// <summary>
    /// 进入战斗
    /// </summary>
    /// <param name="battleProp">自己的战场参数</param>
    public void EnterBattle(Dictionary<int, BattleProp> battlePropDic)
    {
        GameObject battleGO = new GameObject("Battle");
        battleGO.transform.SetParent(gameObject.transform);
        battleMgr = battleGO.AddComponent<BattleMgr>();
        battleMgr.Init(battlePropDic);

        audioSvc.PlayBGMusic(Constants.BGBattle);
        foreach (var item in battlePropDic)
        {
            //玩家血量字典集合
            playerHPDic.Add(item.Value.roleType, item.Value.playerHp);
        }
        playerControlPanel = (PlayerControlPanel)UIManager.Instance.PushPanel(UIPanelType.PlayerControl);
        //SetFireButton();
        CreateAllPlayerHPUI(playerHPDic);

        WeapenCfg weapenCfg = resSvc.GetWeapenCfgData(battlePropDic.TryGet((int)GetCurrentRoleType()).weapenCfgID);
        SetTxtBullet(weapenCfg.weapenFrontBullet, weapenCfg.weapenBackBullet);

        //小于一定范围自动瞄准。
        GameObject[] Players =  GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < Players.Length; i++)
        {
            if (Players[i].GetComponent<PlayerController>() == null)
            {
                playerControlPanel.AddEnemyToDic(Players[i].name, Players[i]);
                PECommon.Log("添加敌方角色进自动瞄准List：" + Players[i].name);
            }
        }
    }

    #region PlayerMove 玩家移动逻辑
    /// <summary>
    /// 设置玩家移动
    /// </summary>
    public void SetPlayerMove(Vector2 distance)
    {
        battleMgr.SetSelfPlayerMove(distance);
    }
    /// <summary>
    /// 设置玩家方向
    /// </summary>
    public void SetPlayerDir(Vector2 dir)
    {
        battleMgr.SetSelfPlayerRot(dir);
    }
    #endregion

    public Vector2 GetPlayerMoveInput()
    {
        return playerControlPanel.currentMove;
    }


    public void DestroyBattle()
    {
    
    }

    public void SetPlayerControlWndShow(bool isShow = true)
    {
        //playerControlPanel.SetWndState(isShow);
    }

    public void SetCurrentRoleType(RoleType roleType)
    {
        currentRoleType = roleType;
    }
    public RoleType GetCurrentRoleType()
    {
        return currentRoleType;
    }


    public void StartFire()
    {
        if (battleMgr!=null)
        {
           if (battleMgr.playerEntity!=null)
           {
                battleMgr.playerEntity.StartFire();
            }
        }
     
    }

    public void StopFire()
    {
        if (battleMgr != null)
        {
            if (battleMgr.playerEntity != null)
            {
                battleMgr.playerEntity.StopFire();
            }
        }
    }

    public void SwitchGlass()
    {
        battleMgr.playerEntity.SwitchGlass();
    }

    public void CharacterJump()
    {
        ChangeJumpBtnInteractable(false);
        battleMgr.playerEntity.CharacterJump();
    }

    public void ExitGame()
    {
        UIManager.Instance.PushPanel(UIPanelType.ExitGame);
    }


    /// <summary>
    /// 左上角创建所有玩家的UI
    /// </summary>
    public void CreateAllPlayerHPUI(Dictionary<RoleType,int> HPDic)
    {
        playerControlPanel.CreateAllPlayerHPUI(HPDic);
    }

    /// <summary>
    /// 设置前弹夹和后弹夹
    /// </summary>
    public void SetTxtBullet(int weapenFrontBullet, int weapenBackBullet)
    {
        playerControlPanel.SetTextBullet(weapenFrontBullet, weapenBackBullet);
    }

    public void ShowReloadBulletAni(float showTime)
    {
        playerControlPanel.ShowReloadBulletAni(showTime);
    }

    public void SetLoadBulletState (bool isLoadBullet)
    {
        playerControlPanel.IsLoadBullet = isLoadBullet;
    }

    public void HideReloadBulletAni()
    {
        playerControlPanel.HideReloadBulletAni();
    }


    public void ReloadBullet()
    {
        battleMgr.playerEntity.ReloadBullet();
    }

    public void SetSniperAim(bool isShow)
    {
        playerControlPanel.SetSniperAim(isShow);
    }

    public void SetImgSight(string path)
    {
        playerControlPanel.SetImgSight(path);
    }

    public void Aim(bool aim)
    {
        playerControlPanel.isOpenGlass = aim;
    }

    public void SetGunFaceWall(bool isFace)
    {
        battleMgr.SetGunFaceWall(isFace);
    }

    public void SetSignRed()
    {
        playerControlPanel.imgSight.color = Color.red;
        timerSvc.AddTimeTask((int itd) => 
        {
            playerControlPanel.imgSight.color = Color.white;
        },100);
    }

    public void JumpCompelate()
    {
        battleMgr.JumpCompelate();
    }

    public void FallCompelate()
    {
        battleMgr.FallCompelate();
    }

    public AniState GetPlayerAniState()
    {
        if (battleMgr!=null)
        {
            if (battleMgr.playerEntity!=null)
            {
                return battleMgr.playerEntity.currentAniState;
            }
            return AniState.None;
        }
        else
        {
            return AniState.None;
        }
      
    }

    public void ChangeJumpBtnInteractable(bool canJump)
    {
        if (playerControlPanel!=null)
        {
            playerControlPanel.JumpBtn.interactable = canJump;
        }
    }
}