/****************************************************
    文件：Bullet.cs
	作者：Solis
    邮箱: 947064269@qq.com
    日期：2019/9/12 10:3:47
	功能：弹痕父类
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BulletBase : MonoBehaviour {
    private Transform m_Transform;
    private Rigidbody m_Rigidbody;
    private AudioSource m_AudioSource;

    protected int damage;

    protected NetSvc netSvc;

    public int damageID = 1;

    public Transform M_Transform { get { return m_Transform; } }
    public Rigidbody M_Rigidbody { get { return m_Rigidbody; } }
    public AudioSource M_AudioSource { get { return m_AudioSource; } }

    public int Damage { get { return damage; } set { damage = value; } }
    private void Awake()
    {
        m_Transform = gameObject.GetComponent<Transform>();
        m_Rigidbody = gameObject.GetComponent<Rigidbody>();
        m_AudioSource = gameObject.GetComponent<AudioSource>();
        netSvc = NetSvc.Instance;
        Init();
    }

    private void OnCollisionEnter(Collision collision)
    {
        CollisionEnter(collision);
    }

    public IEnumerator TailAnimation(Transform Pivot)
    {
        //停止时间
        float shopTime = Time.time + 1.0f;

        //动画颤动的范围
        float range = 1.0f;

        float vel = 0;

        //颤动起始位置
        //Quaternion startRot = Quaternion.Euler(new Vector3(Random.Range(5.0f, -5.0f), Random.Range(5.0f, -5.0f), 0));

        while (Time.time < shopTime)
        {
            Pivot.localRotation = Quaternion.Euler(new Vector3(Random.Range(range, -range), Random.Range(range, -range), 0));

            //平滑阻尼（动画的运动范围,动画的最终位置,这个数据我们保持为0 即可,动画时长）
            //平滑缓冲，东西不是僵硬的移动而是做减速缓冲运动到指定位置
            range = Mathf.SmoothDamp(range, 0, ref vel, shopTime - Time.time);

            yield return null;
        }
    }

    public void KillSelf()
    {
        TimerSvc.Instance.AddTimeTask((int tid) =>
        {
            GameObject.Destroy(gameObject);
        }, 3000);
      
    }

    public abstract void Init();
    public abstract void Shoot(Vector3 dir, int force, int Damage,  ObjectPool bulletPool);
    public abstract void CollisionEnter(Collision collision);

}
