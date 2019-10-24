using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomPlayerItem : WindowRoot {
    public Text txtName;
    public Text txtVictoryNum;
    public RawImage imgModel;
    public Text txtState;
    public Image[] tagImg;

    public void InitItem(string name, int victoryNum, bool isHouse, bool isShowWeapon, int weapenType,bool isReady)
    {
        InitPanel();
        txtName.text = name;
        txtVictoryNum.text = "胜场：" + victoryNum;
        if (!isShowWeapon)
        {
            for (int i = 0; i < tagImg.Length; i++)
            {
                SetActive(tagImg[i], false);
            }
        }
        switch ((WeapenType)weapenType)
        {
            case WeapenType.Rifle:
                imgModel.texture = Resources.Load<RenderTexture>(PathDefine.RifleRawTexture);
                break;
            case WeapenType.Sniper:
                imgModel.texture = Resources.Load<RenderTexture>(PathDefine.SniperRawTexture);
                break;
        }
        if (isHouse)
        {
            txtState.text = "房主";
        }
        else
        {
            if (isReady)
            {
                txtState.text = "准备中";
            }
            else
            {
                txtState.text = "未准备";
            }
           
        }
       
    }

 
    public void SetState(bool isState)
    {
        if (isState)
        {
            SetText(txtState, "准备中");
        }
        else
        {
            SetText(txtState, "未准备");
        }
    }

    public void ReqChangeWeapen(int weapenID)
    {
        WeapenType weapenType = (WeapenType)weapenID;
        SetHighImg(weapenType);
        GameMsg gameMsg = new GameMsg
        {
            cmd = (int)CMD.ReqChangeWeapen,
            reqChangeWeapen = new ReqChangeWeapen
            {
                weapenType = weapenID,
            }
        };
        netSvc.SendMsg(gameMsg);
    }

    public void SetHighImg(WeapenType weapenType)
    {
        int imgIndex = (int)weapenType;
        for (int i = 0; i < tagImg.Length; i++)
        {
            tagImg[i].color = Color.white;
        }
        tagImg[imgIndex].color = Color.red;
        switch (weapenType)
        {
            case WeapenType.Rifle:
                imgModel.texture = Resources.Load<RenderTexture>(PathDefine.RifleRawTexture);
                break;
            case WeapenType.Sniper:
                imgModel.texture = Resources.Load<RenderTexture>(PathDefine.SniperRawTexture);
                break;
        }

    }

}
