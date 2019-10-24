using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bplayer : NavPlayerBase
{
    public override void Init()
    {
        pos.Add(new Vector3(3, 0, 3));
        pos.Add(new Vector3(3, 0, -4));
        pos.Add(new Vector3(-3, 0, -4));
        pos.Add(new Vector3(-3, 0, 4.2f));
    }
}
