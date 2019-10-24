/****************************************************
    文件：ObjectPool.cs
	作者：947064269
    邮箱: 947064269@qq.com
    日期：2019/9/8 16:6:12
	功能：对象池
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour {
    private Queue<GameObject> pool;

    private void Awake()
    {
        pool = new Queue<GameObject>();
    }

    public void AddObject(GameObject go)
    {
        go.SetActive(false);
        pool.Enqueue(go);
    }

    public GameObject GetObject()
    {
        GameObject temp = null;
        if (pool.Count > 0)
        {
            temp = pool.Dequeue();
            return temp;
        }
        return temp;
    }

    public bool Data()
    {
        if (pool.Count > 0)
            return true;
        else
            return false;
    }
}
