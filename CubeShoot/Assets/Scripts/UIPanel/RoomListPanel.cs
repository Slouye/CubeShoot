/****************************************************
    文件：RoomListPanel.cs
	作者：Solis
    邮箱: zhaotianshinai@gmail.com
    日期：2019/9/7 13:9:21
	功能：Nothing
*****************************************************/

using PEProtocol;
using UnityEngine;
using UnityEngine.UI;

public class RoomListPanel : BasePanel 
{
    public Text txtName;
    public Text txtVicotryNum;
    public RectTransform Gride;

    protected override void InitPanel()
    {
        base.InitPanel();
        RefreshSelfUI();
        ReqRoomList();
        
    }



    public override void OnResume()
    {
        base.OnResume();
        RefreshSelfUI();
        ReqRoomList();
    }

    private void RefreshSelfUI()
    {
        PlayerData playerData = GameRoot.Instance.PlayerData;
        txtName.text = playerData.name;
        txtVicotryNum.text = "胜场：" + playerData.victoryNum;
    }

    public void ReqRoomList()
    {
        GameMsg gameMsg = new GameMsg
        {
            cmd = (int)CMD.ReqRoomList
        };
        netSvc.SendMsg(gameMsg);
    }

    /// <summary>
    /// 销毁所有的房间
    /// </summary>
    public void DestoryAllRoomItem()
    {
        RoomItem[] roomItem = Gride.GetComponentsInChildren<RoomItem>();
        foreach (RoomItem item in roomItem)
        {
            item.DestorySelf();
        }
    }


    public void CreateRoomItem(RspCreateRoom rspCreateRoom)
    {
        if (rspCreateRoom==null)
        {
            return;
        }
        GameObject go = resSvc.LoadPrefab(PathDefine.RoomItemPrefab);
        go.transform.SetParent(Gride);
        go.transform.localScale = Vector3.one;
        RoomItem roomItem = go.GetComponent<RoomItem>();
        roomItem.SetRoomData(rspCreateRoom.name, rspCreateRoom.victoryNum);
        roomItem.btnJoin.onClick.AddListener(() =>
        {
            int index = rspCreateRoom.id;
            ReqJoinRoom(index);
            PECommon.Log("要加入的房间的id：" + index);
        });

    }




    public void ReqJoinRoom(int roomID)
    {
        GameMsg gameMsg = new GameMsg
        {
            cmd = (int)CMD.ReqJoinRoom,
            reqJoinRoom = new ReqJoinRoom
            {
                id = roomID,
            }
        };
        netSvc.SendMsg(gameMsg);
    }


    public void ReqCreateRoom()
    {
        GameMsg gameMsg = new GameMsg
        {
            cmd = (int)CMD.ReqCreateRoom,
        };
        netSvc.SendMsg(gameMsg);
    }


        /// <summary>
    /// 关闭按钮点击
    /// </summary>
    public void BtnCloseClick()
    {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        UIManager.Instance.PopPanel();
    }

    /// <summary>
    /// 关闭按钮点击
    /// </summary>
    public void OpenSetingPanel()
    {
        UIManager.Instance.PushPanel(UIPanelType.Set);
    }

    public void CloseGame()
    {
        Application.Quit();
    }
}