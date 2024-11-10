using UnityEngine;

public class ItemControl_CpuMode: ItemControl
{
    protected override void CreateItem_RPC(ABILITY eRand, Vector3 v3){
        CreateItem(eRand, v3);
    }
}
