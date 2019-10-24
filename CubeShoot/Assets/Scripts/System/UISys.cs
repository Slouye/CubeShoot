/****************************************************
    文件：LoginSys.cs
	作者：Solis
    邮箱: zhaotianshinai@gmail.com
    日期：2019/7/30 17:20:20
	功能：登录注册业务系统
*****************************************************/

using PEProtocol;
using System.Collections.Generic;
using UnityEngine;

public class UISys : SystemRoot
{
    public static UISys Instance = null;

    private RoomListPanel roomListPanel;
    private RoomPanel roomPanel;

    public RoomListPanel RoomListPanel
    {
        get { return roomListPanel; }
        set { roomListPanel = value; }
    }

    public override void InitSys()
    {
        base.InitSys();

        Instance = this;
        PECommon.Log("Init LoginSys...");

        EnterLogin();
    }



    /// <summary>
    /// 进入登录场景
    /// </summary>
    public void EnterLogin()
    {
        audioSvc.PlayAudioInUI(audioSvc.uiAudio);
        //异步的加载登录场景
        //并显示加载的进度
        resSvc.AsyncLoadScene(Constants.SceneRoom, () => {
            //加载完成以后再打开注册登录界面
            UIManager.Instance.PushPanel(UIPanelType.Login);
            audioSvc.PlayBGMusic(Constants.BGRoom1);
        });
    }


    public void EnterCreateWnd()
    {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        UIManager.Instance.PushPanel(UIPanelType.Create);
    }


    public void EnterRoomListWnd()
    {
        audioSvc.PlayAudioInUI(audioSvc.uiAudio);
        //roomListPanel.SetWndState();
        roomListPanel = (RoomListPanel)UIManager.Instance.PushPanel(UIPanelType.RoomList);
    }


    public void EnterRoomWnd()
    {
        audioSvc.PlayAudioInUI(audioSvc.uiAudio);
        audioSvc.PlayBGMusic(Constants.BGRoom3);
        roomPanel =(RoomPanel) UIManager.Instance.PushPanel(UIPanelType.Room);
        //roomPanel.SetWndState();

    }

    public void RspLogin(GameMsg msg)
    {
        GameRoot.AddTips("登录成功");

        GameRoot.Instance.SetPlayerData(msg.rspLogin.playerData);
        if (msg.rspLogin.playerData.name == "")
        {
            //打开角色创建界面
            EnterCreateWnd();
        }
        else
        {
            //进入房间列表界面
            EnterRoomListWnd();
          
        }
        GameRoot.Instance.LoadPlayerConfig();
    }



 




    public void RspReName(GameMsg msg)
    {
        audioSvc.PlayAudioInUI(audioSvc.uiAudio);
        //修改缓存的名字
        GameRoot.Instance.SetPlayerName(msg.rspReName.name);
        //房间列表
        roomListPanel = (RoomListPanel)UIManager.Instance.PushPanel(UIPanelType.RoomList);
        //显示主城UI

    }


    public void RspRoomList(GameMsg msg)
    {
        roomListPanel.DestoryAllRoomItem();
        RspRoomList rspRoomList = msg.rspRoomList;
        List<RspCreateRoom> ownerDataList = rspRoomList.rspCreateRoomList;
        for (int i = 0; i < ownerDataList.Count; i++)
        {
            roomListPanel.CreateRoomItem(ownerDataList[i]);
        }
    }

    public void RspCreateRoom(GameMsg msg)
    {
        audioSvc.PlayAudioInUI(audioSvc.uiAudio);
        RspCreateRoom rspCreateRoom = msg.rspCreateRoom;
        BattleSys.Instance.SetCurrentRoleType(rspCreateRoom.roleType);
        GameRoot.Instance.SetPlayerRoleType((int)rspCreateRoom.roleType);
        GameRoot.AddTips(Constants.GetColorStr("创建房间成功", TextColor.Red));
        EnterRoomWnd();
        roomPanel.CreatePlayerInfoItem(rspCreateRoom.name, rspCreateRoom.victoryNum, false, true, true);
    }

    public void RspJoinRoom(GameMsg msg)
    {
        audioSvc.PlayAudioInUI(audioSvc.uiAudio);
        RspJoinRoom rspJoinRoom = msg.rspJoinRoom;
        BattleSys.Instance.SetCurrentRoleType(rspJoinRoom.roleType);
        GameRoot.Instance.SetPlayerRoleType((int)rspJoinRoom.roleType);
        EnterRoomWnd();
        List<PlayerRoomData> memberDataList = rspJoinRoom.memberDataList;

        for (int i = 0; i < memberDataList.Count; i++)
        {
            //找到自己的
            if (memberDataList[i].name == GameRoot.Instance.PlayerData.name)
            {
                roomPanel.CreatePlayerInfoItem(memberDataList[i].name, memberDataList[i].victoryNum, false, false, true);
            }
            else
            {
                roomPanel.CreatePlayerInfoItem(memberDataList[i].name, memberDataList[i].victoryNum, true, memberDataList[i].isRoomOwner, false, memberDataList[i].weapenType, memberDataList[i].isReady);
            }
        }
    }

    public void OtherJoinRoom(GameMsg msg)
    {
        audioSvc.PlayAudioInUI(audioSvc.uiAudio);
        OtherJoinRoom otherJoinRoom = msg.otherJoinRoom;
        PlayerRoomData playerRoomData = otherJoinRoom.playerRoomData;
        roomPanel.CreatePlayerInfoItem(playerRoomData.name, playerRoomData.victoryNum,true);
    }

    public void OwnerQuitRoom(GameMsg msg)
    {
        audioSvc.PlayAudioInUI(audioSvc.uiAudio);
        roomPanel.DestoryAllChild();
        UIManager.Instance.PopPanel();

    }

    public void OtherQuitRoom(GameMsg msg)
    {
        audioSvc.PlayAudioInUI(audioSvc.uiAudio);
        OtherQuitRoom otherQuitRoom = msg.otherQuitRoom;
        roomPanel.SetExitRoom(otherQuitRoom.name);
    }



    public void StartBattle(GameMsg msg)
    {
        audioSvc.PlayAudioInUI(audioSvc.uiAudio);
        StartBattle startBattle = msg.startBattle;

        //传递玩家数据
        PECommon.Log("跳转场景,进入战斗。");
        resSvc.AsyncLoadScene(Constants.SceneBattle, () =>
        {
            BattleSys.Instance.EnterBattle(startBattle.battlePropDic);
        });
        //TODO多人再做。
    }

    public void NoFullReady(GameMsg msg)
    {
        audioSvc.PlayAudioInUI(audioSvc.uiAudio);
        //传递玩家数据
        PECommon.Log("有玩家还没准备。");
        GameRoot.AddTips(Constants.GetColorStr("有玩家还没准备。", TextColor.Red));
    }

    public void RspReadyBattle(GameMsg msg)
    {
        audioSvc.PlayAudioInUI(audioSvc.uiAudio);
        RspReadyBattle rspReadyBattle = msg.rspReadyBattle;
        roomPanel.SetReadyState(rspReadyBattle.isReady);
    }

    public void OtherChangeReadyState(GameMsg msg)
    {
        audioSvc.PlayAudioInUI(audioSvc.uiAudio);
        OtherChangeReadyState readyState = msg.otherChangeReadyState;
        roomPanel.ChangeReadyStateByName(readyState.name, readyState.isReady);
    }

    public void OtherChangeWeapen(GameMsg msg)
    {
        audioSvc.PlayAudioInUI(audioSvc.uiAudio);
        OtherChangeWeapen otherChangeWeapen = msg.otherChangeWeapen;
        roomPanel.ChangeWeapenTypeByName(otherChangeWeapen.name, otherChangeWeapen.weapenType);
    }

}