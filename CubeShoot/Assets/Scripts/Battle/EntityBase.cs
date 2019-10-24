/****************************************************
    文件：EntityBase.cs
	作者：Solis
    邮箱: zhaotianshinai@gmail.com
    日期：2019/9/8 18:27:14
	功能：Nothing
*****************************************************/

using PEProtocol;
using UnityEngine;

public class EntityBase 
{
    public AniState currentAniState = AniState.None;

    public BattleMgr battleMgr = null;
    public StateMgr stateMgr = null;

    public ControllerBase controller = null;

    public bool canControl = true;


    public double lastVoiceTime;

    public EntityBase(BattleMgr battleMgr, StateMgr stateMgr)
    {
        this.battleMgr = battleMgr;
        this.stateMgr = stateMgr;
        
    }


    protected BattleProp battleProp;
    public void SetBattleProps(BattleProp props)
    {
        battleProp = props;
    }
    public void SetCtrl(ControllerBase ctrl)
    {
        controller = ctrl;
    }
    public ControllerBase GetController()
    {
        if (controller != null)
        {
            return controller;
        }
        return null;
    }

    public void Born()
    {
        lastVoiceTime = TimerSvc.Instance.GetNowTime();
        stateMgr.ChangeStatus(this, AniState.Born, null);
    }
    public void Move()
    {
        stateMgr.ChangeStatus(this, AniState.Move, null);
    }
    public void Rot()
    {
        stateMgr.ChangeStatus(this, AniState.Rot, null);
    }
    public void Idle()
    {
        stateMgr.ChangeStatus(this, AniState.Idle, null);
    }
    public void Attack()
    {
        stateMgr.ChangeStatus(this, AniState.Attack, null);
    }
    public void Reload ()
    {
        stateMgr.ChangeStatus(this, AniState.Reload, null);
    }
    public void Hit()
    {
        stateMgr.ChangeStatus(this, AniState.Hit, null);
    }
    public void Die()
    {
        stateMgr.ChangeStatus(this, AniState.Die, null);
    }
    public void Win()
    {
        stateMgr.ChangeStatus(this, AniState.Win, null);
    }
    public void Jump()
    {
        stateMgr.ChangeStatus(this, AniState.Jump, null);
    }
    public void Fall()
    {
        stateMgr.ChangeStatus(this, AniState.Fall, null);
    }
    public virtual void SetMove(Vector2 distance)
    {
        if (controller != null)
        {
            controller.Dis = distance;
        }
    }

    public virtual void SetDir(Vector2 direction)
    {
        if (controller != null)
        {
            controller.Dir = direction;
        }
    }


    public virtual Vector2 GetMoveInput()
    {
        return Vector2.zero;
    }


    /// <summary>
    /// 设置武器属性
    /// </summary>
    public virtual void SetWeapenInfo(WeapenType weapenType, WeapenCfg weapenCfg)
    {

    }

    public virtual void StartFire()
    {

    }
    public virtual void StopFire()
    {

    }

    public void PlayAudio()
    {
      
        if (controller!=null)
        {
            if (TimerSvc.Instance.GetNowTime() - lastVoiceTime > 5000)
            {
                controller.PlayAudio();
            }
            lastVoiceTime = TimerSvc.Instance.GetNowTime();
        }
    }

}