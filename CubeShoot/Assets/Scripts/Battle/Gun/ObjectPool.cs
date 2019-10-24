/****************************************************
    �ļ���ObjectPool.cs
	���ߣ�947064269
    ����: 947064269@qq.com
    ���ڣ�2019/9/8 16:6:12
	���ܣ������
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
