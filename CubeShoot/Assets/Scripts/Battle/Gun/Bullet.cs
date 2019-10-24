/****************************************************
    文件：Bullet.cs
	作者：Solis
    邮箱: 947064269@qq.com
    日期：2019/9/12 10:5:28
	功能：弹痕
*****************************************************/

using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : BulletBase {

    private Ray ray;
    private RaycastHit hit;

    public ObjectPool bulletPool;
    public ObjectPool hitEffectPool;

    private Transform trailTrans;

    private int taskID = -10;
    private int trailTaskID = -100;

    [SerializeField]  private RoleType casterRoleType;
    public void SetCasterRoleType(RoleType casterRoleType)
    {
        this.casterRoleType = casterRoleType;
    }


    public override void Init()
    {
        trailTrans = transform.Find("Trail").GetComponent<Transform>();
    }

    public void PlayAudio()
    {
        if (M_AudioSource.isPlaying)
        {
            M_AudioSource.Stop();
        }
        M_AudioSource.Play();
    }

    public override void Shoot(Vector3 dir, int force, int damage,  ObjectPool bulletPool)
    {
        trailTrans.gameObject.SetActive(true);
        M_Rigidbody.AddForce(dir * force);
        this.bulletPool = bulletPool;
        ray = new Ray(M_Transform.position, dir);
        this.damage = damage;
        TimerSvc.Instance.AddTimeTask((int tid) =>
        {
            taskID = tid;
            M_Rigidbody.Sleep();
            bulletPool.AddObject(gameObject);
        }, 3000);
        trailTaskID = TimerSvc.Instance.AddTimeTask((int tid) =>
        {
            trailTrans.gameObject.SetActive(false);
        }, 500);
    }
    public override void CollisionEnter(Collision collision)
    {
        PECommon.Log("碰到物体的名字是：" + collision.collider.transform.name);
        M_Rigidbody.Sleep();
        TimerSvc.Instance.RemoveTake(taskID);
        taskID = -10;
        TimerSvc.Instance.RemoveTake(trailTaskID);
        trailTaskID = -100;
        TimerSvc.Instance.AddTimeTask((int tid) =>
        {
            bulletPool.AddObject(gameObject);
        }, 500);
        if (collision.collider.GetComponent<BulletMark>() != null)
        {
            if (Physics.Raycast(ray, out hit, 1000, ~(1<<10))) { }
            collision.collider.GetComponent<BulletMark>().CreateBulletMark(hit);
        }
        GameObject targetPlayer = UnityTools.FindUpParent(collision.collider.transform).gameObject;
        if (targetPlayer.tag == "Player")
        {
            Vector3 hitPos = collision.collider.transform.position;
            MyVector3 hitPosMyV3 = UnityTools.GetMyV3Value(hitPos);
            PECommon.Log("射中的目标最上层物体的名字：" + UnityTools.FindUpParent(collision.collider.transform).gameObject.name);
            //----------------------------------
            if (targetPlayer.GetComponent<ControllerBase>()==null)
            {
                return;
            }
            //别射到自己。
            RoleType targetPlayerRoleType = targetPlayer.GetComponent<ControllerBase>().roleType;
            if (casterRoleType == targetPlayerRoleType)
            {
                return;
            }
            int hurt = 0;
            switch (collision.collider.gameObject.tag)
            {
                case "Arm":
                    hurt = damage / 2;
                    PECommon.Log("造成肢体伤害" + hurt);
                    break;
                case "Body":
                    hurt = damage;
                    PECommon.Log("造成身体伤害" + hurt);
                    break;
                case "Head":
                    hurt = damage * 2;
                    PECommon.Log("造成头部伤害" + hurt);
                    break;
            }

            //发送网络消息
            GameMsg gameMsg = new GameMsg
            {
                cmd = (int)CMD.ReqTakeDamage,
                reqTakeDamage = new ReqTakeDamage
                {
                    damageID = damageID,
                    casterRoleType = casterRoleType,
                    targetRoleType = targetPlayerRoleType,
                    hurt = hurt,
                    hitPos = hitPosMyV3,
                },
            };

            netSvc.SendMsg(gameMsg);
            //准星变红
            if (casterRoleType == BattleSys.Instance.GetCurrentRoleType())
            {
                BattleSys.Instance.SetSignRed();
            }
         
        }
        gameObject.SetActive(false);
    }

}

