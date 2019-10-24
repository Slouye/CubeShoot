/****************************************************
    文件：PlayerController.cs
	作者：Solis
    邮箱: zhaotianshinai@gmail.com
    日期：2019/9/8 17:30:55
	功能：Nothing
*****************************************************/

using PEProtocol;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerController : ControllerBase 
{

    private CameraState cameraState = CameraState.None;

    public override void SetRoleType(RoleType roleType)
    {
        this.roleType = roleType;
        cameraState = UnityTools.AheadOrBack(roleType);
        for (int i =0;i<transform.childCount;i++)
        {
            modelItemList.Add(transform.GetChild(i));
        }
    }

    private double lastSendMoveTime;
    private double lastSendRotTime;

    private Vector3 camOffset;
    //当前混合树的值和目标混合树的值。
    private float targetBlend;
    private float currentBlend;

    private Vector3 lastPos;
    private Vector3 lastRot;

    private Vector2 jumpDir = Vector3.up;
    private Vector2 fullDir = Vector3.down;

    public int frameRate;



    private void Awake()
    {
        camTrans = Camera.main.transform;
    }


    protected override void Start()
    {
        base.Start();
        camOffset = transform.position - camTrans.position;
        frameRate = GameRoot.Instance.frameRate;
        SetMove();
    }


    private void Update()
    {
      
        if (BattleSys.Instance.GetPlayerAniState() == AniState.Born)
        {
            return;
        }
        if (currentBlend != targetBlend)
        {
            UpdateMixBlend(jumpDir);
            PECommon.Log("跳跃");
        }
        else
        {
            //if (!ctrl.isGrounded)
            if ((!ctrl.isGrounded && BattleSys.Instance.GetPlayerAniState() == AniState.Fall) || (!ctrl.isGrounded && BattleSys.Instance.GetPlayerAniState() == AniState.Move) )
            {
                PECommon.Log("掉落");
                UpdateMixBlend(fullDir);
            }
            else
            {
                currentBlend = 0;
                targetBlend = 0;
                if (BattleSys.Instance.GetPlayerAniState() == AniState.Fall)
                {
                    //掉落完毕
                    BattleSys.Instance.FallCompelate();
                }
            }
        }

        if (isMove)
        {
            //产生移动
            SetMove();
            //相机跟随
            //SetCamFollow();
            UpdateModelAction();
        }
        if (isRot)
        {
            SetCamRot();
            UpdateModelAction();
            isRot = false;
        }
    }

    /// <summary>
    /// Up ：Y轴上下
    /// </summary>
    /// <param name="up"></param>
    public void SetMove(float up = -0.5f)
    {
        Vector3 v3Dir = new Vector3(Dis.x, 0, Dis.y ); 
        Vector3 FinallyDir = camTrans.forward * v3Dir.z + camTrans.right * v3Dir.x;
        FinallyDir.y = up;

        ctrl.Move(FinallyDir * Constants.PlayerMoveSpeed * Time.deltaTime);
        SetCamFollow();
        SendMoveNetMessage();
    }

    private void SendMoveNetMessage()
    {
        if (timerSvc.GetNowTime() - lastSendMoveTime > frameRate)
        {
            Vector3 pos = UnityTools.RoundTwo(gameObject.transform.position);
            Vector3 rot = UnityTools.RoundTwo(camTrans.localEulerAngles); 
            if (lastPos == pos && lastRot == rot && BattleSys.Instance.GetPlayerAniState() != AniState.Fall)
            {
                return;
            }
            GameMsg gameMsg = new GameMsg
            {
                cmd = (int)CMD.ReqSyncMove,
                reqSyncMove = new ReqSyncMove
                {
                    pos = new MyVector3
                    {
                        x = pos.x,
                        y = pos.y,
                        z = pos.z,
                    },
                    rot = new MyVector3
                    {
                        x = rot.x,
                        y = rot.y,
                        z = rot.z,
                    },
                }

            };
            netSvc.SendMsg(gameMsg);
            lastSendMoveTime = timerSvc.GetNowTime();
            lastPos = pos;
            lastRot = rot;
        }
    }

    public void SetCamFollow()
    {
        if (camTrans != null)
        {
            camTrans.position = transform.position - camOffset;
        }
    }


    public void SetCamRot()
    {
        //限制摄像机上下看
        Vector3 newRot = new Vector3(Dir.y + 360, Dir.x, 0);
        Vector3 totalRot = camTrans.localEulerAngles + newRot;
        Vector3 finallyRot = new Vector3(totalRot.x % 360, totalRot.y % 360, 0);
        if ( finallyRot.x > 88 && finallyRot.x < 272 )
        {
            return;
        }
        camTrans.localEulerAngles = finallyRot;
        Vector3 rot = UnityTools.RoundTwo(camTrans.localEulerAngles);
        if (timerSvc.GetNowTime() - lastSendRotTime > frameRate)
        {
            if (lastRot == rot)
            {
                return;
            }
            GameMsg gameMsg = new GameMsg
            {
                cmd = (int)CMD.ReqSyncRot,
                reqSyncRot = new ReqSyncRot
                {

                    rot = new MyVector3
                    {
                        x = rot.x,
                        y = rot.y,
                        z = rot.z,
                    },
                }
            };
            netSvc.SendMsg(gameMsg);
            lastSendRotTime = timerSvc.GetNowTime();
            lastRot = rot;
        }

    }



    /// <summary>
    /// 更新模型的动作(自己)
    /// </summary>
    public void UpdateModelAction()
    {
       
        if (cameraState == CameraState.Ahead)
        {
            modelItemList[0].transform.localEulerAngles = camTrans.localEulerAngles;
            modelItemList[1].transform.localEulerAngles = camTrans.localEulerAngles;
            modelItemList[2].transform.localEulerAngles = new Vector3(modelItemList[2].transform.localEulerAngles.x, camTrans.localEulerAngles.y, modelItemList[2].transform.localEulerAngles.z);
        }
        else if (cameraState == CameraState.Back)
        {
            Vector3 blueTrans = new Vector3(camTrans.localEulerAngles.x, camTrans.localEulerAngles.y + 180, camTrans.localEulerAngles.z);
            modelItemList[0].transform.localEulerAngles = blueTrans;
            modelItemList[1].transform.localEulerAngles = blueTrans;
            modelItemList[2].transform.localEulerAngles = new Vector3(modelItemList[2].transform.localEulerAngles.x, blueTrans.y, modelItemList[2].transform.localEulerAngles.z);
        }

    }


    private void UpdateMixBlend(Vector3 dir)
    {
        if (Mathf.Abs(currentBlend - targetBlend) < Constants.AccelerateSpeed  * Time.deltaTime)
        {
            currentBlend = targetBlend;
            //跳跃完成
            BattleSys.Instance.JumpCompelate();
            PECommon.Log("跳跃完成,进行下落");

        }
        else if (currentBlend > targetBlend)
        {
            currentBlend -= Constants.AccelerateSpeed * Time.deltaTime;
        }
        else
        {
            currentBlend += Constants.AccelerateSpeed * Time.deltaTime;
        }

        ctrl.Move(dir * currentBlend *  Constants.AccelerateSpeed * Time.deltaTime);
        SetCamFollow();
        SendMoveNetMessage();
    }

    public void SetCamOffset(Vector3 offset)
    {
        camOffset = offset;
    }

    public void SetCamAgain(Vector3 trans)
    {
        camTrans.position = trans;
    }

    public override void CharacterJump()
    {
        targetBlend = Constants.JumpTargetBlend;
    }

    public override void DirJump()
    {
        //当前位置+前方
        Vector2 dir =  BattleSys.Instance.GetPlayerMoveInput();
        jumpDir = new Vector3(dir.x, 1.5f, dir.y);
        fullDir = new Vector3(dir.x, -1.5f, dir.y);

        targetBlend = Constants.JumpTargetBlend;
    }

}