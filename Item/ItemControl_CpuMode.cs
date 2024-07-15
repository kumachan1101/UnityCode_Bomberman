using UnityEngine;

public class ItemControl_CpuMode: ItemControl
{
    protected override void CreateItem_RPC(ABILITY eRand, Vector3 v3){
        CreateItem(eRand, v3);
    }

    public void CreateItem(ABILITY eRand, Vector3 v3){
        GameObject gItem = Create(eRand);
        if(gItem != null){
            GameObject g = Instantiate(gItem);
            g.transform.position = v3;
        }
    }
}
