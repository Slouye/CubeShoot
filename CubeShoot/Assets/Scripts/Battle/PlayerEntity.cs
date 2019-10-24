/****************************************************
    文件：PlayerEntity.cs
	作者：Solis
    邮箱: zhaotianshinai@gmail.com
    日期：2019/9/8 21:16:19
	功能：Nothing
*****************************************************/

using UnityEngine;

public class PlayerEntity : EntityBase 
{
    private RoleType roleType;

    private CameraState cameraState = CameraState.None;

    public RoleType GetRoleType()
    {
        return roleType;
    }
    public void SetRoleType(RoleType roleType)
    {
        this.roleType = roleType;
        cameraState = UnityTools.AheadOrBack(roleType);
    }


    private GunControllerBase gunControllerBase;


    public PlayerEntity(BattleMgr battleMgr, StateMgr stateMgr) : base(battleMgr, stateMgr)
    {

    }

    public override Vector2 GetMoveInput()
    {
        return battleMgr.GetMoveInput();
    }
    public void JumpCompelate()
    {
        Fall();
    }
    public void FallCompelate()
    {
        if (GetMoveInput() == Vector2.zero)
        {
            Idle();
        }
        else
        {
            Move();
        }
    }

    public override void SetMove(Vector2 distance)
    {
        if (canControl == false)
        {
            return;
        }
        if (currentAniState == AniState.Born || currentAniState == AniState.Die || currentAniState == AniState.Jump || currentAniState == AniState.Fall)
        {
            return;
        }
        //避免受击滑动
        if (distance == Vector2.zero)
        {
            Idle();
        }
        else
        {
            Move();
        }
        if (controller != null)
        {
            //开镜移速减半
            if (gunControllerBase.isHold)
            {
                distance *= 0.5f;
            }
            controller.Dis = distance;
        }
    }

    public override void SetDir(Vector2 direction)
    {
        if (canControl == false)
        {
            return;
        }
        if (currentAniState== AniState.Born || currentAniState == AniState.Die )
        {
            return;
        }
        //if (direction == Vector2.zero)
        //{
        //    Idle();
        //}
        //else
        //{
        //    Rot();
        //}
        if (controller != null)
        {
            if (cameraState == CameraState.Ahead)
            {
                direction.y = -direction.y;
                controller.Dir = direction;
            }
            else if (cameraState == CameraState.Back)
            {
                direction.x = -direction.x;
                controller.Dir = -direction;
            }
        }
    }


    public void SetPlayerWeapen()
    {
        WeapenCfg weapenCfg = ResSvc.Instance.GetWeapenCfgData(battleProp.weapenCfgID);
        controller.gameObject.AddComponent<ObjectPool>();
        switch (weapenCfg.id)
        {
            case (int)WeapenType.Rifle:
                RifleView rifleView = controller.gameObject.AddComponent<RifleView>();
                gunControllerBase = controller.gameObject.AddComponent<Rifle>();
                gunControllerBase.M_GunViewBase = rifleView;
                break;
            case (int)WeapenType.Sniper:
                SniperView sniperView = controller.gameObject.AddComponent<SniperView>();
                gunControllerBase = controller.gameObject.AddComponent<Sniper>();
                gunControllerBase.M_GunViewBase = sniperView;
                break;
        }
       
        gunControllerBase.Init(weapenCfg);
    }


    public override void StartFire()
    {
        if (currentAniState == AniState.Born || currentAniState == AniState.Die)
        {
            return;
        }
        //双重判断
        if (gunControllerBase.canShoot == true)
        {
            gunControllerBase.isShoot = true;
        }
    }
    public override void StopFire()
    {
        gunControllerBase.isShoot = false;
    }

    public void SwitchGlass()
    {
        if (currentAniState == AniState.Born || currentAniState == AniState.Die)
        {
            return;
        }
        gunControllerBase.SwitchGlass();
    }

    public void ReloadBullet()
    {
        if (currentAniState == AniState.Born || currentAniState == AniState.Die)
        {
            return;
        }
        gunControllerBase.ReloadBullet();
    }

    public void SetGunFaceWall(bool isFace)
    {
        gunControllerBase.SetGunFaceWall(isFace);
    }
    public void CharacterJump()
    {
        if (currentAniState == AniState.Born || currentAniState == AniState.Die)
        {
            return;
        }
        Jump(); 
    }

}