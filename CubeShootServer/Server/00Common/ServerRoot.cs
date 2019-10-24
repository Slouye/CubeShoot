/****************************************************
	文件：ServerRoot.cs
	作者：Solis
	邮箱: zhaotianshinai@gmail.com
	日期：2019/07/30 16:39   	
	功能：服务器根节点
*****************************************************/

class ServerRoot
{
    private static ServerRoot instance;
    public static ServerRoot Instance
    {
        get
        {
            if (instance==null)
            {
                instance = new ServerRoot();
            }
            return instance;
        } 
    }

    public int sessionID;

    public void Init()
    {

        DBMgr.Instance.Init();

        CacheSvc.Instance.Init();
        NetSvc.Instance.Init();
        CfgSvc.Instance.Init();
        TimerSvc.Instance.Init();


        UISys.Instance.Init();
        BattleSys.Instance.Init();
    }

    public void Update()
    {
        NetSvc.Instance.Update();
        TimerSvc.Instance.Update();
    }


    /// <summary>
    /// 获得一个SessionID
    /// </summary>
    public int GetSessionId()
    {
        if (sessionID == int.MaxValue)
        {
            sessionID = 0;
        }
        return sessionID += 1;
    }

}
