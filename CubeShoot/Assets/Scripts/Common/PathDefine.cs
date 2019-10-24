/****************************************************
	文件：PathDefine.cs
	作者：Solis
    邮箱: zhaotianshinai@gmail.com
	日期：2018/12/05 3:46   	
	功能：路径常量定义
*****************************************************/

using UnityEngine;
using System.Collections;

public class PathDefine
{

    #region Configs

    public const string RDNameCfg = "ResCfgs/rdname";
    public const string WeapenCfg = "ResCfgs/weapen";
 
    #endregion

    #region Image

    public const string ImgRifle = "ResImgs/Rifle";
    public const string ImgSniper = "ResImgs/Sniper";
    public const string AimSightImg = "ResTextures/SightAim";
    public const string GunSightImg = "ResTextures/SightGun";
    public const string RifleRawTexture = "ResTextures/RifleModel";
    public const string SniperRawTexture = "ResTextures/SniperModel";

    #endregion
  

    #region Prefabs
    public const string RiflePrefab = "ResGuns/Rifle";
    public const string SniperPrefab = "ResGuns/Sniper";

    public const string RoomItemPrefab = "ResUI/RoomItem";

    public const string HPContentPath = "ResUI/HPBar";

    public const string BulletPrefab = "ResGuns/RifleBullet";

    public const string LaserBullet = "ResGuns/LaserBullet";

    public const string HitEffect = "ResEffect/HitEffect";

    public const string RifleFireEffect = "ResEffect/RifleFireEffect";
    public const string RifleShellEffect = "ResEffect/RifleShellEffect";

    public const string RoomPlayerItem = "ResUI/RoomPlayerItem";
    public const string PlayerPath = "ResPlayer/Player";
    #endregion

    #region Transform
    public const string RoomPlayerItemTrans = "PlayerList/ScrollRect/Gride";
    public const string gunTransName = "Arm/RightArm/GunPos";
    public const string rifleGunPointTransName = "Rifle/RifleMesh/EffectPos_A";
    public const string sniperGunPointTransName = "Sniper/SniperMesh/EffectPos_A";
    #endregion


    #region Audio
    public const string AudioPath = "ResAudio";

    #endregion
}
