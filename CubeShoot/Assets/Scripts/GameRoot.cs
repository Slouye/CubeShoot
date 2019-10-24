/****************************************************
    文件：GameRoot.cs
	作者：Solis
    邮箱: zhaotianshinai@gmail.com
    日期：2019/7/29 14:2:6
	功能：游戏启动入口
*****************************************************/

using PEProtocol;
using System.Collections.Generic;
using UnityEngine;

//模式：观察者模式  此类是被观察者(主播)    粉丝关注的是主播 粉丝是观察者
//public delegate void AutoAimDel(bool isAutoAim);

public class GameRoot : MonoBehaviour
{

    public static GameRoot Instance = null;

    public LoadingPanel LoadingPanel;
    public DynamicPanel DynamicPanel;
    public SetPanel SetPanel;

    public bool isAutoAim = true;
    public float bgAudioVoleum = 1f;
    public float uiAudioVoleum = 1f;
    public int sensitivity = 50;
    public int frameRate = 0;  // 0是 60帧 1 是90帧 2 是120帧


    public RectTransform canvasTrans;

    //玩家数据
    public PlayerData PlayerData { get; private set; }
    public void SetPlayerData(PlayerData playerData)
    {
        PlayerData = playerData;
    }

    private void Start()
    {
        Instance = this;
        DontDestroyOnLoad(this);
        PECommon.Log("Game Start...");

        ClearUIRoot();
        DynamicPanel.gameObject.SetActive(true);
        Init();
    }


    public void ClearUIRoot()
    {
        Transform canvas = transform.Find("Canvas");
        for (int i = 0; i < canvas.childCount; i++)
        {
            canvas.GetChild(i).gameObject.SetActive(false);
        }
    }

    private void Init()
    {
        //服务模块初始化
        NetSvc netSvc = GetComponent<NetSvc>();
        netSvc.InitSvc();
        ResSvc res = GetComponent<ResSvc>();
        res.InitSvc();
        AudioSvc audio = GetComponent<AudioSvc>();
        audio.InitSvc();
        TimerSvc timerSvc = GetComponent<TimerSvc>();
        timerSvc.InitSvc();


        //业务系统初始化
        UISys uiSys = GetComponent<UISys>();
        uiSys.InitSys();
        BattleSys battleSys = GetComponent<BattleSys>();
        battleSys.InitSys();

        //dynamicWnd.SetWndState();
        //进入登录场景并加载相应UI
        uiSys.EnterLogin();

    }

    /// <summary>
    /// 添加一条UI提示
    /// </summary>
    public static void AddTips(string tips)
    {
        Instance.DynamicPanel.AddTips(tips);
    }


    /// <summary>
    /// 重新设置玩家名字
    /// </summary>
    public void SetPlayerName(string name)
    {
        PlayerData.name = name;
    }

    /// <summary>
    /// 重新设置玩家的胜场
    /// </summary>
    public void SetPlayerVictoryNum(int num)
    {
        PlayerData.victoryNum = num;
    }


    public void SetPlayerRoleType(int roleType)
    {
        PlayerData.roleType = roleType;
    }
    public void SetPlayerHP(int hp)
    {
        PlayerData.hp = hp;
    }

    public void OpenSetPanel()
    {
        SetPanel = (SetPanel)UIManager.Instance.PushPanel(UIPanelType.Set);
    }

    public void LoadPlayerConfig()
    {
        //自动瞄准
        if (PlayerPrefs.HasKey("AutoAim"))
        {
            int temp = PlayerPrefs.GetInt("AutoAim");
            if (temp == 1)
            {
                isAutoAim = true;
            }
            else
            {
                isAutoAim = false;
            }
        }
        else
        {
            isAutoAim = true;
        }
     
        //背景音乐
        if (PlayerPrefs.HasKey("BGAudio"))
        {
            bgAudioVoleum = PlayerPrefs.GetFloat("BGAudio");
        }
        else
        {
            bgAudioVoleum = 1;
        }
        //UI点击
        if (PlayerPrefs.HasKey("UIAudio"))
        {
            uiAudioVoleum = PlayerPrefs.GetFloat("UIAudio");
        }
        else
        {
            uiAudioVoleum = 1;
        }
        //灵敏度
        if (PlayerPrefs.HasKey("Sensitivity"))
        {
            sensitivity = PlayerPrefs.GetInt("Sensitivity");
        }
        else
        {
            sensitivity = 50;
        }
        if (PlayerPrefs.HasKey("FrameRate"))
        {
            frameRate = PlayerPrefs.GetInt("FrameRate");
        }
        else
        {
            frameRate = 60;
        }
        PECommon.Log("自动瞄准为：" + isAutoAim);
        PECommon.Log("背景音乐为：" + bgAudioVoleum);
        PECommon.Log("UI音乐为：" + uiAudioVoleum);
        PECommon.Log("灵敏度：" + sensitivity);
        PECommon.Log("帧率为：" + frameRate);
    }

}