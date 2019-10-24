using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(ObjectPool))]
public class BulletMark : MonoBehaviour {
    private ObjectPool pool;

    private Texture2D m_BulletMark;     //弹痕贴图
    private Texture2D m_MinTexture;     //模型主贴图
    private Texture2D m_MinTextureBackup_1;     //备份模型主贴图
    private Texture2D m_MinTextureBackup_2;     //备份模型主贴图
    private GameObject prefab_Effect;       //材质特效

    private Transform effectParent;

    Queue<Vector2> bulletMarkQueue;

    void Start () {
        m_BulletMark = Resources.Load<Texture2D>("ResGuns/BulletMarks/imgBulletMark");
        m_MinTexture = (Texture2D)gameObject.GetComponent<MeshRenderer>().material.mainTexture;
        if (m_MinTexture != null)
        {
            m_MinTextureBackup_1 = GameObject.Instantiate<Texture2D>(m_MinTexture);
            m_MinTextureBackup_2 = GameObject.Instantiate<Texture2D>(m_MinTexture);

        }
        gameObject.GetComponent<MeshRenderer>().material.mainTexture = m_MinTextureBackup_1;
        bulletMarkQueue = new Queue<Vector2>();
    }

   

    /// <summary>
    /// 生成弹痕
    /// </summary>
    /// <param name="hit"></param>
    public void CreateBulletMark(RaycastHit hit)
    {
        PlayAudios(hit);

        //获取碰撞点在主贴图上的UV 坐标；
        Vector2 uv = hit.textureCoord;
        bulletMarkQueue.Enqueue(uv);
        if (hit.collider==null)
        {
            return;
        }
        if (hit.collider.transform.localScale.y >= 10 && hit.collider.transform.localScale.y <= 12)
        {
            m_BulletMark = ScaleTexture(m_BulletMark, 10, 2);
        }
        else if (hit.collider.transform.localScale.x == 70)
        {
            m_BulletMark = ScaleTexture(m_BulletMark, 2, 20);
        }
        else
        {
            m_BulletMark = ScaleTexture(m_BulletMark, 20, 20);
        }

       

        //痕贴图的宽
        for (int i = 0; i < m_BulletMark.width; i++)
        {
            //痕贴图的高
            for (int j = 0; j < m_BulletMark.height; j++)
            {
                //uv.x* 主贴图宽度-弹痕贴图宽度 / 2 + i;
                float x = uv.x * m_MinTextureBackup_1.width - m_BulletMark.width / 2 + i;

                //uv.y* 主贴图高度-弹痕贴图高度 / 2 + j;
                float y = uv.y * m_MinTextureBackup_1.height - m_BulletMark.height / 2 + j;

                //通过循环索引获取弹痕像素点的颜色值；
                Color color = m_BulletMark.GetPixel(i, j);
                if (color.a >= 0.2f)
                {
                    //在主贴图的相应位置设置新的像素值；
                    m_MinTextureBackup_1.SetPixel((int)x, (int)y, color);
                }
                
            }
        }
        m_MinTextureBackup_1.Apply();

        //播放特效
        PlayEffect(hit);

        //5秒后消除弹痕
        Invoke("ResetBulletMark",5);
    }

    //消除弹痕
    private void ResetBulletMark()
    {
        if (bulletMarkQueue.Count > 0)
        {
            Vector2 uv = bulletMarkQueue.Dequeue();
            for (int i = 0; i < m_BulletMark.width; i++)
            {
                for (int j = 0; j < m_BulletMark.height; j++)
                {
                    float x = uv.x * m_MinTextureBackup_2.width - m_BulletMark.width / 2 + i;
                    float y = uv.y * m_MinTextureBackup_2.height - m_BulletMark.height / 2 + j;
                    Color color = m_MinTextureBackup_2.GetPixel((int)x, (int)y);
                    m_MinTextureBackup_1.SetPixel((int)x, (int)y, color);
                }
            }
            m_MinTextureBackup_1.Apply();
        }
    }

    /// <summary>
    /// 播放特效
    /// </summary>
    private void PlayEffect(RaycastHit hit)
    {
       
    }

    private IEnumerator Delay(GameObject go,float time)
    {
        yield return new WaitForSeconds(time);
        //pool.AddObject(go);
    }

    /// <summary>
    /// 播放三类音效
    /// </summary>
    private void PlayAudios(RaycastHit hit)
    {
      
       
    }

    private Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, false);
        //float incX = (1.0f / targetWidth);
        //float incY = (1.0f / targetHeight);
        for (int i = 0; i < result.height; ++i)
        {
            for (int j = 0; j < result.width; ++j)
            {
                Color newColor = source.GetPixelBilinear((float)j / (float)result.width, (float)i / (float)result.height);
                result.SetPixel(j, i, newColor);
            }
        }
        result.Apply();
        return result;
    }
}
