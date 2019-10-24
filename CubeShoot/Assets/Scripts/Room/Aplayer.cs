using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aplayer : NavPlayerBase {

    public override void Init()
    {
        pos.Add(new Vector3(3, 0, -4));
        pos.Add(new Vector3(-3, 0, -4));
        pos.Add(new Vector3(-3, 0, 4.2f));
        pos.Add(new Vector3(3, 0, 3));
    }
}
