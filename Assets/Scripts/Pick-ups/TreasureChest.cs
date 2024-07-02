using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureChest : Pickup
{
    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (target)
            GameManager.instance.StartLevelUp(true);
    }
}
