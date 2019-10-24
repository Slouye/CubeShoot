
using PENet;

public class BaseCfg
{
    public int id;
}

public class WeapenCfg : BaseCfg
{
    public string weapenType;
    public int weapenDamage;
    public int weapenShootRate;
    public int weapenFrontBullet;
    public int weapenBackBullet;
    public int weapenReload;
    public int weapenRocoil;
    public int weapenRocoilTime;
    public string BulletPrefab;
    public string FireEffect;
    public string ShellEffect;

}

public enum WeapenType
{
    Rifle,
    Sniper,
}

public enum RoleType
{
    Red,
    Blue,     //Y轴+180
    Yellow,   //Y轴+180
    Green,    //Y轴+180
    Purple,
    Black,
    Orange,
    White,
    None
}

public enum CameraState
{
    Ahead,
    Back,
    None
}

public enum RoomState
{
    WaitingJoin,
    WaitingBattle,
    Battle,
    End
}

public enum SocketLogType
{
    Log =0,
    Warnning = 1,
    Error = 2,
    Info = 3
}




public class PECommon
{

    public const int PlayerHp = 100;   //玩家初始HP

    public const int MaxPlayer = 8;//最大玩家数

    public const string weapenCfgPath = @"F:\project\Unity2017.3.1\GraduateDesgin-master\CubeShoot\Assets\Resources\ResCfgs\weapen.xml";

    /// <summary>
    /// 输出用...
    /// </summary>
    public static void Log(string msg = "", SocketLogType logType = SocketLogType.Log)
    {
        LogLevel logLevel = (LogLevel)logType;
        PETool.LogMsg(msg, logLevel);
    }

}

