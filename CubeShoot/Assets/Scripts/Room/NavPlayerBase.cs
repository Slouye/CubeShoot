using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class NavPlayerBase : MonoBehaviour {
    public NavMeshAgent m_NavMeshAgent;
    public List<Vector3> pos = new List<Vector3>();
    private int index;
    
	void Start () {
        m_NavMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        Init();
        m_NavMeshAgent.SetDestination(pos[0]);
    }
	
	// Update is called once per frame
	void Update () {
        if (Vector3.Distance(transform.position,pos[index]) <= 1)
        {
            index += 1;
            if (index == 4)
            {
                index = 0;
            }
            m_NavMeshAgent.SetDestination(pos[index]);
        }

    }


    public abstract void Init();
}
