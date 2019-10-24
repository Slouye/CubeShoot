/****************************************************
    文件：RoomWnd.cs
	作者：Solis
    邮箱: zhaotianshinai@gmail.com
    日期：2019/8/28 18:21:4
	功能：房间窗口
*****************************************************/

using PEProtocol;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomPanel : BasePanel 
{


    public Text txtStart;
    public Button btnExitRoom;
    public Transform GuideTrans;

    //是否准备
    private bool isReady;


    private Dictionary<string, RoomPlayerItem> roomPlayerItemDic = new Dictionary<string, RoomPlayerItem>();

    public override void OnEnter()
    {
        base.OnEnter();
        isReady = false;
        SetActive(btnExitRoom);
    }
    public override void OnExit()
    {
        base.OnExit();
        isReady = false;

    }
    public override void OnPause()
    {
        base.OnPause();
        isReady = false;
    }
    public override void OnResume()
    {
        base.OnResume();
        isReady = false;
        SetActive(btnExitRoom);
    }

    protected override void InitPanel()
    {
        base.InitPanel();
        DestoryAllChild();
    }


    public void ReqQuitRoom()
    {
        GameMsg gameMsg = new GameMsg
        {
            cmd = (int)CMD.ReqQuitRoom,
        };
        netSvc.SendMsg(gameMsg);
    }

    public void DestoryAllChild()
    {
        //删除所有子物体
        for (int i = 0; i < GuideTrans.childCount; i++)
        {
            Destroy(GuideTrans.GetChild(i).gameObject);
        }
        roomPlayerItemDic.Clear();
    }

    public void SetExitRoom(string name)
    {
       RoomPlayerItem roomPlayerItem = roomPlayerItemDic.TryGet(name);
       if (roomPlayerItem!=null)
       {
            Destroy(roomPlayerItem.gameObject);
       }
        roomPlayerItemDic.Remove(name);
    }


    public void ReqReadyBattle()
    {
        isReady = !isReady;
        GameMsg gameMsg = new GameMsg
        {
            cmd = (int)CMD.ReqReadyBattle,
            reqReadyBattle = new ReqReadyBattle
            {
                isReady = isReady,
            }
        };
        PECommon.Log("发送准备请求：" + isReady);
        netSvc.SendMsg(gameMsg);
    }

    public void ChangeReadyStateByName(string name,bool isReady)
    {
        RoomPlayerItem roomPlayerItem = roomPlayerItemDic.TryGet(name);
        if (roomPlayerItem != null)
        {
            roomPlayerItem.SetState(isReady);
        }
    }

    public void ChangeWeapenTypeByName(string name, int weapenType)
    {
        RoomPlayerItem roomPlayerItem = roomPlayerItemDic.TryGet(name);
        if (roomPlayerItem!=null)
        {
            roomPlayerItem.SetHighImg((WeapenType)weapenType);
        }
    }


    public void CreatePlayerInfoItem(string name, int victoryNum, bool isJoin =false,bool isHouse = false, bool isSelf = false, int weapenType = 0,bool isReady = false)
    {
        GameObject go = resSvc.LoadPrefab(PathDefine.RoomPlayerItem);
        go.transform.SetParent(GuideTrans);
        go.transform.localScale = Vector3.one;
        RoomPlayerItem roomPlayerItem = go.GetComponent<RoomPlayerItem>();
        roomPlayerItem.InitItem(name, victoryNum, isHouse, isSelf, weapenType, isReady);
        roomPlayerItemDic.Add(name, roomPlayerItem);
        if (!isJoin)
        {
            if (isHouse)
            {
                SetText(txtStart, "开始游戏");
            }
            else
            {
                SetText(txtStart, "准备战斗");
            }
        }
 
    }


    public void SetReadyState(bool isReady)
    {
        if (isReady)
        {
            SetText(txtStart, "取消准备");
        }
        else
        {
            SetText(txtStart, "准备战斗");
        }
        ChangeReadyStateByName(GameRoot.Instance.PlayerData.name, isReady);
    }

}