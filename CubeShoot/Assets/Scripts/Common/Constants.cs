/****************************************************
    文件：Constants.cs
	作者：Solis
    邮箱: zhaotianshinai@gmail.com
    日期：2019/7/30 17:20:20
	功能：常量配置
*****************************************************/

using UnityEngine;


public enum TextColor
{
    Red,
    Blue,
    Green,
    Yellow
}


public class Constants
{

    public const int GameRate120 = 8;
    public const int GameRate90 = 11;
    public const int GameRate60 = 16;

    public const float SelfAimRange = 200.0f;
    public const float SelfAimStopRange = 30.0f;


    public const float JumpTargetBlend =2f;


    public const int BulletForce = 300;


    public const string RifleGunName = "Rifle";
    public const string SniperGunName = "Sniper";

   



#region 染色工具

    private const string RedUBB = "<color=#FF0000FF>";
    private const string BlueUBB = "<color=#5300FFFF>";
    private const string GreenUBB = "<color=#00FF02FF>";
    private const string YellowUBB = "<color=#DCFF00FF>";
    private const string WhiteUBB = "<color=#FFFFFFFF>";
    private const string EndUBB = "</color>";

    public static string GetColorStr(string str, TextColor c)
    {
        switch (c)
        {
            case TextColor.Red:
                return RedUBB + str + EndUBB;
            case TextColor.Blue:
                return BlueUBB + str + EndUBB;
            case TextColor.Green:
                return GreenUBB + str + EndUBB;
            case TextColor.Yellow:
                return YellowUBB + str + EndUBB;
            default:
                return WhiteUBB + str + EndUBB;
        }
    }
#endregion


    //登录
    public const string SceneLogin = "SceneLogin";
    //房间
    public const string SceneRoom = "SceneRoom";
    //战斗场景
    public const string SceneBattle = "SceneBattle";
    //频道场景
    public const string SceneChat = "SceneChat";


    //音效名称
    public const string BGRoom1 = "BG/BGAudio";
    public const string BGRoom2 = "BG/BGAudio3";
    public const string BGRoom3 = "BG/Dark Fantasy Studio- Take over (seamless)";
    public const string BGBattle = "BG/BGAudio2";
    public const string UIClick = "ClickAudio";


    //登录按钮音效
    public const string UILoginBtn = "uiLoginBtn";
    //菜单展开音效按钮音效
    public const string UIOpenPage = "uiOpenPage";
    //常规UI点击音效
    public const string UIClickBtn = "uiClickBtn";
    //结算音效
    public const string EndBattleItem = "fbitem";
    public const string EndBattleLoss = "fblose";
    public const string EndBattleWin = "fbwin";
    public const string EndBattleList = "uiExtenBtn";


    //角色受击音效
    public const string PlayerHitAudio = "assassin_Hit";

    //屏幕标准宽度和高度
    public const int ScreenStandardWidth = 1920;
    public const int ScreenStandardHeight = 1080;

    //在标准屏幕下操纵杆可移动的最大距离
    public const int ControllerMaxDis = 100;

    //玩家的移动速度
    public const int PlayerMoveSpeed = 7;

    //运动平滑加速度
    public const float AccelerateSpeed = 5;
    //血条的加速度
    public const float AccelerateHPSpeed = 0.15f;

}